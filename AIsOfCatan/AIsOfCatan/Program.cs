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
            using (GUIControl game = new GUIControl())
            {
                game.Run();
            }
        }
    }
#endif
}

