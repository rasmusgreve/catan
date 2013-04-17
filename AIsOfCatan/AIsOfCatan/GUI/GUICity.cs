using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan
{
    class GUICity : TXADrawableComponent, IGUIPiece
    {
        public GUICity(Vector2 position) : base(position, GetTex() )
        {
        }

        private static Texture2D GetTex()
        {
            return TXAGame.TEXTURES["TO_City"];
        }

        protected override void DoUpdate(GameTime time)
        {
            throw new NotImplementedException();
        }
    }
}
