// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
#if WINDOWS_UAP
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
#endif
using Monogame.Audio;
using Monogame.Content;
using Monogame.Graphics;
using Monogame.Input.Touch;


namespace Monogame;

/// <summary>
/// This class is the entry point for most games. Handles setting up
/// a window and graphics and runs a game loop that calls <see cref="Update"/> and <see cref="Draw"/>.
/// </summary>
public partial class Game : IDisposable
{
    private GameServiceContainer _services;
    private ContentManager _content;
    internal GamePlatform Platform;

    private IGraphicsDeviceManager _graphicsDeviceManager;
    private IGraphicsDeviceService _graphicsDeviceService;

    private bool _initialized = false;
    private bool _isFixedTimeStep = true;

    private TimeSpan _targetElapsedTime = TimeSpan.FromTicks(166667); // 60fps
    private TimeSpan _inactiveSleepTime = TimeSpan.FromSeconds(0.02);

    private TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500);

    private bool _shouldExit;
    private bool _suppressDraw;

    partial void PlatformConstruct();

    /// <summary>
    /// Create a <see cref="Game"/>.
    /// </summary>
    public Game()
    {
        _instance = this;

        _services = new GameServiceContainer();
        _content = new ContentManager(_services);

        Platform = GamePlatform.PlatformCreate(this);
        Platform.Activated += OnActivated;
        Platform.Deactivated += OnDeactivated;
        _services.AddService(typeof(GamePlatform), Platform);

        // Calling Update() for first time initializes some systems
        FrameworkDispatcher.Update();

        // Allow some optional per-platform construction to occur too.
        PlatformConstruct();

    }

    /// <summary/>
    ~Game()
    {
        Dispose(false);
    }

    [Conditional("DEBUG")]
    internal void Log(string Message)
    {
        Platform?.Log(Message);
    }

    #region IDisposable Implementation

    private bool _isDisposed;
    /// <inheritdoc cref="IDisposable.Dispose()"/>
    public void Dispose()
    {
        Dispose(true);
        Disposed?.Invoke();
        GC.SuppressFinalize(this);
    }

    /// <summary/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                if (_content != null)
                {
                    _content.Dispose();
                    _content = null;
                }

                if (_graphicsDeviceManager != null)
                {
                    (_graphicsDeviceManager as GraphicsDeviceManager).Dispose();
                    _graphicsDeviceManager = null;
                }

                if (Platform != null)
                {
                    Platform.Activated -= OnActivated;
                    Platform.Deactivated -= OnDeactivated;
                    _services.RemoveService(typeof(GamePlatform));

                    Platform.Dispose();
                    Platform = null;
                }

                ContentTypeReaderManager.ClearTypeCreators();

                if (SoundEffect._systemState == SoundEffect.SoundSystemState.Initialized)
                    SoundEffect.PlatformShutdown();
            }
#if ANDROID
            Activity = null;
#endif
            _isDisposed = true;
            _instance = null;
        }
    }

    [DebuggerNonUserCode]
    private void AssertNotDisposed()
    {
        if (_isDisposed)
        {
            string name = GetType().Name;
            throw new ObjectDisposedException(
                name, string.Format("The {0} object was used after being Disposed.", name));
        }
    }

    #endregion IDisposable Implementation

    #region Properties

#if ANDROID
    public static AndroidGameActivity Activity { get; internal set; }
