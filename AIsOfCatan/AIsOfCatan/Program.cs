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
            /*using (Game1 game = new Game1())
            {
                game.Run();
            }*/

            GameState state = new GameState(0);
            System.Diagnostics.Debug.WriteLine(state.ToString());
        }
    }
#endif
}

