using System;

namespace WindowsGame4
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1(1024,768,13))
            {
                game.Run();
            }
        }
    }
}
