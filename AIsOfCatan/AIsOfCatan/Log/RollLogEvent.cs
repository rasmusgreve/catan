using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class RollLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public int Roll { get; private set; }

        public RollLogEvent(int player, int roll)
        {
            Player = player;
            Roll = roll;
        }

        public override string ToString()
        {
            return "Player " + Player + " rolled " + Roll;
        }
    }
}
