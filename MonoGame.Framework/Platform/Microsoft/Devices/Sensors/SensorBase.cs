// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Monogame;

namespace Microsoft.Devices.Sensors;

public abstract class SensorBase<TSensorReading> : IDisposable where TSensorReading : ISensorReading
{
#if IOS
    protected static readonly CoreMotion.CMMotionManager motionManager = new CoreMotion.CMMotionManager();
#endif
    bool disposed;
    private TimeSpan timeBetweenUpdates;
    private TSensorReading currentValue;

    public TSensorReading CurrentValue
    {
        get => currentValue;
        protected set
        {
            currentValue = value;
            CurrentValueChanged?.Invoke(currentValue);
        }
    }

    public bool IsDataValid { get; protected set; }

    public TimeSpan TimeBetweenUpdates
    {
        get => timeBetweenUpdates;
        set
        {
            if (timeBetweenUpdates != value)
            {
                timeBetweenUpdates = value;
                TimeBetweenUpdatesChanged?.Invoke();
            }
        }
    }

    public event Action<TSensorReading> CurrentValueChanged;
    protected event Action TimeBetweenUpdatesChanged;

    protected bool IsDisposed => disposed;

    public SensorBase()
    {
        this.TimeBetweenUpdates = TimeSpan.FromMilliseconds(2);
    }

    ~SensorBase()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Derived classes override this method to dispose of managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">True if unmanaged resources are to be disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        disposed = true;
    }

    public abstract void Start();

    public abstract void Stop();
}