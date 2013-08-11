using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class BuildPieceLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public Token Piece { get; private set; }

        private Tuple<int, int, int> position;
        private Tuple<int, int, int> Position { get { return new Tuple<int, int, int>(position.Item1, position.Item2, position.Item3); } }

        public BuildPieceLogEvent(int player, Token piece, Tuple<int,int,int> position)
        {
            Player = player;
            Piece = piece;
            this.position = position;
        }

        public override string ToString()
        {
            return "Player " + Player + " build " + Piece.ToString() + " at [" + position.Item1 + ", " + position.Item2 + ", " + position.Item3 + "]";
        }
    }
}
