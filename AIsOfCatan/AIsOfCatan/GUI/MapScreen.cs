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
        private List<GUIPiece> pieces = new List<GUIPiece>();

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

            //Test Roads and pieces
            UpdateGameState(initial);
        }

        public void UpdateGameState(GameState state)
        {
            latestGameState = state;

            #region Roads
            Dictionary<Tuple<int,int>, int> allRoads = latestGameState.Board.GetAllRoads();

            foreach (KeyValuePair<Tuple<int, int>, int> road in allRoads)
            {
                int tile1 = road.Key.Item1;
                int tile2 = road.Key.Item2;

                if (roads.Exists(r => r.Tile1 == tile1 && r.Tile2 == tile2))
                {
                    continue;
                }

                Tuple<int, int> t1coord = GetTerrainCoords(tile1);
                Tuple<int, int> t2coord = GetTerrainCoords(tile2);

                Vector2 diffVector = board[t2coord.Item1][t2coord.Item2].Position / TXAGame.SCALE -
                                     board[t1coord.Item1][t1coord.Item2].Position / TXAGame.SCALE;

                Vector2 placeVector = (board[t1coord.Item1][t1coord.Item2].Position/TXAGame.SCALE)+(diffVector/2);

                float rotation = 0;

                float value = (float) (Math.PI/3);

                if (diffVector.X < 0)
                {
                    rotation = value*2;
                }
                else if (diffVector.X < diffVector.Y)
                {
                    rotation = value; 
                }

                GUIRoad newRoad = new GUIRoad(placeVector,rotation,road.Value, tile1, tile2);

                AddDrawableComponent(newRoad);

            }
            #endregion

            #region Pieces

            Dictionary<Tuple<int, int, int>, Board.Piece> piecelist = state.Board.GetAllPieces();

            foreach (KeyValuePair<Tuple<int, int, int>, Board.Piece> piece in piecelist)
            {
                int t1 = piece.Key.Item1;
                int t2 = piece.Key.Item2;
                int t3 = piece.Key.Item3;

                GUIPiece alreadyPiece = pieces.FirstOrDefault(e => e.Tile1 == t1 && e.Tile2 == t2 && e.Tile3 == t3);

                if (alreadyPiece != null)
                {
                    if (alreadyPiece.Type == Token.Settlement && piece.Value.Token == Token.City)
                    {
                        alreadyPiece.Type = Token.City;
                    }
                    continue;
                }

                //Tuple<int, int> t2C = GetTerrainCoords(2);

                Vector2 diffVector = t1 + 1 == t2
                                         ? new Vector2(GUITile.TileWidth()/2, GUITile.TileHeight()/4)
                                         : new Vector2(0, GUITile.TileHeight()/2);

                Tuple<int, int> t1C = GetTerrainCoords(t1);

                Vector2 placePos = board[t1C.Item1][t1C.Item2].Position/TXAGame.SCALE + diffVector;

                GUIPiece newPiece = new GUIPiece(placePos, piece.Value.Player, piece.Value.Token, t1, t2, t3);

                pieces.Add(newPiece);

                AddDrawableComponent(newPiece);
            }



            #endregion


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

        internal static int GetTerrainIndex(int row, int col)
        {
            int index = 0;
            bool longrow = false;
            while (row > 0)
            {
                row--;
                index += longrow ? 7 : 6;
                longrow = !longrow;
            }
            return index + col;
        }

    }
}
