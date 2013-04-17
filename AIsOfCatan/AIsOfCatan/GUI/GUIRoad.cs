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
    class GUIRoad : TXADrawableComponent
    {
        private int PlayerId { get; set; }
        public int Tile1 { get; private set; }
        public int Tile2 { get; private set; }

        public GUIRoad(Vector2 position, float rotation, int playerId, int tile1, int tile2) : base(position, TXAGame.TEXTURES["TO_Road"],rotation)
        {
            PlayerId = playerId;
            Tile1 = tile1;
            Tile2 = tile2;
        }

        protected override void DoUpdate(GameTime time)
        {
            //throw new NotImplementedException();
        }

        protected override void Draw(SpriteBatch batch)
        {
            //Debug.WriteLine(string.Format("Drawing at {0}", Position));

            batch.Draw(Texture,Position,null,GetPlayerColor(PlayerId),Rotation,Origin,TXAGame.SCALE,SpriteEffects.None, 0f);
        }


        internal static Color GetPlayerColor(int i)
        {
            switch (i)
            {
                case 1:
                    return Color.RoyalBlue;
                case 2:
                    return Color.Red;
                case 3:
                    return Color.Yellow;
                case 4:
                    return Color.White;
                default:
                    return Color.Black;
            }
            
            //TODO: update with correct colors
        }
    }
}
