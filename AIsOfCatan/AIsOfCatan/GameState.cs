using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.Log;

namespace AIsOfCatan
{
        //                     19 of each in bank
    public enum Resource {Brick, Lumber, Wool, Grain, Ore}; // names from wikipedia
    //                   3      4       4        4       3          1       X
    public enum Terrain {Hills, Forest, Pasture, Fields, Mountains, Desert, Water} 
    //                            14      5             2             2             2
    public enum DevelopmentCard {Knight, VictoryPoint, RoadBuilding, YearOfPlenty, Monopoly}

    public enum Token {Settlement, City};

    public enum HarborType { Brick, Lumber, Wool, Grain, Ore, ThreeForOne };

    //----------------------------------------------------------------------------------------//

    public class GameState : IGameState
    {
        public Board Board { get; private set; }
        public int DevelopmentCards { get; private set; }
        public int[] ResourceBank { get; private set; }

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
