namespace Test.DesktopGL;

public class Program
{
    public static void Main(string[] args)
    {
        using var game = new TestGame();
        game.Run();
    }
}