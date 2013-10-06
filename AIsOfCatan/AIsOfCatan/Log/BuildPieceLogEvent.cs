using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.API;

namespace AIsOfCatan.Log
{
    public class BuildPieceLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public Token Piece { get; private set; }

        public Intersection Position { get; private set; }

        public BuildPieceLogEvent(int player, Token piece, Intersection position)
        {
            Player = player;
            Piece = piece;
            this.Position = position;
        }

        public override string ToString()
        {
            return "Player " + Player + " build " + Piece.ToString() + " at "+Position;
        }
    }
}
