using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.API;

namespace AIsOfCatan.Log
{
    public class BuildRoadLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public Edge Position { get; private set; }

        public BuildRoadLogEvent(int player, Edge position)
        {
            Player = player;
            this.Position = position;
        }

        public override string ToString()
        {
            return "Player " + Player + " build a Road at " + Position;
        }
    }
}