#endif
    private static Game _instance = null;
    internal static Game Instance { get { return Game._instance; } }

    /// <summary>
    /// Gets or sets time to sleep between frames when the game is not active
    /// </summary>
    public TimeSpan InactiveSleepTime
    {
        get { return _inactiveSleepTime; }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, TimeSpan.Zero);

            _inactiveSleepTime = value;
        }
    }

    /// <summary>
    /// The maximum amount of time we will frameskip over and only perform Update calls with no Draw calls.
    /// MonoGame extension.
    /// </summary>
    public TimeSpan MaxElapsedTime
    {
        get { return _maxElapsedTime; }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfLessThan(value, _targetElapsedTime);

            _maxElapsedTime = value;
        }
    }

    /// <summary>
    /// Indicates if the game is the focused application.
    /// </summary>
    public bool IsActive => Platform.IsActive;

    /// <summary>
    /// Indicates if the mouse cursor is visible on the game screen.
    /// </summary>
    public bool IsMouseVisible
    {
        get => Platform.IsMouseVisible;
        set => Platform.IsMouseVisible = value;
    }

    /// <summary>
    /// The time between frames when running with a fixed time step. <seealso cref="IsFixedTimeStep"/>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Target elapsed time must be strictly larger than zero.</exception>
    public TimeSpan TargetElapsedTime
    {
        get { return _targetElapsedTime; }
        set
        {
            // Give GamePlatform implementations an opportunity to override
            // the new value.
            value = Platform.TargetElapsedTimeChanging(value);

            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, _maxElapsedTime);

            if (value != _targetElapsedTime)
            {
                _targetElapsedTime = value;
                Platform.TargetElapsedTimeChanged();
            }
        }
    }


    /// <summary>
    /// Indicates if this game is running with a fixed time between frames.
    /// 
    /// When set to <code>true</code> the target time between frames is
    /// given by <see cref="TargetElapsedTime"/>.
    /// </summary>
    public bool IsFixedTimeStep { get => _isFixedTimeStep; set => _isFixedTimeStep = value; }

    /// <summary>
    /// Get a container holding service providers attached to this <see cref="Game"/>.
    /// </summary>
    public GameServiceContainer Services => _services;

    /// <summary>
    /// The <see cref="ContentManager"/> of this <see cref="Game"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">If Content is set to <code>null</code>.</exception>
    public ContentManager Content
    {
        get => _content;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _content = value;
        }
    }

    /// <summary>
    /// Gets the <see cref="GraphicsDevice"/> used for rendering by this <see cref="Game"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// There is no <see cref="Graphics.GraphicsDevice"/> attached to this <see cref="Game"/>.
    /// </exception>
    public GraphicsDevice GraphicsDevice
    {
        get
        {
            if (_graphicsDeviceService == null)
            {
                _graphicsDeviceService = (IGraphicsDeviceService)
                    Services.GetService(typeof(IGraphicsDeviceService));

                if (_graphicsDeviceService == null)
                    throw new InvalidOperationException("No Graphics Device Service");
            }
            return _graphicsDeviceService.GraphicsDevice;
        }
    }

    /// <summary>
    /// The system window that this game is displayed on.
    /// </summary>
    public GameWindow Window
    {
        get { return Platform.Window; }
    }

    #endregion Properties

    #region Internal Properties

    // FIXME: Internal members should be eliminated.
    // Currently Game.Initialized is used by the Mac game window class to
    // determine whether to raise DeviceResetting and DeviceReset on
    // GraphicsDeviceManager.
    internal bool Initialized => _initialized;

    #endregion Internal Properties

    #region Events

    /// <summary>
    /// Raised when the game gains focus.
    /// </summary>
    public event Action Activated;

    /// <summary>
    /// Raised when the game loses focus.
    /// </summary>
    public event Action Deactivated;

    /// <summary>
    /// Raised when this game is being disposed.
    /// </summary>
    public event Action Disposed;

#if WINDOWS_UAP
    public ApplicationExecutionState PreviousExecutionState { get; internal set; }
#endif

    #endregion

    #region Public Methods

    /// <summary>
    /// Exit the game at the end of this tick.
    /// </summary>
#if IOS
    [Obsolete("This platform's policy does not allow programmatically closing.", true)]
