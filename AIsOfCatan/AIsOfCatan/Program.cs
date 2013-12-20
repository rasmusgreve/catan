using System;
using System.Collections.Generic;
using System.Linq;
using AIsOfCatan.API;

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
            PerformGameBenchmarking();
            
            /*var controller = new GameController();
            var agents = new IAgent[] { new StarterAgent(), new StarterAgent(), new StarterAgent(), new StarterAgent() }; // new HumanAgent()
            int intWinner = controller.StartGame(agents, 0, 0, false, true);

            if (intWinner != -1)
            {
                var winner = agents[intWinner];
                Console.WriteLine("Winner is: " + winner.GetName() + " (" + intWinner + ")");
            }
            else
            {
                Console.WriteLine("It is a draw after 100 rounds");
            }
            
            Console.WriteLine(controller.GetBoard());*/
        }

        static void PerformGameBenchmarking()
        {
            Dictionary<int, int> wins = new Dictionary<int, int>(4);

            for (int i = 0; i < 10000; i++)
            {
                var controller = new GameController();
                var agents = new IAgent[] { new StarterAgent(), new StarterAgent(), new StarterAgent() }; // new HumanAgent()
                int intWinner = controller.StartGame(agents, i, i, false, false);
                //var winner = agents[intWinner];

                if (wins.ContainsKey(intWinner))
                {
                    wins[intWinner] = wins[intWinner] + 1;
                }
                else
                {
                    wins.Add(intWinner, 1);
                }
                Console.WriteLine(i + ": P " + intWinner + " wins.");
            }

            Console.WriteLine();
            wins.OrderBy(w => w.Key).ForEach(kv => Console.WriteLine("Player " + kv.Key + ": " + kv.Value + " wins."));
        }
    }
#endif
}

