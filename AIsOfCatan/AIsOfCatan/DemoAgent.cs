using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class DemoAgent : Agent
    {
        public void BeforeDiceRoll(GameState state, Action PlayKnight)
        {
            try
            {
                PlayKnight();
            }
            catch { } //Always try playing knights! (bad way of doing it as exceptions are slow)
        }
    }
}
