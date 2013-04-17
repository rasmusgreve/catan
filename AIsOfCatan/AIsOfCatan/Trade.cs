using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public enum TradeStatus {Declined, Accepted, Countered};

    public class Trade : ITrade
    {
        // For a Wildcard put several Resource in the same inner list.
        public List<List<Resource>> Give { get; private set; }
        public List<List<Resource>> Want { get; private set; }
        public TradeStatus Status { get; private set; }

        public Trade(List<List<Resource>> give, List<List<Resource>> want){
            this.Give = give;
            this.Want = want;
            Status = TradeStatus.Declined;
        }

        public Trade Reverse()
        {
            return new Trade(DeepClone(Want),DeepClone(Give)) {Status = Status};
        }

        public void Accept()
        {
            Status = TradeStatus.Accepted;
        }

        public void CounterOffer()
        {
            Status = TradeStatus.Countered;
        }

        private List<List<Resource>> DeepClone(List<List<Resource>> list)
        {
            var result = new List<List<Resource>>(list.Count);
            result.AddRange(list.Select(l => new List<Resource>(l)));
            return result;
        }
    }
}
