using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.API
{
    public class Piece
    {
        public Token Token { get; private set; }
        public int Player { get; private set; }

        public Piece(Token token, int player)
        {
            this.Token = token;
            this.Player = player;
        }
    }
}
