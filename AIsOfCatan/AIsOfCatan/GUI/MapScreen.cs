using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using AIsOfCatan.Log;
using MS.Internal.Xml.XPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroyXnaAPI;

namespace AIsOfCatan
{
    class MapScreen : TXAScreen
    {
        private GameState latestGameState;

        private DateTime lastLogPoll;

        private readonly GUITile[][] board = new GUITile[7][];

        private readonly List<int> omitted = new List<int> { 0, 5, 6, 12, 32, 38, 39, 44 };

        private readonly List<GUIRoad> roads = new List<GUIRoad>();
        private readonly List<GUIPiece> pieces = new List<GUIPiece>();

        private readonly GUILogList<GUIBufferTextBlock> gamelog;

        private readonly GUIRobber robber;

        public MapScreen(GameState initial)
        {
            latestGameState = initial;

            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new GUITile[Board.GetRowLength(i)];

                for (int j = 0; j < board[i].Length; j++)
                {
                    bool omit = OmittedTile(GetTerrainIndex(i, j));
                    GUITile tile = new GUITile(j, i, latestGameState.Board.GetTile(i, j), omit);
                    AddDrawableComponent(tile);
                    board[i][j] = tile;
                }
            }

            //Entire Screen size: GraphicsAdapter.DefaultAdapter.CurrentDisplayMode


            const int logWidth = 350;

            //int gameH = graphics.Viewport.Height;
            //int gameW = graphics.Viewport.Width;
            const int screenWidth = 1280;
            const int screenHeight = 720;

            //Debug.WriteLine(string.Format("width {0}, height {1}", gameW, gameH));

            gamelog = new GUILogList<GUIBufferTextBlock>(new Vector2((screenWidth-logWidth)/TXAGame.SCALE, 1/TXAGame.SCALE),screenHeight-2, logWidth-1);

            //AddButton(new TXAButton(new Vector2(750 / TXAGame.SCALE, 50 / TXAGame.SCALE), "Debug Log"), InsertText);

            AddDrawableComponent(gamelog);

            robber = new GUIRobber(GetRobberPos());

            AddDrawableComponent(robber);

            initial.GetEventsSince(DateTime.MinValue).ForEach(a => InsertLogEvent(a.ToString()));
            lastLogPoll = DateTime.Now;

            //Test Roads and pieces
            UpdateGameState(initial);
        }

        private void InsertLogEvent(string logText)
        {
            GUIBufferTextBlock textB = new GUIBufferTextBlock(new Vector2(0,-25)) {Text = logText};
            gamelog.AddToList(textB);
        }

        public void UpdateGameState(GameState state)
        {
            Console.WriteLine("UpdateGameState called");

            latestGameState = state;

            #region Roads
            Dictionary<Tuple<int, int>, int> allRoads = latestGameState.Board.GetAllRoads();

            foreach (KeyValuePair<Tuple<int, int>, int> road in allRoads)
            {
                int tile1 = road.Key.Item1;
                int tile2 = road.Key.Item2;

                if (roads.Exists(r => r.Tile1 == tile1 && r.Tile2 == tile2))
                {
                    continue;
                }

                Tuple<int, int> t1Coord = GetTerrainCoords(tile1);
                Tuple<int, int> t2Coord = GetTerrainCoords(tile2);

                Vector2 diffVector = board[t2Coord.Item1][t2Coord.Item2].Position / TXAGame.SCALE -
                                     board[t1Coord.Item1][t1Coord.Item2].Position / TXAGame.SCALE;

                Vector2 placeVector = (board[t1Coord.Item1][t1Coord.Item2].Position/TXAGame.SCALE)+(diffVector/2);

                float rotation = 0;

                const float value = (float) (Math.PI/3);

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

            #region Robber
            robber.UpdateRobberPosition(GetRobberPos());
            #endregion

            #region GameLog

            List<LogEvent> events = state.GetEventsSince(lastLogPoll);
            Console.WriteLine(events.Count);
            events.ForEach(a => InsertLogEvent(a.ToString()));
            lastLogPoll = DateTime.Now;
            #endregion
        }

        private Vector2 GetRobberPos()
        {
            int robberTile = latestGameState.Board.GetRobberLocation();
            Tuple<int, int> robberCoord = GetTerrainCoords(robberTile);

            return board[robberCoord.Item1][robberCoord.Item2].Position/TXAGame.SCALE;
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

        private bool OmittedTile(int index)
        {
            return omitted.Contains(index);
        }

        internal static Color GetPlayerColor(int i)
        {
            switch (i)
            {
                case 1:
                    return Color.RoyalBlue;
                case 2:
                    return Color.Red;
                case 3:
                    return Color.Orange;
                case 4:
                    return Color.White;
                default:
                    return Color.Black;
            }
        }

    }
}
