using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan
{
    class GUITile : TXADrawableComponent
    {
        public static int TileWidth() { return 222; }
        public static int TileHeight() { return 255; }
        private static float TileShift() { return (float)Math.Sqrt(Math.Pow(TileWidth(), 2) - Math.Pow((TileWidth() / 2), 2)); }

        //private static readonly Vector2 EAST = new Vector2(0, TILE_WIDTH);
        //private static readonly Vector2 WEST = new Vector2(0, -TILE_WIDTH);
        //private static readonly Vector2 S_EAST = new Vector2(TileShift, TILE_WIDTH / 2);
        //private static readonly Vector2 N_EAST = new Vector2(-TileShift, TILE_WIDTH / 2);
        //private static readonly Vector2 S_WEST = new Vector2(TileShift, -TILE_WIDTH / 2);
        //private static readonly Vector2 N_WEST = new Vector2(-TileShift, -TILE_WIDTH / 2);

        private bool Omitted { get; set; }

        private Vector2 numberPos;
        private Vector2 textPos;
        private readonly Color valueColour;

        private Board.Tile Tile { get; set; }

        public GUITile(int x, int y, Board.Tile tile, bool omit) : 
            base(
                new Vector2((float) ((x+0.5+(y % 2 == 0 ? 0.5 : 0))*TileWidth()), (float) ((0.66+y)*TileShift())),
                GetTexture(tile.Terrain)
            )
        {
            Tile = tile;
            Omitted = omit;
            NumAreaAndTextPos();
            valueColour = (Tile.Value == 6 || Tile.Value == 8 ? Color.Red : Color.Black);
            
        }

        //private int X { get; set; }
        //private int Y { get; set; }

        private static Texture2D GetTexture(Terrain ter)
        {
            return TXAGame.TEXTURES["T_" + ter];
        }

        protected override void DoUpdate(GameTime time)
        {
            if (Omitted && Visible)
            {
                //!OmittedTile(MapScreen.GetTerrainIndex(Y, X))
                Visible = false;
            }
        }

        private void NumAreaAndTextPos()
        {
            int width = (int)(TXAGame.TEXTURES["TO_Number"].Width * TXAGame.SCALE);
            int height = (int)(TXAGame.TEXTURES["TO_Number"].Height * TXAGame.SCALE);

            numberPos = new Vector2(
                (Position.X) - (width / 2),
                (Position.Y) - (height / 2));

            Vector2 measurementValue = TXAGame.ARIAL.MeasureString(Tile.Value.ToString(CultureInfo.InvariantCulture))* TXAGame.SCALE;

            textPos = Position - (measurementValue/2);
        }

        protected override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            if (Visible)
            {
                if (IsTileNumbered(Tile))
                {
                    batch.Draw(TXAGame.TEXTURES["TO_Number"], numberPos, null, Color.Wheat, 0f, new Vector2(0, 0), TXAGame.SCALE, SpriteEffects.None, 0.0f);
                    batch.DrawString(TXAGame.ARIAL, Tile.Value.ToString(CultureInfo.InvariantCulture), textPos, valueColour, 0f, new Vector2(0, 0), TXAGame.SCALE, SpriteEffects.None, 0.0f);
                }
            }
        }

        private static bool IsTileNumbered(Board.Tile tile)
        {
            switch (tile.Terrain)
            {
                case Terrain.Pasture:
                case Terrain.Mountains:
                case Terrain.Hills:
                case Terrain.Fields:
                case Terrain.Forest:
                    return true;
                default:
                    return false;
            }
            //
        }

    }
}
