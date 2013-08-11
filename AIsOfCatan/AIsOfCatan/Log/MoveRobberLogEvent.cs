using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class MoveRobberLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public int Tile { get; private set; }

        public MoveRobberLogEvent(int player, int tile)
        {
            Player = player;
            Tile = tile;
        }

        public override string ToString()
        {
            return "Player " + Player + " moves the Robber to tile " + Tile;
        }
    }
}
