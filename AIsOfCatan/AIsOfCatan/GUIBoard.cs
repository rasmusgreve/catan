using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIsOfCatan
{
    class GUIBoard : TroyXnaAPI.TXAComponent
    {
        private const int TileWidth = 222;
        private static readonly float TileShift = (float) Math.Sqrt(Math.Pow(TileWidth,2)-Math.Pow((TileWidth/2),2));

        private static readonly Vector2 EAST = new Vector2(0, TileWidth);
        private static readonly Vector2 WEST = new Vector2(0, -TileWidth);
        private static readonly Vector2 S_EAST = new Vector2(TileShift, TileWidth / 2);
        private static readonly Vector2 N_EAST = new Vector2(-TileShift, TileWidth / 2);
        private static readonly Vector2 S_WEST = new Vector2(TileShift, -TileWidth / 2);
        private static readonly Vector2 N_WEST = new Vector2(-TileShift, -TileWidth / 2);

        protected GUIBoard()
        {
            
        }

        protected override void DoUpdate(GameTime time)
        {
            throw new NotImplementedException();
        }
    }
}
