using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    //                     19 of each in bank
    public enum Resources {Brick, Lumber, Wool, Grain, Ore}; // names from wikipedia
    //                   3      4       4        4       3          1       X
    public enum Terrain {Hills, Forest, Pasture, Fields, Mountains, Desert, Water} 
    //                            14      5             2             2             2
    public enum DevelopmentCards {Knight, VictoryPoint, RoadBuilding, YearOfPlenty, Monopoly}
    public enum TradeStatus {Declined, Accepted, Countered};

    class Trade
    {
        private TradeStatus status = TradeStatus.Declined;

        // For a Wildcard put several Resources in the same inner list.
        public List<List<Resources>> Give { get; private set; }
        public List<List<Resources>> Want { get; private set; }

        public Trade(List<List<Resources>> give, List<List<Resources>> want){
            this.Give = give;
            this.Want = want;
        }

        public Trade Reverse()
        {
            return new Trade(Want,Give);
        }

        public void Accept()
        {
            status = TradeStatus.Accepted;
        }

        public void CounterOffer()
        {
            status = TradeStatus.Countered;
        }

        public TradeStatus GetStatus()
        {
            return status;
        }
    }
}
