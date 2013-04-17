using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MS.Internal.Xml.XPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan
{
    class MapScreen : TXAScreen
    {
        private GameState latestGameState;

        private GUITile[][] board = new GUITile[7][];

        private List<GUIRoad> roads = new List<GUIRoad>();

        public MapScreen(GameState initial)
        {
            latestGameState = initial;

            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new GUITile[Board.GetRowLength(i)];

                for (int j = 0; j < board[i].Length; j++)
                {
                    //Debug.WriteLine("SCALE " + TXAGame.SCALE);
                    GUITile tile = new GUITile(j, i, latestGameState.Board.GetTile(i, j));
                    AddDrawableComponent(tile);
                    board[i][j] = tile;
                }
            }

            //Test Roads
            UpdateGameState(initial);
        }

        public void UpdateGameState(GameState state)
        {
            


            latestGameState = state;

            Dictionary<Tuple<int,int>, int> allRoads = latestGameState.Board.GetAllRoads();

            foreach (KeyValuePair<Tuple<int, int>, int> road in allRoads)
            {
                int tile1 = road.Key.Item1;
                int tile2 = road.Key.Item2;

                ////Swap so that tile1 < tile2
                //if (tile1 > tile2)
                //{
                //    int temp = tile1;
                //    tile1 = tile2;
                //    tile2 = temp;
                //}

                if (roads.Exists(r => r.Tile1 == tile1 && r.Tile2 == tile2))
                {
                    continue;
                }

                Tuple<int, int> t1coord = GetTerrainCoords(tile1);
                Tuple<int, int> t2coord = GetTerrainCoords(tile2);

                Vector2 diffVector = board[t2coord.Item1][t2coord.Item2].Position / TXAGame.SCALE -
                                     board[t1coord.Item1][t1coord.Item2].Position / TXAGame.SCALE;

                Vector2 placeVector = (board[t1coord.Item1][t1coord.Item2].Position/TXAGame.SCALE)+(diffVector/2);

                //Vector2 placeVector = new Vector2(board[t1coord.Item1][t1coord.Item2].Position.X / TXAGame.SCALE + diffVector.X / 2,
                //                                  board[t1coord.Item1][t1coord.Item2].Position.Y / TXAGame.SCALE + diffVector.Y / 2);
                //Debug.WriteLine(string.Format("DiffVector: {0}", diffVector));

                float rotation = 0;//(float)Math.PI;

                if (diffVector.X < 0)
                {
                    float value = (float) Math.PI;
                    value /= 3;
                    value *= 2;
                    rotation = value;
                }
                else if (diffVector.X < diffVector.Y)
                {
                    float value = (float) Math.PI;
                    value /= 3;
                    value *= 1;
                    rotation = value; 
                }

                //Debug.WriteLine(string.Format("rotation: {0}, player {1}", rotation,road.Value));

                //Color color = GetPlayerColor(road.Value);

                GUIRoad newRoad = new GUIRoad(placeVector,rotation,road.Value, tile1, tile2);

                AddDrawableComponent(newRoad);

            }


            //TODO: update board with new info
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }

        private Tuple<int, int> GetTerrainCoords(int index)
        {
            int row = 0;
            bool longrow = false;
            while (index >= (longrow ? 7 : 6))
            {
                row++;
                index -= longrow ? 7 : 6;
                longrow = !longrow;
            }
            return new Tuple<int, int>(row, index);
        }
    }
}
