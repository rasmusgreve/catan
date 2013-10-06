using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.API;

namespace AIsOfCatan
{
    class StarterAgent : IAgent
    {
        private const bool silent = true;

        private bool hasDevcardToPlay = false;
        private DevelopmentCard nextToPlay;
        private int id;
        public void Reset(int assignedId)
        {
            id = assignedId;
            if (!silent)
                Console.WriteLine("IAgent reset with id " + id);
        }

        public string GetName()
        {
            return "Starter Agent";
        }

        public string GetDescription()
        {
            return "";
        }

        private int Chances(int roll)
        {
            return 6 - Math.Abs(7 - roll);
        }

        public void PlaceStart(IGameState state, IStartActions actions)
        {
            if (!silent)
                Console.WriteLine(id + ": Place starts");

            var spos = state.Board.GetAllIntersections()
                .Where(i => state.Board.GetPiece(i) == null &&
                    state.Board.HasNoNeighbors(i))
                .OrderBy(i => Chances(state.Board.GetTile(i.FirstTile).Value) +
                    Chances(state.Board.GetTile(i.SecondTile).Value) +
                    Chances(state.Board.GetTile(i.ThirdTile).Value)).Last();
            
            actions.BuildSettlement(spos);
            if (state.Board.CanBuildRoad(new Edge(spos.FirstTile,spos.SecondTile)))
                actions.BuildRoad(new Edge(spos.FirstTile,spos.SecondTile));
            else if (state.Board.CanBuildRoad(new Edge(spos.FirstTile,spos.ThirdTile)))
                actions.BuildRoad(new Edge(spos.FirstTile,spos.ThirdTile));
            else
                actions.BuildRoad(new Edge(spos.SecondTile,spos.ThirdTile));
        }

        public void BeforeDiceRoll(IGameState state, IGameActions actions)
        {
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

            for (bool changed = true; changed; )
            {
                changed = false;
                var resources = ((GameState)state).GetOwnResources();

                //Build city
                if (resources.Count(r => r == Resource.Grain) >= 2 && resources.Count(r => r == Resource.Ore) >= 3)
                {
                    var pos = state.Board.GetPossibleCities(id);
                    pos.OrderBy(e => Guid.NewGuid());
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildCity(pos[0]);
                    }
                }
                //Build settlement
                if (resources.Contains(Resource.Grain) && resources.Contains(Resource.Wool) && resources.Contains(Resource.Lumber) && resources.Contains(Resource.Brick))
                {
                    var pos = state.Board.GetPossibleSettlements(id);
                    pos.OrderBy(e => Guid.NewGuid());
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildSettlement(pos[0]);
                    }
                }
                //Build road
                if (resources.Contains(Resource.Lumber) && resources.Contains(Resource.Brick))
                {
                    var pos = state.Board.GetPossibleRoads(id);
                    pos.OrderBy(e => Guid.NewGuid());
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildRoad(pos[0]);
                    }
                }
                //Trade bank
                foreach (Resource give in Enum.GetValues(typeof(Resource)))
                {
                    if (changed) break;
                    if (resources.Count(r => r == give) > 4)
                    {
                        foreach (Resource take in Enum.GetValues(typeof(Resource)))
                        {
                            if (changed) break;
                            if (resources.Count(r => r == take) == 0)
                            {
                                state = actions.TradeBank(give, take);
                                changed = true;
                            }
                        }
                    }
                }
                //TODO: other stuff

            }
        }

        public ITrade HandleTrade(ITrade offer, int proposingPlayerId)
        {
            return offer.Respond(offer.Give[0],offer.Take[0]);
        }
    }
}
