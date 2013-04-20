using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan
{
    class GUIBufferTextBlock : TXATextBlock
    {
        private static Vector2 Buffer = new Vector2(3,3);
        private Vector2 textPos;

        public GUIBufferTextBlock(Vector2 pos) : base(pos)
        {
            textPos = Position + Buffer;
        }

        public GUIBufferTextBlock(Vector2 pos, int bufSize) : this(pos)
        {
            Buffer = new Vector2(bufSize, bufSize);
            textPos = Position + Buffer;
        }

        protected override void DoUpdate(GameTime time)
        {
            base.DoUpdate(time);
            textPos = Position + Buffer;
        }

        protected override void Draw(SpriteBatch batch)
        {
            if (Visible)
            {
                batch.DrawString(TXAGame.ARIAL, Text, textPos, Color.Black, Rotation, Origin, TXAGame.SCALE, SpriteEffects.None, 0f);
            }
        }

        protected override void UpdateRect()
        {
            Vector2 textVector;
            if (Text != null)
            {
                textVector = TXAGame.ARIAL.MeasureString(Text) * TXAGame.SCALE;
            }
            else
            {
                textVector = TXAGame.ARIAL.MeasureString("") * TXAGame.SCALE;
            }

            Area = new Rectangle(
                (int) (Math.Round(Position.X)), (int) (Math.Round(Position.Y)),
                (int) (Math.Round(textVector.X + (Buffer.X*2))), (int) (Math.Round(textVector.Y + (Buffer.Y*2))));

        }
    }
}
