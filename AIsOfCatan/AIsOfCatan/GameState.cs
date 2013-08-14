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

        public GameState(Board board, List<DevelopmentCard> deck, int[] resourceBank, Player[] players, int curPlayer, List<LogEvent> log)
        {
            Board = board;
            DevelopmentCards = deck == null ? 0 : deck.Count;
            ResourceBank = resourceBank == null ? null : resourceBank.ToArray();
            this.players = players;
            this.curPlayer = curPlayer;
            this.log = log;
            if (players == null) players = new Player[0];
            AllPlayerIds = players.Select(p => p.Id).ToArray();
        }

        public Board Board { get; private set; }
        public int DevelopmentCards { get; private set; }
        public int[] ResourceBank { get; private set; }
        public int[] AllPlayerIds { get; private set; }

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
