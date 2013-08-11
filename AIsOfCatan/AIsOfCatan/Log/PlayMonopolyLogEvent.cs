using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class PlayMonopolyLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public Resource Resource { get; private set; }
        public int Gained { get; private set; }

        public PlayMonopolyLogEvent(int player, Resource res, int cardsGained)
        {
            Player = player;
            Resource = res;
            Gained = cardsGained;
        }

        public override string ToString()
        {
            return "Player " + Player + " plays Monopoly on " + Resource + " and gets " + Gained + " cards";
        }
    }
}
