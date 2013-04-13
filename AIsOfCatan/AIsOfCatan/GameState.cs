using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
        //                     19 of each in bank
    public enum Resource {Brick, Lumber, Wool, Grain, Ore}; // names from wikipedia
    //                   3      4       4        4       3          1       X
    public enum Terrain {Hills, Forest, Pasture, Fields, Mountains, Desert, Water} 
    //                            14      5             2             2             2
    public enum DevelopmentCard {Knight, VictoryPoint, RoadBuilding, YearOfPlenty, Monopoly}

    public enum Token {Road, Settlement, City};

    //----------------------------------------------------------------------------------------//

    public class GameState
    {
        public Board Board { get; private set; }
        public int DevelopmentCards { get; private set; }
        public int[] ResourceBank { get; private set; }

        private Object[] players;
        private int curPlayer;

        public GameState(Board board, List<DevelopmentCard> deck, int[] resourceBank, Object[] players, int curPlayer)
        {
            Board = board;
            DevelopmentCards = deck == null ? 0 : deck.Count;
            ResourceBank = resourceBank.ToArray();
            this.players = players;
            this.curPlayer = curPlayer;
        }
    }
}
