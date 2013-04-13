using System;

namespace AIsOfCatan
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            GameState state = new GameState(0);
            System.Diagnostics.Debug.WriteLine(state.ToString());

            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

