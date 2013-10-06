using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class CounterTradeLogEvent : LogEvent
    {
        public int Player { get; private set; }
        private List<Resource> give;
        public List<Resource> Give { get { return give.ToList(); } }
        private List<Resource> take;
        public List<Resource> Take { get { return take.ToList(); } }

        public CounterTradeLogEvent(int player, List<Resource> give, List<Resource> take)
        {
            Player = player;
            this.give = give;
            this.take = take;
        }

        public override string ToString()
        {
            return "Player " + Player + " suggests the trade to be " + ListToString(give) + " for " + ListToString(take);
        }
    }
}
