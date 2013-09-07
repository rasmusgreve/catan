using System;
using System.Collections.Generic;
using System.Linq;

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
            if (true) //Set to false if you wanna see the GUI
            {
                /*var agent1 = new DebugAgent();
                var agent2 = new HumanAgent();
                var controller = new GameController();
                controller.StartGame(new IAgent[] { agent1, agent2 }, 0, 0);*/

                IBoard b = new Board(0).PlaceRoad(14, 20, 0).PlaceRoad(20, 21, 0).PlaceRoad(21, 27, 0).PlaceRoad(21, 28, 0).PlaceRoad(21, 22, 0).PlaceRoad(15, 21, 0).PlaceRoad(14, 21, 0).PlaceRoad(14, 15, 0).PlaceRoad(15, 16, 1).PlacePiece(20, 21, 27, new Board.Piece(Token.City, 1));
                for (int i = 0; i < 20; i++)
                {
                    DateTime start = DateTime.Now;
                    Dictionary<int, int> results = b.GetLongestRoad();
                    int player = results.OrderByDescending(r => r.Value).First().Key;
                    int length = results[player];
                    Console.WriteLine("Time: " + (DateTime.Now - start).TotalMilliseconds);
                    Console.WriteLine("Longest Road; Player " + player + " with " + length);
                }
                Console.WriteLine("Player 0's harbors: " + b.GetPlayersHarbors(0).Length);

                Console.ReadLine();
            }

            using (GUIControl game = new GUIControl())
            {
                game.Run();
            }
        }
    }
#endif
}

