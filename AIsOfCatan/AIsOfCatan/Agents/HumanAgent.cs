using System;
using System.Collections.Generic;
using System.Linq;

namespace AIsOfCatan
{
    class HumanAgent : IAgent
    {
        private int assignedId;
        private bool hasPlayedDevCard = false;

        private Tuple<int, int, int> getCityPosition()
        {
            Console.WriteLine("Enter 3 id's (each followed by enter) for tiles describing which settlement to upgrade to a city");
            return new Tuple<int, int, int>(int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine()));
        }

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

        private Resource selectResource(Resource[] resources = null)
        {
            Console.WriteLine("Select a resource:");
            var res = resources ?? new Resource[] {Resource.Lumber, Resource.Brick, Resource.Grain, Resource.Ore, Resource.Wool};
            for (var i = 0; i < res.Length; i++)
            {
                Console.WriteLine((i+1)+")" + res[i]);
            }
            return res[int.Parse(Console.ReadLine()) - 1];
        }

        private Resource selectResourceTradeBank(IBoard board, Resource[] resources = null)
        {
            Console.WriteLine("Select a resource:");
            var res = resources ?? new Resource[] { Resource.Lumber, Resource.Brick, Resource.Grain, Resource.Ore, Resource.Wool };
            for (var i = 0; i < res.Length; i++)
            {
                Console.WriteLine((i + 1) + ")" + res[i] + " - " + TradePrice(board, res[i]) + ":1");
            }
            return res[int.Parse(Console.ReadLine()) - 1];
        }

        public void Reset(int assignedId)
        {
            Console.WriteLine("You are playing as player id #" + assignedId);
            this.assignedId = assignedId;
        }

        public string GetName()
        {
            return "Human agent " + assignedId;
        }

        public string GetDescription()
        {
            return "Cogito ergo sum";
        }

        public void PlaceStart(IGameState state, IStartActions actions)
        {
            Console.WriteLine("It is your turn to place a starting settlement (#" + assignedId + ")");
            var settlement = getSettlementPosition();
            actions.BuildSettlement(settlement.Item1,settlement.Item2,settlement.Item3);
            Console.WriteLine("Place a road connected to the settlement you just placed");
            var road = getRoadPosition();
            actions.BuildRoad(road.Item1,road.Item2);
        }

        private IGameState PlayDevelopmentCard(IGameState state, IGameActions actions)
        {
            Console.WriteLine("Which development card do you wish to play:");
            Console.WriteLine("0) None");
            int i = 1;
            var cards = new Dictionary<int, DevelopmentCard>();
            foreach (var c in state.GetOwnDevelopmentCards().Where(c => c != DevelopmentCard.VictoryPoint))
            {
                Console.WriteLine(i + ") " + c);
                cards.Add(i, c);
                i++;
            }
            int selection = int.Parse(Console.ReadLine() ?? "0");
            if (selection == 0) return null;
            Console.WriteLine("You played the card " + cards[selection]);
            hasPlayedDevCard = true;
            switch (cards[selection])
            {
                case DevelopmentCard.Knight:
                    return actions.PlayKnight();
                case DevelopmentCard.Monopoly:
                    Console.WriteLine("Choose the resource type to get monopoly on");
                    return actions.PlayMonopoly(selectResource());
                case DevelopmentCard.RoadBuilding:
                    Console.WriteLine("Decide where to build the two roads");
                    var road1 = getRoadPosition();
                    var road2 = getRoadPosition();
                    return actions.PlayRoadBuilding(road1.Item1, road1.Item2, road2.Item1, road2.Item2);
                case DevelopmentCard.YearOfPlenty:
                    Console.WriteLine("Choose which two resources you want to draw");
                    return actions.PlayYearOfPlenty(selectResource(), selectResource());
            }
            return null;
        }

