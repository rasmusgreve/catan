using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class StealCardLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public int TargetPlayer { get; private set; }

        public StealCardLogEvent(int player, int targetPlayer)
        {
            Player = player;
            TargetPlayer = targetPlayer;
        }

        public override string ToString()
        {
            return "Player " + Player + " steals a card from Player " + TargetPlayer;
        }
    }
}
