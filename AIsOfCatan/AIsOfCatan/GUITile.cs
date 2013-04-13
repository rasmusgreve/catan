using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan
{
    class GUITile : TXADrawableComponent
    {
        public const int TILE_WIDTH = 222;
        public const int TILE_HEIGHT = 255;
        private static readonly float TileShift = (float)Math.Sqrt(Math.Pow(TILE_WIDTH, 2) - Math.Pow((TILE_WIDTH / 2), 2));

        private static readonly Vector2 EAST = new Vector2(0, TILE_WIDTH);
        private static readonly Vector2 WEST = new Vector2(0, -TILE_WIDTH);
        private static readonly Vector2 S_EAST = new Vector2(TileShift, TILE_WIDTH / 2);
        private static readonly Vector2 N_EAST = new Vector2(-TileShift, TILE_WIDTH / 2);
        private static readonly Vector2 S_WEST = new Vector2(TileShift, -TILE_WIDTH / 2);
        private static readonly Vector2 N_WEST = new Vector2(-TileShift, -TILE_WIDTH / 2);


        private GameState.Tile Tile { get; set; }

        public GUITile(int x, int y, GameState.Tile tile) : 
            base(
                new Vector2((float) ((x+0.5+(y % 2 == 0 ? 0 : 0.5))*TILE_WIDTH), (float) ((0.66+y)*TileShift)),
                GetTexture(tile.Terrain)
            )

        {
            Tile = tile;
        }

        private static Texture2D GetTexture(Terrain ter)
        {
            return TXAGame.TEXTURES["T_" + ter];
        }

        protected override void DoUpdate(GameTime time)
        {
            //throw new NotImplementedException();
        }

        protected override void Draw(SpriteBatch batch)
        {
            Debug.WriteLine(string.Format("drawing at {0}, visible {1}", Position, Visible));

            base.Draw(batch);
        }

        //internal void Draw(SpriteBatch batch)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
