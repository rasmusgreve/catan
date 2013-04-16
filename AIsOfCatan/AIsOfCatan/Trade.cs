using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public enum TradeStatus {Declined, Accepted, Countered};

    public class Trade
    {
        private TradeStatus status = TradeStatus.Declined;

        // For a Wildcard put several Resource in the same inner list.
        public List<List<Resource>> Give { get; private set; }
        public List<List<Resource>> Want { get; private set; }

        public Trade(List<List<Resource>> give, List<List<Resource>> want){
            this.Give = give;
            this.Want = want;
        }

        public Trade Reverse()
        {
            return new Trade(DeepClone(Want),DeepClone(Give)) {status = status};
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

        private List<List<Resource>> DeepClone(List<List<Resource>> list)
        {
            var result = new List<List<Resource>>(list.Count);
            foreach (List<Resource> l in list) result.Add(new List<Resource>(l));
            return result;
        }
    }
}
