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
            var agent1 = new DebugAgent();
            var controller = new GameController();
            if (!true) //Set to false if you wanna see the GUI
                controller.StartGame(new Agent[] { agent1 }, 0, 0);

            using (GUIControl game = new GUIControl())
            {
                game.Run();
            }
        }
    }
#endif
}

