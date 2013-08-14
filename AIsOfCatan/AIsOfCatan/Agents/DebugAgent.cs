using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class DebugAgent : IAgent
    {
        private const bool silent = true;

        private bool hasDevcardToPlay = false;
        private DevelopmentCard nextToPlay;
        private int id;
        private int[] start1 = new[] { 21, 27, 28 };
        private bool firstStartPlaced = false;
        private int[] start2 = new[] { 9, 15, 16 };
        private int[] tooClose = new[] { 21, 22, 28 };
        private int[] farRoad = new[] { 34, 35 };
        public void Reset(int assignedId)
        {
            id = assignedId;
            if (!silent)
                Console.WriteLine("IAgent reset with id " + id);
        }

        public string GetName()
        {
            return "Debug Agent";
        }

        public string GetDescription()
        {
            return "";
        }

        public void PlaceStart(IGameState state, IStartActions actions)
        {
            if (!silent)
                Console.WriteLine(id + ": Place starts");
            if (!firstStartPlaced)
            {
                firstStartPlaced = true;
                actions.BuildSettlement(start1[0], start1[1], start1[2]);
                if (!silent)
                    Console.WriteLine(id + ": First settlement built succesfully");
                actions.BuildRoad(start1[0], start1[1]);
                if (!silent)
                    Console.WriteLine(id + ": First road built succesfully");
            }
            else
            {
                try
                {
                    actions.BuildSettlement(start1[0], start1[1], start1[2]);
                    if (!silent)
                        Console.WriteLine(id + ": Controller allowed a building on top of another");
                }
                catch (IllegalBuildPositionException e)
                {
                    if (!silent)
                        Console.WriteLine(id + ": Controller threw exception as expected: " + e.Message);
                }
                try
                {
                    actions.BuildSettlement(tooClose[0], tooClose[1], tooClose[2]);
                    if (!silent)
                        Console.WriteLine(id + ": Controller allowed a building too close");
                }
                catch (IllegalBuildPositionException e)
                {
                    if (!silent)
                        Console.WriteLine(id + ": Controller threw exception as expected: " + e.Message);
                }
                actions.BuildSettlement(start2[0], start2[1], start2[2]);
                Console.WriteLine(id + ": Second settlement built succesfully");
                try
                {
                    actions.BuildRoad(farRoad[0], farRoad[1]);
                    if (!silent)
                        Console.WriteLine(id + ": Controller allowed a building disconnected road");
                }
                catch (IllegalBuildPositionException e)
                {
                    if (!silent)
                        Console.WriteLine(id + ": Controller threw exception as expected: " + e.Message);
                }
                actions.BuildRoad(start2[0], start2[1]);
                if (!silent)
                    Console.WriteLine(id + ": Second road built succesfully");
            }
        }

        public void BeforeDiceRoll(IGameState state, IGameActions actions)
        {
            if (!silent)
                Console.WriteLine(id + ": Before dice roll");
        }

        public int MoveRobber(IGameState state)
        {
            if (!silent)
                Console.WriteLine(id + ": Move robber");
            return ((GameState)state).Board.GetRobberLocation() == 8 ? 9 : 8;
        }

        public int ChoosePlayerToDrawFrom(int[] validOpponents)
        {
            if (!silent)
                Console.WriteLine(id + ": Choosing opponent to draw from");
            return validOpponents[0];
        }

        public Resource[] DiscardCards(IGameState state, int toDiscard)
        {
            if (!silent)
                Console.WriteLine(id + ": Choosing cards to discard");
            return ((GameState)state).GetOwnResources().Take(toDiscard).ToArray();
        }

        public void PerformTurn(IGameState state, IGameActions actions)
        {
            if (!silent)
                Console.WriteLine(id + ": Performing main turn");
            var resources = ((GameState)state).GetOwnResources();
            int[] resCount = new int[5];
            foreach (var r in resources)
                resCount[(int)r]++;
            if (!silent)
                Console.Write(id + ": Resources: ( ");
            foreach (var i in resCount)
                if (!silent)
                    Console.Write(i + " ");
            if (!silent)
                Console.WriteLine(")");

            if (hasDevcardToPlay)
            {
                hasDevcardToPlay = false;
                if (!silent)
                    Console.WriteLine("-----------");
                if (!silent)
                    Console.WriteLine("Has a dev card to play: " + nextToPlay);
                switch (nextToPlay)
                {
                    case DevelopmentCard.Knight:
                        if (!silent)
                            Console.WriteLine("Play knight");
                        state = ((MainActions) actions).PlayKnight();
                        break;

                    case DevelopmentCard.Monopoly:
                        if (!silent)
                            Console.WriteLine("Play monopoly");
                        state = ((MainActions)actions).PlayMonopoly(Resource.Ore);
                        break;

                    case DevelopmentCard.RoadBuilding:
                        if (!silent)
                            Console.WriteLine("Play road building");
                        state = ((MainActions)actions).PlayRoadBuilding(27,28, 28, 34);
                        break;

                    case DevelopmentCard.YearOfPlenty:
                        if (!silent)
                            Console.WriteLine("Play year of plenty");
                        state = ((MainActions)actions).PlayYearOfPlenty(Resource.Grain, Resource.Wool);
                        break;
                }
                if (!silent)
                    Console.WriteLine("-----------");
            }
            resources = ((GameState)state).GetOwnResources();

            if (resources.Contains(Resource.Grain) && resources.Contains(Resource.Ore) &&
                resources.Contains(Resource.Wool))
            {
                var prevCards = ((GameState) state).GetOwnDevelopmentCards();
                state = actions.DrawDevelopmentCard();
                if (!silent)
                    Console.WriteLine("Drawn developmentcard successfully");
                hasDevcardToPlay = true;
                var cards = ((GameState) state).GetOwnDevelopmentCards().ToList();
                foreach (var developmentCard in prevCards)
                {
                    cards.Remove(developmentCard);
                }
                nextToPlay = cards.ToArray()[0];
            }
            else
            {
                try
                {
                    actions.DrawDevelopmentCard();
                    if (!silent)
                        Console.WriteLine("WARNING! Was able to buy a development card with not enough resources");
                }
                catch (InsufficientResourcesException e)
                {
                    if (!silent)
                        Console.WriteLine("exceptions was thrown as excpected");
                }
            }


        }

        public ITrade HandleTrade(ITrade offer, int proposingPlayerId)
        {
            if (!silent)
                Console.WriteLine(id + ": Handling trade");
            return offer;
        }
    }
}
