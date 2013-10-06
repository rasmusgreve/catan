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
    class GUILogList<T> : TXAListOverview<T> where T : TXADrawableComponent
    {
        private int Border { get; set; }

        private int Width { get; set; }
        private int Height { get; set; }

        public int Count { get { return fullList.Count; } }

        private readonly List<T> fullList = new List<T>();
        private int numOfElements;

        public GUILogList(Vector2 position) : base(position)
        {
            Border = 2;
            Height = 500;
            Width = 300;
            //fullList = new List<T>();
        }

        public GUILogList(Vector2 pos, int height, int width) : this(pos)
        {
            Height = height;
            Width = width;
        }

        protected override void Draw(SpriteBatch batch)
        {
            if (Visible)
            {
                Rectangle blackArea = new Rectangle(Area.X - Border, Area.Y - Border, Area.Width + 2 * Border, Area.Height + 2 * Border);
                batch.Draw(TXAGame.WHITE_BASE, blackArea, null, Color.Black);
                batch.Draw(TXAGame.WHITE_BASE, Area, null, Color.White);
            }
            
            base.Draw(batch);
        }

        public override void AddToList(T addItem)
        {
            fullList.Add(addItem);

            if (List.Count == 0)
            {
                numOfElements = (Height / fullList.First().Area.Height);
            }
            
        }

        protected override void UpdateRect()
        {
            Area = new Rectangle(
                (int)(Math.Round(Position.X)),
                (int)(Math.Round(Position.Y)),
                Width,
                Height);
        }

        protected override void DoUpdate(GameTime time)
        {

            base.DoUpdate(time);

            T last = fullList.LastOrDefault();
            if (last != null && !last.Equals(List.FirstOrDefault()))
            {

                List<T> tempList = fullList.ToList();
                tempList.Reverse();
                if (tempList.Count <= numOfElements)
                {
                    List = tempList.GetRange(0, tempList.Count);
                }
                else
                {
                    List = tempList.GetRange(0, numOfElements);
                }
                

            }
        }
    }
}
