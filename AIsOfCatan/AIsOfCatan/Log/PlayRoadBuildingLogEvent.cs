using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class PlayRoadBuildingLogEvent : LogEvent
    {
        public int Player { get; private set; }
        private Tuple<int, int> first, second;
        public Tuple<int, int> First { get { return new Tuple<int, int>(first.Item1, first.Item2); } }
        public Tuple<int, int> Second { get { return new Tuple<int, int>(second.Item1, second.Item2); } }

        public PlayRoadBuildingLogEvent(int player, Tuple<int, int> first, Tuple<int, int> second = null)
        {
            Player = player;
            this.first = first;
            this.second = second;
        }

        public override string ToString()
        {
            if (second == null)
                return "Player " + Player + " plays Road Building and builds on [" + First.Item1 + ", " + First.Item2 + "] (out of roads)";
            else
                return "Player " + Player + " plays Road Building and builds on [" + First.Item1 + ", " + First.Item2 + "] and [" + Second.Item1 + ", " + Second.Item2 + "]";
        }
    }
}
