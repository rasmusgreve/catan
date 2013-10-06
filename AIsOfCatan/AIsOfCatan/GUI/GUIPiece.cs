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
        public int Tile1 { get; private set; }
        public int Tile2 { get; private set; }
        public int Tile3 { get; private set; }

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

        public GUIPiece(Vector2 position, int player, Token token, int t1, int t2, int t3) : base(position, GetTex(Token.Settlement))
        {
            Player = player;
            Type = token;
            Tile1 = t1;
            Tile2 = t2;
            Tile3 = t3;

            //throw new NotImplementedException();
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
            if (Visible)
            {
                batch.Draw(Texture, Position, null, MapScreen.GetPlayerColor(Player), Rotation, Origin, TXAGame.SCALE, SpriteEffects.None, 0f);
            }
        }

        protected override void DoUpdate(GameTime time)
        {
            //throw new NotImplementedException();
        }
    }
}
