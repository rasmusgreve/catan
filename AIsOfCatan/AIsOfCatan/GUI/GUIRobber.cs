using Microsoft.Xna.Framework;
using TroyXnaAPI;

namespace AIsOfCatan
{
    class GUIRobber : TXADrawableComponent
    {
        public GUIRobber(Vector2 pos) : base(pos, TXAGame.TEXTURES["TO_Robber"])
        {
        }

        protected override void DoUpdate(GameTime time)
        {
        }

        public void UpdateRobberPosition(Vector2 pos)
        {
            Position = pos;
        }
    }
}
