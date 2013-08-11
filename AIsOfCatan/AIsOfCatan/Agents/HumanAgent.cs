using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class HumanAgent : IAgent
    {
        private Tuple<int, int, int> getSettlementPosition()
        {
            Console.WriteLine("Enter 3 id's (each followed by enter) for tiles describing where to place the settlement");
            return new Tuple<int, int, int>(int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine()));
        }

        private Tuple<int, int> getRoadPosition()
        {
            Console.WriteLine("Enter 2 id's (each followed by enter) for tiles describing where to place the road");
            return new Tuple<int, int>(int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine()));
        }

        private Resource selectResource()
        {
            Console.WriteLine("Select a resource:");
            Resource[] res = new Resource[] {Resource.Lumber, Resource.Brick, Resource.Grain, Resource.Ore, Resource.Wool};
            for (var i = 0; i < res.Length; i++)
            {
                Console.WriteLine((i+1)+")" + res[i]);
            }
            return res[int.Parse(Console.ReadLine()) - 1];
        }

        public void Reset(int assignedId)
        {
            Console.WriteLine("You are playing as player id #" + assignedId);
        }

        public void PlaceStart(IGameState state, IStartActions actions)
        {
            Console.WriteLine("It is your turn to place a starting settlement");
            var settlement = getSettlementPosition();
            actions.BuildSettlement(settlement.Item1,settlement.Item2,settlement.Item3);
            Console.WriteLine("Place a road connected to the settlement you just placed");
            var road = getRoadPosition();
            actions.BuildRoad(road.Item1,road.Item2);
        }

        public void BeforeDiceRoll(IGameState state, IGameActions actions)
        {
            GameState gs = (GameState) state;
            Console.WriteLine("Do you want to use a Development Card before roling the dice? Y/N");
            var r = Console.ReadLine();
            if (r.ToLower().StartsWith("y"))
            {
                Console.WriteLine("Which development card do you wish to play:");
                Console.WriteLine("0) None");
                int i = 1;
                var cards = new Dictionary<int, DevelopmentCard>();
                foreach (var c in gs.GetOwnDevelopmentCards().Where(c => c != DevelopmentCard.VictoryPoint))
                {
                    Console.WriteLine(i + ") " + c);
                    cards.Add(i,c);
                    i++;
                }
                int selection = int.Parse(Console.ReadLine());
                if (selection == 0) return;
                Console.WriteLine("You played the card " + cards[selection]);
                switch (cards[selection])
                {
                    case DevelopmentCard.Knight:
                        actions.PlayKnight();
                        break;
                    case DevelopmentCard.Monopoly:
                        Console.WriteLine("Choose the resource type to get monopoly on");
                        actions.PlayMonopoly(selectResource());
                        break;
                    case DevelopmentCard.RoadBuilding:
                        Console.WriteLine("Decide where to build the two roads");
                        var road1 = getRoadPosition();
                        var road2 = getRoadPosition();
                        actions.PlayRoadBuilding(road1.Item1, road1.Item2, road2.Item1, road2.Item2);
                        break;
                    case DevelopmentCard.YearOfPlenty:
                        Console.WriteLine("Choose which two resources you want to draw");
                        actions.PlayYearOfPlenty(selectResource(),selectResource());
                        break;
                }
            }
        }

        public int MoveRobber(IGameState state)
        {
            Console.WriteLine("Choose where to place the robber by typing in the id of a position: ");
            return int.Parse(Console.ReadLine());
        }

        public int ChoosePlayerToDrawFrom(int[] validOpponents)
        {
            Console.WriteLine("Choose which opponent to draw a card from: ");
            foreach (int o in validOpponents)
            {
                Console.Write(o + " ");
            }
            return int.Parse(Console.ReadLine());
        }

        public Resource[] DiscardCards(IGameState state, int toDiscard)
        {
            Console.WriteLine("You must discard " + toDiscard + " cards from you hand");
            var cards = new Resource[toDiscard];
            while (toDiscard-- > 0)
            {
                Console.WriteLine("Select which resource to discard");
                cards[toDiscard] = selectResource();
            }
            return cards;
        }

        public void PerformTurn(IGameState state, IGameActions actions)
        {
            throw new NotImplementedException();
        }

        public ITrade HandleTrade(ITrade offer, int proposingPlayerId)
        {
            throw new NotImplementedException();
        }
    }
}
