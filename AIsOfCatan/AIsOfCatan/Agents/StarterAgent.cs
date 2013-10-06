using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return "Simple Agent";
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
                .Where(i => state.Board.GetPiece(i.Item1,i.Item2,i.Item3) == null &&
                    state.Board.HasNoNeighbors(i.Item1,i.Item2,i.Item3))
                .OrderBy(i => Chances(state.Board.GetTile(i.Item1).Value) +
                    Chances(state.Board.GetTile(i.Item2).Value) +
                    Chances(state.Board.GetTile(i.Item3).Value)).Last();
            
            actions.BuildSettlement(spos.Item1, spos.Item2, spos.Item3);
            if (state.Board.CanBuildRoad(spos.Item1,spos.Item2))
                actions.BuildRoad(spos.Item1, spos.Item2);
            else if (state.Board.CanBuildRoad(spos.Item1, spos.Item3))
                actions.BuildRoad(spos.Item1, spos.Item3);
            else
                actions.BuildRoad(spos.Item2, spos.Item3);
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
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildCity(pos[0].Item1, pos[0].Item2, pos[0].Item3);
                    }
                }
                //Build settlement
                if (resources.Contains(Resource.Grain) && resources.Contains(Resource.Wool) && resources.Contains(Resource.Lumber) && resources.Contains(Resource.Brick))
                {
                    var pos = state.Board.GetPossibleSettlements(id);
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildSettlement(pos[0].Item1, pos[0].Item2, pos[0].Item3);
                    }
                }
                //Build road
                if (resources.Contains(Resource.Lumber) && resources.Contains(Resource.Brick))
                {
                    var pos = state.Board.GetPossibleRoads(id);
                    if (pos.Length > 0)
                    {
                        changed = true;
                        state = actions.BuildRoad(pos[0].Item1, pos[0].Item2);
                    }
                }
            }
        }

        public ITrade HandleTrade(ITrade offer, int proposingPlayerId)
        {
            return offer.Respond(offer.Give[0],offer.Take[0]);
        }
    }
}
