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
            var agent = new DebugAgent();
            var controller = new GameController();
            controller.StartGame(new Agent[] {agent}, 0, 0);
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

