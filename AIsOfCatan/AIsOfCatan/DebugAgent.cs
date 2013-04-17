using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class DebugAgent : Agent
    {
        private int id;
        private int[] start1 = new[] { 8, 9, 15};
        private bool firstStartPlaced = false;
        private int[] start2 = new[] { 9, 10, 16};
        private int[] tooClose = new[] {9, 15, 16};
        private int[] farRoad = new[] {34, 35};
        public void Reset(int assignedId)
        {
            id = assignedId;
            Console.WriteLine("Agent reset with id " + id);
        }

        public void PlaceStart(GameState state, StartActions actions)
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

        public void BeforeDiceRoll(GameState state, GameActions actions)
        {
            Console.WriteLine(id + ": Before dice roll");
            System.Threading.Thread.Sleep(1000);
        }

        public int MoveRobber(GameState state)
        {
            Console.WriteLine(id + ": Move robber");
            System.Threading.Thread.Sleep(1000);
            return state.Board.GetRobberLocation() == 8 ? 9 : 8;
        }

        public int ChoosePlayerToDrawFrom(int[] validOpponents)
        {
            Console.WriteLine(id + ": Choosing opponent to draw from");
            System.Threading.Thread.Sleep(1000);
            return validOpponents[0];
        }

        public Resource[] DiscardCards(GameState state, int toDiscard)
        {
            Console.WriteLine(id + ": Choosing cards to discard");
            System.Threading.Thread.Sleep(1000);
            return state.GetOwnResources().Take(toDiscard).ToArray();
        }

        public void PerformTurn(GameState state, GameActions actions)
        {
            Console.WriteLine(id + ": Performing main turn");
            System.Threading.Thread.Sleep(1000);
        }

        public Trade HandleTrade(Trade offer)
        {
            Console.WriteLine(id + ": Handling trade");
            return offer;
        }
    }
}
