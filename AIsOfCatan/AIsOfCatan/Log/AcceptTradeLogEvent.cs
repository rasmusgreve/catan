using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class AcceptTradeLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public int OtherPlayer { get; private set; }

        private List<Resource> give;
        public List<Resource> Give { get { return give.ToList(); } }
        private List<Resource> take;
        public List<Resource> Take { get { return take.ToList(); } }

        public AcceptTradeLogEvent(int player, int otherplayer, List<Resource> give, List<Resource> take){
            Player = player;
            OtherPlayer = otherplayer;
            this.give = give;
            this.take = take;
        }

        public override string ToString()
        {
            return "Player " + Player + " accepts to trade " + CounterTradeLogEvent.ListToString(give) + " for " + CounterTradeLogEvent.ListToString(take) + " with " + OtherPlayer;
        }
    }
}
