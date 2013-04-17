using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan
{
    public class GUIPiece : TXADrawableComponent
    {
        private Token type;
        public Token Type
        {
            get { return type; }
            set
            {
                type = value;
                Texture = GetTex(type);
            }
        }

        public int Player { get; private set; }

        public GUIPiece(Vector2 position, int player) : base(position, GetTex(Token.Settlement))
        {
            Player = player;
        }

        private static Texture2D GetTex(Token token)
        {
            switch (token)
            {
                case Token.City:
                    return TXAGame.TEXTURES["TO_City"];
                case Token.Settlement:
                default:
                    return TXAGame.TEXTURES["TO_Settle"];

            }
                
            
        }

        protected override void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture,Position,null,GUIRoad.GetPlayerColor(Player), Rotation,Origin,TXAGame.SCALE, SpriteEffects.None, 0f);

            //base.Draw(batch);
        }

        protected override void DoUpdate(GameTime time)
        {
            throw new NotImplementedException();
        }
    }
}
