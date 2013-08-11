using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class BuyDevLogEvent : LogEvent
    {
        public int Player { get; private set; }

        public BuyDevLogEvent(int player)
        {
            Player = player;
        }

        public override string ToString()
        {
            return "Player " + Player + " bought a Development Card";
        }
    }
}
