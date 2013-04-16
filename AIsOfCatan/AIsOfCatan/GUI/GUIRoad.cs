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
        private float Rotation { get; set; }
        private int PlayerId { get; set; }
        public int Tile1 { get; private set; }
        public int Tile2 { get; private set; }

        public GUIRoad(Vector2 position, float rotation, int playerId, int tile1, int tile2) : base(position, TXAGame.TEXTURES["TO_Road"])
        {
            Rotation = rotation;
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
            Debug.WriteLine("Drawing");

            batch.Draw(Texture,Position,null,GetPlayerColor(PlayerId),Rotation,Position,GUIControl.SCALE,SpriteEffects.None, 0f);
        }


        private Color GetPlayerColor(int i)
        {
            return Color.RoyalBlue;
            //TODO: update with correct colors
        }
    }
}