        public void BeforeDiceRoll(IGameState state, IGameActions actions)
        {
            hasPlayedDevCard = false;
            if (!state.GetOwnDevelopmentCards().Any(dc => dc != DevelopmentCard.VictoryPoint))
                return; //No cards to play
            Console.WriteLine("Do you want to use a Development Card before roling the dice? Y/N");
            var r = Console.ReadLine() ?? "";
            if (!r.ToLower().StartsWith("y")) return;
            PlayDevelopmentCard(state, actions);
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
                Console.Write(o + ") Agent #"+o);
            }
            Console.WriteLine();
            return int.Parse(Console.ReadLine());
        }

        public Resource[] DiscardCards(IGameState state, int toDiscard)
        {
            Console.WriteLine("You must discard " + toDiscard + " cards from you hand:");
            var hand = state.GetOwnResources().ToList();
            var cards = new Resource[toDiscard];
            while (toDiscard-- > 0)
            {
                Console.WriteLine("Select a resource to discard:");
                foreach (Resource resource in Enum.GetValues(typeof (Resource)))
                {
                    if (hand.Count(r => r == resource) == 0) continue;
                    Console.WriteLine(((int)resource) + ") " + resource + " x " + hand.Count(r => r == resource));
                }

                //Keep trying if answering wrong
                do
                {
                    cards[toDiscard] = (Resource)int.Parse(Console.ReadLine());
                } while (!hand.Remove(cards[toDiscard]));
            }
            return cards;
        }

        private bool hasRes(IGameState state, Resource res, int amount = 1)
        {
            return state.GetOwnResources().Count(r => r == res) >= amount;
        }

        public void PerformTurn(IGameState state, IGameActions actions)
        {
            Console.WriteLine("It is now your turn (#" + assignedId + ")");
            while (true)
            {
                try
                {
                    Console.Write("Resources: [");
                    foreach (Resource resource in Enum.GetValues(typeof (Resource)))
                    {
                        var count = state.GetOwnResources().Count(r => r == resource);
                        if (count == 0) continue;
                        Console.Write(resource + " x " + count + ", ");
                    }
                    Console.WriteLine("]");
                    Console.Write("Dev. cards: [");
                    foreach (DevelopmentCard devcard in Enum.GetValues(typeof(DevelopmentCard)))
                    {
                        var count = state.GetOwnDevelopmentCards().Count(r => r == devcard);
                        if (count == 0) continue;
                        Console.Write(devcard + " x " + count + ", ");
                    }
                    Console.WriteLine("]");


                    bool canBuildRoad = hasRes(state, Resource.Lumber) && hasRes(state, Resource.Brick);
                    //TODO: has any more pieces
                    bool canBuildSettlement = hasRes(state, Resource.Brick) && hasRes(state, Resource.Lumber) &&
                                              hasRes(state, Resource.Grain) && hasRes(state, Resource.Wool);
                    //TODO: has any more pieces
                    bool canBuildCity = hasRes(state, Resource.Ore, 3) && hasRes(state, Resource.Grain, 2);
                    //TODO: has any more pieces
                    bool canBuyDevCard = hasRes(state, Resource.Grain) && hasRes(state, Resource.Ore) &&
                                         hasRes(state, Resource.Wool); //TODO: any more dev cards

                    bool canTradeBank = true, canTradePlayers = true; //TODO: Implement these

                    Console.WriteLine("Choose an action:");
                                            Console.WriteLine("0) End turn");
                    if (canBuildRoad)       Console.WriteLine("1) Build road");
                    if (canBuildSettlement) Console.WriteLine("2) Build settlement");
                    if (canBuildCity)       Console.WriteLine("3) Build city");
                    if (canBuyDevCard)      Console.WriteLine("4) Buy development card");
                    if (!hasPlayedDevCard &&
                        state.GetOwnDevelopmentCards().Count(d => d != DevelopmentCard.VictoryPoint) > 0)
                                            Console.WriteLine("5) Play development card");
                    if (canTradeBank)       Console.WriteLine("6) Trade resources with the bank");
                    if (canTradePlayers)    Console.WriteLine("7) Trade resources with the other players");

                    int answer = int.Parse(Console.ReadLine() ?? "0");

                    switch (answer)
                    {
                        case 1: //road
                            var roadPos = getRoadPosition();
                            state = actions.BuildRoad(roadPos.Item1, roadPos.Item2);
                            break;
                        case 2: //settlement
                            var settlementPos = getSettlementPosition();
                            state = actions.BuildSettlement(settlementPos.Item1, settlementPos.Item2,
                                                            settlementPos.Item3);
                            break;
                        case 3: //city
                            var cityPos = getCityPosition();
                            state = actions.BuildCity(cityPos.Item1, cityPos.Item2, cityPos.Item3);
                            break;
                        case 4: //buy dev
                            state = actions.DrawDevelopmentCard();
                            break;
                        case 5: //play dev
                            state = PlayDevelopmentCard(state, actions) ?? state;
                            break;
                        case 6: //trade bank
                            Console.WriteLine("Choose which resource to give");
                            var tbGive = selectResourceTradeBank(state.Board);
                            Console.WriteLine("Choose which resource to receive");
                            var tbTake = selectResource();
                            state = actions.TradeBank(tbGive, tbTake);
                            break;
                        case 7: //trade players
                            Console.WriteLine("Which resource type do you want to give away:");
                            var tpGiveType = selectResource();
                            Console.WriteLine("How many " + tpGiveType + " do you want to give:");
                            var tpGiveAmount = int.Parse(Console.ReadLine() ?? "2");
                            Console.WriteLine("Which resource type do you want to get in return for " + tpGiveAmount + " " + tpGiveType + "? :");
                            var tpTakeType = selectResource();
                            Console.WriteLine("How many " + tpTakeType + " do you want to get:");
                            var tpTakeAmount = int.Parse(Console.ReadLine() ?? "1");

                            var give = new List<List<Resource>>(){new List<Resource>()};
                            for (int i = 0; i < tpGiveAmount; i++)
                                give[0].Add(tpGiveType);
                            var take = new List<List<Resource>>(){new List<Resource>()};
                            for (int i = 0; i < tpTakeAmount; i++)
                                take[0].Add(tpTakeType);

                            var feedback = actions.ProposeTrade(give, take);
                            Console.WriteLine("The other players responded:");
                            foreach (var f in feedback)
                            {
                                Console.Write(f.Key + ") ");
                                Console.Write(f.Value.Status + " ");
                                if (f.Value.Status == TradeStatus.Countered)
                                {
                                    Console.Write("(They give: ");

                                    Console.Write(f.Value.Give[0].Select(r => r.ToString()).Aggregate((a,b) => a + ", " + b));

                                    Console.Write(" for ");

                                    Console.Write(f.Value.Take[0].Select(r => r.ToString()).Aggregate((a, b) => a + ", " + b));

                                    Console.WriteLine(")");
                                }
                                else
                                {
                                    Console.WriteLine();
                                }
                            }
                            Console.WriteLine("Select a player to trade with by entering the id or -1 to cancel");
                            int reply = int.Parse(Console.ReadLine() ?? "-1");
                            if (reply != -1)
                            {
                                state = actions.Trade(reply);
                            }
                            break;

                        default:
                            return;
                    }
                }
                catch (AgentActionException ex)
                {
                    Console.WriteLine("Illegal action! Message: " + ex.Message);
                }
                catch(FormatException ex)
                {
                    Console.WriteLine("Illegal input! Message: " + ex.Message);
                }
            }
        }

        private int TradePrice(IBoard board, Resource giving)
        {
            var harbors = board.GetPlayersHarbors(assignedId);
            var hasSpecific = harbors.Contains((HarborType)giving);
            var hasGeneral = harbors.Contains(HarborType.ThreeForOne);

            return (hasSpecific) ? 2 : ((hasGeneral) ? 3 : 4);
        }


        public ITrade HandleTrade(ITrade offer, int proposingPlayerId)
        {
            throw new NotImplementedException();
        }
    }
}
