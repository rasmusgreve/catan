using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan.GUI
{
    class GUIPlayerInfo : TXADrawableComponent
    {
        public GUIPlayerInfo(Vector2 position) : base(position, TXAGame.WHITE_BASE)
        {
            //Text Agent Name Line, largest of the fonts
            //Text Agent Desc Line, smaller font
            //Text: Knights
            //Text Knight amount
            //Text: Long Road
            //Text Road Length
            //Text: Points
            //Text Point Amount
            //List Develop Cards
            //picture Bricks
            //Text amount bricks
            //picture Grain
            //Text amount grain
            //picture Ore
            //Text amount ore
            //picture lumber
            //Text amount lumber
            //picture Wool
            //Text amount wool
        }

        protected override void Draw(SpriteBatch batch)
        {
            //Text Agent Name Line, largest of the fonts
            //Text Agent Desc Line, smaller font
            //Text: Knights
            //Text Knight amount
            //Text: Long Road
            //Text Road Length
            //Text: Points
            //Text Point Amount
            //List Develop Cards
            //picture Bricks
            //Text amount bricks
            //picture Grain
            //Text amount grain
            //picture Ore
            //Text amount ore
            //picture lumber
            //Text amount lumber
            //picture Wool
            //Text amount wool
            base.Draw(batch);
        }

        protected override void DoUpdate(GameTime time)
        {
            throw new NotImplementedException();
        }
    }
}
