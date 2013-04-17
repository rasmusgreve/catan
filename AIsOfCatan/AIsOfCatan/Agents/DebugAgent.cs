using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class DebugAgent : IAgent
    {
        private int id;
        private int[] start1 = new[] { 8, 9, 15 };
        private bool firstStartPlaced = false;
        private int[] start2 = new[] { 9, 10, 16 };
        private int[] tooClose = new[] { 9, 15, 16 };
        private int[] farRoad = new[] { 34, 35 };
        public void Reset(int assignedId)
        {
            id = assignedId;
            Console.WriteLine("IAgent reset with id " + id);
        }

        public void PlaceStart(IGameState state, IStartActions actions)
        {
            Console.WriteLine(id + ": Place starts");
            if (!firstStartPlaced)
            {
                firstStartPlaced = true;
                actions.BuildSettlement(start1[0], start1[1], start1[2]);
                Console.WriteLine(id + ": First settlement built succesfully");
                actions.BuildRoad(start1[0], start1[1]);
                Console.WriteLine(id + ": First road built succesfully");
            }
            else
            {
                try
                {
                    actions.BuildSettlement(start1[0], start1[1], start1[2]);
                    Console.WriteLine(id + ": Controller allowed a building on top of another");
                }
                catch (IllegalBuildPositionException e)
                {
                    Console.WriteLine(id + ": Controller threw exception as expected: " + e.Message);
                }
                try
                {
                    actions.BuildSettlement(tooClose[0], tooClose[1], tooClose[2]);
                    Console.WriteLine(id + ": Controller allowed a building too close");
                }
                catch (IllegalBuildPositionException e)
                {
                    Console.WriteLine(id + ": Controller threw exception as expected: " + e.Message);
                }
                actions.BuildSettlement(start2[0], start2[1], start2[2]);
                Console.WriteLine(id + ": Second settlement built succesfully");
                try
                {
                    actions.BuildRoad(farRoad[0], farRoad[1]);
                    Console.WriteLine(id + ": Controller allowed a building disconnected road");
                }
                catch (IllegalBuildPositionException e)
                {
                    Console.WriteLine(id + ": Controller threw exception as expected: " + e.Message);
                }
                actions.BuildRoad(start2[0], start2[1]);
                Console.WriteLine(id + ": Second road built succesfully");
            }
        }

        public void BeforeDiceRoll(IGameState state, IGameActions actions)
        {
            Console.WriteLine(id + ": Before dice roll");
            System.Threading.Thread.Sleep(100);
        }

        public int MoveRobber(IGameState state)
        {
            Console.WriteLine(id + ": Move robber");
            System.Threading.Thread.Sleep(1000);
            return ((GameState)state).Board.GetRobberLocation() == 8 ? 9 : 8;
        }

        public int ChoosePlayerToDrawFrom(int[] validOpponents)
        {
            Console.WriteLine(id + ": Choosing opponent to draw from");
            System.Threading.Thread.Sleep(1000);
            return validOpponents[0];
        }

        public Resource[] DiscardCards(IGameState state, int toDiscard)
        {
            Console.WriteLine(id + ": Choosing cards to discard");
            System.Threading.Thread.Sleep(1000);
            return ((GameState)state).GetOwnResources().Take(toDiscard).ToArray();
        }

        public void PerformTurn(IGameState state, IGameActions actions)
        {
            Console.WriteLine(id + ": Performing main turn");
            var resources = ((GameState)state).GetOwnResources();
            int[] resCount = new int[5];
            foreach (var r in resources)
                resCount[(int)r]++;
            Console.Write(id + ": Resources: ( ");
            foreach (var i in resCount)
                Console.Write(i + " ");
            Console.WriteLine(")");
            System.Threading.Thread.Sleep(1000);
        }

        public ITrade HandleTrade(ITrade offer, int proposingPlayerId)
        {
            Console.WriteLine(id + ": Handling trade");
            return offer;
        }
    }
}
