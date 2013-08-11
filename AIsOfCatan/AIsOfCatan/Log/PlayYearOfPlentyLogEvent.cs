using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class PlayYearOfPlentyLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public Resource First { get; private set; }
        public Resource Second { get; private set; }

        public PlayYearOfPlentyLogEvent(int player, Resource first, Resource second)
        {
            Player = player;
            First = first;
            Second = second;
        }

        public override string ToString()
        {
            return "Player " + Player + " plays Year of Plenty to gain " + First + " and " + Second;
        }
    }
}
