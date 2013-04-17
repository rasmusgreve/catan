using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan.GUI
{
    class GUIRobber : TXADrawableComponent
    {
        public GUIRobber(Vector2 pos) : base(pos, TXAGame.TEXTURES["TO_Robber"])
        {
        }

        protected override void DoUpdate(GameTime time)
        {
        }

        protected override void Draw(SpriteBatch batch)
        {
            if (Visible)
            {
                batch.Draw(Texture,Position,null,Color.White,Rotation,Origin,TXAGame.SCALE,SpriteEffects.None, 0f);
            }
            //base.Draw(batch);
        }

        public void UpdateRobberPosition(Vector2 pos)
        {
            Position = pos;
        }
    }
}
