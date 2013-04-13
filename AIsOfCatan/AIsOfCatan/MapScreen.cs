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
    class MapScreen : TXAScreen
    {
        private GameState latestGameState;

        private GUITile[][] board = new GUITile[7][];

        public MapScreen(GameState initial)
        {
            latestGameState = initial;

            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new GUITile[latestGameState.GetRowLength(i)];

                for (int j = 0; j < board[i].Length; j++)
                {
                    GUITile tile = new GUITile(j, i, latestGameState.GetTile(i, j));
                    AddDrawableComponent(tile);
                    board[i][j] = tile;
                }
            }
        }

        public void UpdateGameState(GameState state)
        {
            latestGameState = state;

            //TODO: update board with new info
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }
    }
}