#endif
    public void Exit()
    {
        _shouldExit = true;
        _suppressDraw = true;
    }

    /// <summary>
    /// Reset the elapsed game time to <see cref="TimeSpan.Zero"/>.
    /// </summary>
    public void ResetElapsedTime()
    {
        Platform.ResetElapsedTime();
        if (_gameTimer != null)
        {
            _gameTimer.Reset();
            _gameTimer.Start();
        }

        _accumulatedElapsedTime = TimeSpan.Zero;
        _gameTime.ElapsedGameTime = TimeSpan.Zero;
        _previousTicks = 0L;
    }

    /// <summary>
    /// Supress calling <see cref="Draw"/> in the game loop.
    /// </summary>
    public void SuppressDraw()
    {
        _suppressDraw = true;
    }

    /// <summary>
    /// Run the game for one frame, then exit.
    /// </summary>
    public void RunOneFrame()
    {
        if (Platform == null)
            return;

        if (!Platform.BeforeRun())
            return;

        if (!_initialized)
        {
            DoInitialize();
            _gameTimer = Stopwatch.StartNew();
            _initialized = true;
        }

        BeginRun();

        // Not quite right..
        Tick();

        EndRun();

    }

    /// <summary>
    /// Run the game using the default <see cref="GameRunBehavior"/> for the current platform.
    /// </summary>
    public void Run()
    {
        Run(Platform.DefaultRunBehavior);
    }

    /// <summary>
    /// Run the game.
    /// </summary>
    /// <param name="runBehavior">Indicate if the game should be run synchronously or asynchronously.</param>
    public void Run(GameRunBehavior runBehavior)
    {
        AssertNotDisposed();
        if (!Platform.BeforeRun())
        {
            BeginRun();
            _gameTimer = Stopwatch.StartNew();
            return;
        }

        if (!_initialized)
        {
            DoInitialize();
            _initialized = true;
        }

        BeginRun();
        _gameTimer = Stopwatch.StartNew();
        switch (runBehavior)
        {
        case GameRunBehavior.Asynchronous:
            Platform.AsyncRunLoopEnded += Platform_AsyncRunLoopEnded;
            Platform.StartRunLoop();
            break;
        case GameRunBehavior.Synchronous:
            // XNA runs one Update even before showing the window
            DoUpdate(new GameTime());

            Platform.RunLoop();
            break;
        default:
            throw new ArgumentException(string.Format("Handling for the run behavior {0} is not implemented.", runBehavior));
        }
    }

    private TimeSpan _accumulatedElapsedTime;
    private readonly GameTime _gameTime = new GameTime();
    private Stopwatch _gameTimer;
    private long _previousTicks = 0;
    private int _updateFrameLag;
#if WINDOWS_UAP
    private readonly object _locker = new object();
