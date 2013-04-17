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
            if (false) //Set to false if you wanna see the GUI
            {
                var agent = new DebugAgent();
                var controller = new GameController();
                controller.StartGame(new IAgent[] { agent }, 0, 0);
            }

            using (GUIControl game = new GUIControl())
            {
                game.Run();
            }
        }
    }
#endif
}

