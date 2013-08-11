using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class ProposeTradeLogEvent : LogEvent
    {
        private int Player { get; private set; }
        private List<List<Resource>> give;
        private List<List<Resource>> Give { get { return DeepClone(give); } }
        private List<List<Resource>> take;
        private List<List<Resource>> Take { get { return DeepClone(take); } }

        public ProposeTradeLogEvent(int player, List<List<Resource>> give, List<List<Resource>> take)
        {
            Player = player;
            this.give = give;
            this.take = take;
        }

        private static List<List<Resource>> DeepClone(List<List<Resource>> list)
        {
            var result = new List<List<Resource>>(list.Count);
            result.AddRange(list.Select(l => new List<Resource>(l)));
            return result;
        }

        private static string ListToString(List<List<Resource>> list)
        {
            return "";
        }

        public override string ToString()
        {
            return "Player " + Player + " proposes to trade " + ListToString(give) + " for " + ListToString(take);
        }
    }
}
