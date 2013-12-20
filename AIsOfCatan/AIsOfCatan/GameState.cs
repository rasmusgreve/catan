using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.Log;

namespace AIsOfCatan
{
    public class GameState : IGameState
    {
        private Player[] players;
        private int curPlayer;
        private List<LogEvent> log;

        public GameState(IBoard board, List<DevelopmentCard> deck, int[] resourceBank, Player[] players, int curPlayer, List<LogEvent> log, int longestRoad, int largestArmy)
        {
            Board = board;
            DevelopmentCards = deck == null ? 0 : deck.Count;
            ResourceBank = resourceBank == null ? null : resourceBank.ToArray();
            this.players = players;
            this.curPlayer = curPlayer;
            this.log = log;
            if (players == null) players = new Player[0];
            AllPlayerIds = players.Select(p => p.Id).ToArray();
            LongestRoadId = longestRoad;
            LargestArmyId = largestArmy;
        }

        public IBoard Board { get; private set; }
        public int DevelopmentCards { get; private set; }
        public int[] ResourceBank { get; private set; }
        public int[] AllPlayerIds { get; private set; }
        public int LongestRoadId { get; private set; }
        public int LargestArmyId { get; private set; }

        public int GetPlayerScore(int playerId)
        {
            int result = 0;

            if (playerId == LargestArmyId) result += 2;
            if (playerId == LongestRoadId) result += 2;

            // add 2 for each city and 1 for each settlement
            Board.GetAllPieces().Where(p => p.Value.Player == playerId).ForEach(p => result += p.Value.Token == Token.City ? 2 : 1);

            return result;
        }

        public int GetRoundNumber()
        {
            return log.OfType<RollLogEvent>().Where(r => r.Player == 0).Count();
        }

        public int GetResourceCount(int playerID)
        {
            return players[playerID].Resources.Count;
        }

        public int GetDevelopmentCardCount(int playerID)
        {
            return players[playerID].DevelopmentCards.Count;
        }

        public int GetKnightCount(int playerID)
        {
            return players[playerID].PlayedKnights;
        }

        public int GetSettlementsLeft(int playerID)
        {
            return players[playerID].SettlementsLeft;
        }

        public int GetCitiesLeft(int playerID)
        {
            return players[playerID].CitiesLeft;
        }

        public int GetRoadsLeft(int playerID)
        {
            return players[playerID].RoadsLeft;
        }

        public Resource[] GetOwnResources()
        {
            return players[curPlayer].Resources.ToArray();
        }

        public DevelopmentCard[] GetOwnDevelopmentCards()
        {
            return players[curPlayer].DevelopmentCards.ToArray();
        }

        public int GetResourceBank(Resource res)
        {
            return ResourceBank[(int)res];
        }

        public List<LogEvent> GetLatestEvents(int amount)
        {
            return log.Skip(Math.Max(0, log.Count() - amount)).Take(amount).ToList();
        }

        public List<LogEvent> GetEventsSince(DateTime time)
        {
            return log.Where(e => e.TimeStamp.CompareTo(time) > 0).ToList();
        }
    }
}
