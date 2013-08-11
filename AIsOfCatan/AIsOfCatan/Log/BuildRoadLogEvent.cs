using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class BuildRoadLogEvent : LogEvent
    {
        private int Player { get; private set; }
        private Tuple<int, int> position;
        private Tuple<int, int> Position { get{ return new Tuple<int,int>(position.Item1, position.Item2); } }

        public BuildRoadLogEvent(int player, Tuple<int, int> position)
        {
            Player = player;
            this.position = position;
        }

        public override string ToString()
        {
            return "Player " + Player + " build a Road at [" + position.Item1 + ", " + position.Item2 + "]"; 
        }
    }
}
