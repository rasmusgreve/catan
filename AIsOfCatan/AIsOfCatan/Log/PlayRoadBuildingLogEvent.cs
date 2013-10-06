using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.API;

namespace AIsOfCatan.Log
{
    public class PlayRoadBuildingLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public Edge First { get; private set; }
        public Edge Second { get; private set; }

        public PlayRoadBuildingLogEvent(int player, Edge first, Edge second = null)
        {
            Player = player;
            this.First = first;
            this.Second = second;
        }

        public override string ToString()
        {
            return "Player " + Player + " plays Road Building and builds on " + First + " and " + Second;
        }
    }
}
