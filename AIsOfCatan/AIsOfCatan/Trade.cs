using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
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
