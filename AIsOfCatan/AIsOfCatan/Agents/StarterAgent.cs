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
            return "Starter Agent #"+id;
        }

        public string GetDescription()
        {
            return "";
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
            // We don't play knights before dice roll
        }

        public int MoveRobber(IGameState state)
        {
            if (!silent)
                Console.WriteLine(id + ": Move robber");

            // get locations not connected to ours
            List<int> options = new List<int>(45);
            for (int i = 0; i < 45; i++) options.Add(i);

            int choice = options.Where(t => state.Board.GetTile(t).Terrain != Terrain.Water
                        && state.Board.GetTile(t).Terrain != Terrain.Desert
                        && t != state.Board.GetRobberLocation()) // legal
                .OrderBy(t => OwnTileValue(t, state.Board) - OpponentTileValue(t, state.Board)).First(); // own value - opponent value, lowest is best for us

            return choice;
        }

        public int ChoosePlayerToDrawFrom(IGameState state, int[] validOpponents)
        {
            if (!silent)
                Console.WriteLine(id + ": Choosing opponent to draw from");
            return validOpponents.OrderBy(o => state.GetPlayerScore(o)).First(); // steal from highest points
        }

        public Resource[] DiscardCards(IGameState state, int toDiscard)
        {
            if (!silent)
                Console.WriteLine(id + ": Choosing cards to discard");

            List<Resource> chosenDiscard = new List<Resource>(toDiscard);
            List<Resource> hand = state.GetOwnResources().ToList();

            while (chosenDiscard.Count < toDiscard)
            {
                // pick one of the type we have most of
                Resource pick = hand.OrderByDescending(r => hand.Count(c => c == r)).First();
                chosenDiscard.Add(pick);
                hand.Remove(pick);
            }

            return chosenDiscard.ToArray();
        }

        public void PerformTurn(IGameState state, IGameActions actions)
        {
            if (!silent)
                Console.WriteLine(id + ": Performing main turn");

            for (bool changed = true; changed; )
            {
                changed = false;
                var resources = state.GetOwnResources();

                //Build city
                if (state.GetCitiesLeft(id) > 0 && resources.Count(r => r == Resource.Grain) >= 2 && resources.Count(r => r == Resource.Ore) >= 3)
                {
                    var pos = state.Board.GetPossibleCities(id);
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildCity(FindBestIntersection(pos,state.Board));
                    }
                }
                //Build settlement
                if (!changed && state.GetSettlementsLeft(id) > 0 && resources.Contains(Resource.Grain) && resources.Contains(Resource.Wool) && resources.Contains(Resource.Lumber) && resources.Contains(Resource.Brick))
                {
                    var pos = state.Board.GetPossibleSettlements(id);
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildSettlement(FindBestIntersection(pos,state.Board));
                    }
                }
                //Build road
                if (!changed && state.GetRoadsLeft(id) > 0 && resources.Contains(Resource.Lumber) && resources.Contains(Resource.Brick))
                {
                    var pos = state.Board.GetPossibleRoads(id);
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildRoad(FindBestRoad(pos, state.Board));
                    }
                }

                //Trade players

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
            // accept if convert extras to needed and opponent < 7 points
            List<Resource> extras = new List<Resource>();
            foreach (Resource r in Enum.GetValues(typeof(Resource)))
            {
                int extra = state.GetOwnResources().Count(res => res == r) - 1;
                for (int i = 0; i < extra; i++) extras.Add(r);
            }

            // good offer?
            var valid = offer.Give.Where(o => o.All(r => o.Count(cur => cur == r) <= extras.Count(e => e == r)));

            if (valid.Count() == 0) return offer.Decline();

            // take the one with least cards to give, and then by most duplicates
            List<Resource> bestGive = offer.Give.OrderBy(o => o.Count)
                .ThenByDescending(o => state.GetOwnResources().Sum(r => state.GetOwnResources().Count(res => res == r)))
                .First();

            // find best "take" (cards we get) kind of the opposite of above
            List<Resource> bestTake = offer.Take.OrderBy(o => o.Count)
                .ThenBy(o => state.GetOwnResources().Sum(r => state.GetOwnResources().Count(res => res == r)))
                .First();

            return offer.Respond(bestGive, bestTake);
        }

        // PRIVATE HELPERS //

        private int Chances(int roll)
        {
            return 6 - Math.Abs(7 - roll);
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

        private int OpponentTileValue(int tile, IBoard board)
        {
            int value = 0;

            foreach (Intersection i in board.GetAdjacentIntersections(tile))
            {
                Piece curPiece = board.GetPiece(i);
                if (curPiece != null && curPiece.Player != id)
                {
                    value += curPiece.Token == Token.City ? 2 : 1;
                }
            }

            return Chances(board.GetTile(tile).Value) * value;
        }

        private int OwnTileValue(int tile, IBoard board)
        {
            int value = 0;

            foreach(Intersection i in board.GetAdjacentIntersections(tile)){
                Piece curPiece = board.GetPiece(i);
                if (curPiece != null && curPiece.Player == id)
                {
                    value += curPiece.Token == Token.City ? 2 : 1;
                }
            }

            return Chances(board.GetTile(tile).Value) * value;
        }
    }
}
