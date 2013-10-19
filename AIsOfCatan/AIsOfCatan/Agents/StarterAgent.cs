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

            var spos = FindBestIntersection(state.Board.GetAllIntersections()
                .Where(i => state.Board.GetPiece(i) == null &&
                    state.Board.HasNoNeighbors(i)), state.Board);
            
            actions.BuildSettlement(spos);
            actions.BuildRoad(FindBestRoad(spos, state.Board));
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

        public int ChoosePlayerToDrawFrom(IGameState state, int[] validOpponents)
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
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildCity(FindBestIntersection(pos,state.Board));
                    }
                }
                //Build settlement
                if (resources.Contains(Resource.Grain) && resources.Contains(Resource.Wool) && resources.Contains(Resource.Lumber) && resources.Contains(Resource.Brick))
                {
                    var pos = state.Board.GetPossibleSettlements(id);
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildSettlement(FindBestIntersection(pos,state.Board));
                    }
                }
                //Build road
                if (state.GetRoadsLeft(id) > 0 && resources.Contains(Resource.Lumber) && resources.Contains(Resource.Brick))
                {
                    var pos = state.Board.GetPossibleRoads(id);
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildRoad(FindBestRoad(pos, state.Board));
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
            }
        }

        public ITrade HandleTrade(IGameState state, ITrade offer, int proposingPlayerId)
        {
            return offer.Respond(offer.Give[0],offer.Take[0]);
        }

        private int GetScore(Intersection inter, IBoard board)
        {
            return Chances(board.GetTile(inter.FirstTile).Value) +
                    Chances(board.GetTile(inter.SecondTile).Value) +
                    Chances(board.GetTile(inter.ThirdTile).Value);
        }

        private double GetScore(Edge e, IBoard board)
        {
            Intersection[] adjacentFree = board.GetAdjacentIntersections(e).Where(i => board.HasNoNeighbors(i)).ToArray();
            if (adjacentFree.Length > 0)
            {
                return adjacentFree.Average(i => GetScore(i, board));
            }
            return 0.0;
        }

        private Intersection FindBestIntersection(IEnumerable<Intersection> enumerable, IBoard board)
        {
            return enumerable.OrderBy(i => GetScore(i,board)).Last();
        }

        private IEnumerable<Intersection> GetEnds(Intersection inter, IBoard board)
        {
            return board.GetAdjacentEdges(inter).SelectMany(e => board.GetAdjacentIntersections(e)).Where(i => !i.Equals(inter));
        }

        private Edge GetEdgeBetween(Intersection first, Intersection second)
        {
            int[] result = first.ToArray().Where(i => second.ToArray().Contains(i)).ToArray();
            if (result.Length < 2) return null;
            return new Edge(result[0], result[1]);
        }

        private Edge FindBestRoad(Intersection from, IBoard board)
        {
            // find the best neighbor intersection and takes the edge to that
            return GetEdgeBetween(FindBestIntersection(GetEnds(from, board),board),from);
        }

        private Edge FindBestRoad(IEnumerable<Edge> edges, IBoard board)
        {
            // best edge ordered by highest average of possible values on edges ends
            return edges.OrderBy(e => GetScore(e,board)).Last();
        }
    }
}
