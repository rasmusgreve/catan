using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan
{
    class GUISettlement : TXADrawableComponent, IGUIPiece
    {
        public GUISettlement(Vector2 position) : base(position, GetTex())
        {
        }

        public GUISettlement(Vector2 position, Texture2D tex, float rota) : base(position, tex, rota)
        {
        }

        protected override void DoUpdate(GameTime time)
        {
            throw new NotImplementedException();
        }
    }
}