#endif

    /// <summary>
    /// Run one iteration of the game loop.
    ///
    /// Makes at least one call to <see cref="Update"/>
    /// and exactly one call to <see cref="Draw"/> if drawing is not supressed.
    /// When <see cref="IsFixedTimeStep"/> is set to <code>false</code> this will
    /// make exactly one call to <see cref="Update"/>.
    /// </summary>
    public void Tick()
    {
    // NOTE: This code is very sensitive and can break very badly
    // with even what looks like a safe change.  Be sure to test 
    // any change fully in both the fixed and variable timestep 
    // modes across multiple devices and platforms.

    RetryTick:

        if (!IsActive && (InactiveSleepTime.TotalMilliseconds >= 1.0))
        {
#if WINDOWS_UAP
            lock (_locker)
                System.Threading.Monitor.Wait(_locker, (int)InactiveSleepTime.TotalMilliseconds);
#else
            System.Threading.Thread.Sleep((int)InactiveSleepTime.TotalMilliseconds);
#endif
        }

        // Advance the accumulated elapsed time.
        if (_gameTimer == null)
        {
            _gameTimer = new Stopwatch();
            _gameTimer.Start();
        }
        var currentTicks = _gameTimer.Elapsed.Ticks;
        _accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
        _previousTicks = currentTicks;

        if (IsFixedTimeStep && _accumulatedElapsedTime < TargetElapsedTime)
        {
            // Sleep for as long as possible without overshooting the update time
            var sleepTime = (TargetElapsedTime - _accumulatedElapsedTime).TotalMilliseconds;
            // We only have a precision timer on Windows, so other platforms may still overshoot
#if WINDOWS && !DESKTOPGL
            MonoGame.Framework.Utilities.TimerHelper.SleepForNoMoreThan(sleepTime);
#elif WINDOWS_UAP
            lock (_locker)
                if (sleepTime >= 2.0)
                    System.Threading.Monitor.Wait(_locker, 1);
#elif DESKTOPGL || ANDROID || IOS
            if (sleepTime >= 2.0)
                System.Threading.Thread.Sleep(1);
#endif
            // Keep looping until it's time to perform the next update
            goto RetryTick;
        }

        // Do not allow any update to take longer than our maximum.
        if (_accumulatedElapsedTime > _maxElapsedTime)
            _accumulatedElapsedTime = _maxElapsedTime;

        if (IsFixedTimeStep)
        {
            _gameTime.ElapsedGameTime = TargetElapsedTime;
            var stepCount = 0;

            // Perform as many full fixed length time steps as we can.
            while (_accumulatedElapsedTime >= TargetElapsedTime && !_shouldExit)
            {
                _gameTime.TotalGameTime += TargetElapsedTime;
                _accumulatedElapsedTime -= TargetElapsedTime;
                ++stepCount;

                DoUpdate(_gameTime);
            }

            //Every update after the first accumulates lag
            _updateFrameLag += Math.Max(0, stepCount - 1);

            //If we think we are running slowly, wait until the lag clears before resetting it
            if (_gameTime.IsRunningSlowly)
            {
                if (_updateFrameLag == 0)
                    _gameTime.IsRunningSlowly = false;
            }
            else if (_updateFrameLag >= 5)
            {
                //If we lag more than 5 frames, start thinking we are running slowly
                _gameTime.IsRunningSlowly = true;
            }

            //Every time we just do one update and one draw, then we are not running slowly, so decrease the lag
            if (stepCount == 1 && _updateFrameLag > 0)
                _updateFrameLag--;

            // Draw needs to know the total elapsed time
            // that occured for the fixed length updates.
            _gameTime.ElapsedGameTime = TimeSpan.FromTicks(TargetElapsedTime.Ticks * stepCount);
        }
        else
        {
            // Perform a single variable length update.
            _gameTime.ElapsedGameTime = _accumulatedElapsedTime;
            _gameTime.TotalGameTime += _accumulatedElapsedTime;
            _accumulatedElapsedTime = TimeSpan.Zero;

            DoUpdate(_gameTime);
        }

        // Draw unless the update suppressed it.
        if (_suppressDraw)
            _suppressDraw = false;
        else
        {
            DoDraw(_gameTime);
        }

        if (_shouldExit)
        {
            bool cancel = OnExiting();

            if (!cancel)
            {
                Platform.Exit();
                EndRun();
                UnloadContent();
            }

            _shouldExit = false;
        }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Called right before <see cref="Draw"/> is normally called. Can return <code>false</code>
    /// to let the game loop not call <see cref="Draw"/>.
    /// </summary>
    /// <returns>
    ///   <code>true</code> if <see cref="Draw"/> should be called, <code>false</code> if it should not.
    /// </returns>
    protected virtual bool BeginDraw() { return true; }

    /// <summary>
    /// Called right after <see cref="Draw"/>. Presents the
    /// rendered frame in the <see cref="GameWindow"/>.
    /// </summary>
    protected virtual void EndDraw()
    {
        Platform.Present();
    }

    /// <summary>
    /// Called after <see cref="Initialize"/>, but before the first call to <see cref="Update"/>.
    /// </summary>
    protected virtual void BeginRun() { }

    /// <summary>
    /// Called when the game loop has been terminated before exiting.
    /// </summary>
    protected virtual void EndRun() { }

    /// <summary>
    /// Override this to load graphical resources required by the game.
    /// </summary>
    protected virtual void LoadContent() { }

    /// <summary>
    /// Override this to unload graphical resources loaded by the game.
    /// </summary>
    protected virtual void UnloadContent() { }

    /// <summary>
    /// Override this to initialize the game and load any needed non-graphical resources.
    /// </summary>
    protected virtual void Initialize()
    {
        // TODO: This should be removed once all platforms use the new GraphicsDeviceManager
#if !(WINDOWS && DIRECTX)
        ApplyChanges(graphicsDeviceManager);
#endif

        _graphicsDeviceService = (IGraphicsDeviceService)
            Services.GetService(typeof(IGraphicsDeviceService));

        if (_graphicsDeviceService != null &&
            _graphicsDeviceService.GraphicsDevice != null)
        {
            LoadContent();
        }
    }

    /// <summary>
    /// Called when the game should draw a frame.
    /// Override this to render your game.
    /// </summary>
    /// <param name="gameTime">A <see cref="GameTime"/> instance containing the elapsed time since the last call to <see cref="Draw"/> and the total time elapsed since the game started.</param>
    protected virtual void Draw(GameTime gameTime)
    {
    }

    /// <summary>
    /// Called when the game should update.
    /// Override this to update your game.
    /// </summary>
    /// <param name="gameTime">The elapsed time since the last call to <see cref="Update"/>.</param>
    protected virtual void Update(GameTime gameTime)
    {
    }

    /// <summary>
    /// Called when the game is exiting.
    /// </summary>
    protected virtual bool OnExiting()
    {
        return false;
    }

    /// <summary>
    /// Called when the game gains focus. Raises the <see cref="Activated"/> event.
    /// </summary>
    protected virtual void OnActivated()
    {
        AssertNotDisposed();
        Activated?.Invoke();
    }

    /// <summary>
    /// Called when the game loses focus. Raises the <see cref="Deactivated"/> event.
    /// </summary>
    protected virtual void OnDeactivated()
    {
        AssertNotDisposed();
        Deactivated?.Invoke();
    }

    #endregion Protected Methods

    #region Event Handlers

    private void Platform_AsyncRunLoopEnded()
    {
        AssertNotDisposed();
        Platform.AsyncRunLoopEnded -= Platform_AsyncRunLoopEnded;
    }

    #endregion Event Handlers

    #region Internal Methods

    // FIXME: We should work toward eliminating internal methods.  They
    //        break entirely the possibility that additional platforms could
    //        be added by third parties without changing MonoGame itself.

#if !(WINDOWS && DIRECTX)
    internal void ApplyChanges(GraphicsDeviceManager manager)
    {
        Platform.BeginScreenDeviceChange(GraphicsDevice.PresentationParameters.IsFullScreen);

        if (GraphicsDevice.PresentationParameters.IsFullScreen)
            Platform.EnterFullScreen();
        else
            Platform.ExitFullScreen();
        var viewport = new Viewport(0, 0,
                                    GraphicsDevice.PresentationParameters.BackBufferWidth,
                                    GraphicsDevice.PresentationParameters.BackBufferHeight);

        GraphicsDevice.Viewport = viewport;
        Platform.EndScreenDeviceChange(string.Empty, viewport.Width, viewport.Height);
    }
#endif

    internal void DoUpdate(GameTime gameTime)
    {
        AssertNotDisposed();
        if (Platform.BeforeUpdate(gameTime))
        {
            FrameworkDispatcher.Update();

            Update(gameTime);

            //The TouchPanel needs to know the time for when touches arrive
            TouchPanelState.CurrentTimestamp = gameTime.TotalGameTime;
        }
    }

    internal void DoDraw(GameTime gameTime)
    {
        AssertNotDisposed();
        // Draw and EndDraw should not be called if BeginDraw returns false.
        // http://stackoverflow.com/questions/4054936/manual-control-over-when-to-redraw-the-screen/4057180#4057180
        // http://stackoverflow.com/questions/4235439/xna-3-1-to-4-0-requires-constant-redraw-or-will-display-a-purple-screen
        if (Platform.BeforeDraw(gameTime) && BeginDraw())
        {
            Draw(gameTime);
            EndDraw();
        }
    }

    internal void DoInitialize()
    {
        AssertNotDisposed();
        if (GraphicsDevice == null && graphicsDeviceManager != null)
            _graphicsDeviceManager.CreateDevice();

        Platform.BeforeInitialize();
        Initialize();
    }

    #endregion Internal Methods

    internal GraphicsDeviceManager graphicsDeviceManager
    {
        get
        {
            _graphicsDeviceManager ??= Services.GetService<IGraphicsDeviceManager>();
            return (GraphicsDeviceManager)_graphicsDeviceManager;
        }
        set
        {
            if (_graphicsDeviceManager != null)
                throw new InvalidOperationException("GraphicsDeviceManager already registered for this Game object");
            _graphicsDeviceManager = value;
        }
    }
}