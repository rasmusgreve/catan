using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class PlayKnightLogEvent : LogEvent
    {
        public int Player { get; private set; }

        public PlayKnightLogEvent(int player)
        {
            Player = player;
        }

        public override string ToString()
        {
            return "Player " + Player + " plays a Knight";
        }
    }
}
