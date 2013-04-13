using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
        //                     19 of each in bank
    public enum Resource {Brick, Lumber, Wool, Grain, Ore}; // names from wikipedia
    //                   3      4       4        4       3          1       X
    public enum Terrain {Hills, Forest, Pasture, Fields, Mountains, Desert, Water} 
    //                            14      5             2             2             2
    public enum DevelopmentCard {Knight, VictoryPoint, RoadBuilding, YearOfPlenty, Monopoly}

    public enum Token {Road, Settlement, City};

    //----------------------------------------------------------------------------------------//

    public class GameState
    {
        private static readonly int[] WaterTiles = new int[]{0,1,2,3,4,5,6,7,11,12,13,18,19,25,26,31,32,33,37,38,39,40,41,42,43,44};
        private static readonly int[] placementOrder = new int[]{8,14,20,27,34,35,36,30,24,17,10,9,15,21,28,29,23,16,22};
        private static readonly int[] valueOrder = new int[]{5,2,6,3,8,10,9,12,11,4,8,10,9,4,5,6,3,11};

        private Tile[][] terrain;
        private Dictionary<Tuple<int, int>, int> roads;
        private Dictionary<Tuple<int, int, int>, Piece> settlements;
        private int robberLocation;

        public GameState(int boardSeed)
        {
            // initialize fields
            roads = new Dictionary<Tuple<int, int>, int>(22);
            settlements = new Dictionary<Tuple<int, int, int>, Piece>(22);

            // create board
            terrain = new Tile[7][];
            bool longrow = false;
            for(int i = 0; i < 7; i++)
            {
                terrain[i] = new Tile[longrow ? 7 : 6];
                longrow = !longrow;
            }

            // place water
            foreach (int water in WaterTiles)
            {
                Tuple<int, int> coords = GetTerrainCoords(water);
                terrain[coords.Item1][coords.Item2] = new Tile(Terrain.Water, 0);
            }

            // generate random board

            // construct pool
            List<Terrain> terrainPool = new List<Terrain>(19);
            for(int i = 0; i < 4; i++)
            {
                if(i < 3) terrainPool.Add(Terrain.Hills);
                if(i < 3) terrainPool.Add(Terrain.Mountains);
                terrainPool.Add(Terrain.Pasture);
                terrainPool.Add(Terrain.Fields);
                terrainPool.Add(Terrain.Forest);
            }
            terrainPool.Add(Terrain.Desert);
            Shuffle(terrainPool,boardSeed); // shuffle

            // place randomized tiles
            bool desertFound = false;
            for(int i = 0; i < placementOrder.Length; i++)
            {

                Tuple<int, int> coords = GetTerrainCoords(placementOrder[i]);
                if(terrainPool.First() == Terrain.Desert)
                {
                    terrain[coords.Item1][coords.Item2] = new Tile(terrainPool.First(), 0);
                    desertFound = true;
                    robberLocation = placementOrder[i]; // place the robber
                }else
                {
                    terrain[coords.Item1][coords.Item2] = new Tile(terrainPool.First(), valueOrder[i - (desertFound ? 1 : 0)]);
                }
                
                terrainPool.RemoveAt(0);
            }
        }

        public int GetRowLength(int row)
        {
            return row%2 == 0 ? 6 : 7;
        }

        /// <summary>
        /// Gives the type of terrain and dice value for a given index of the board.
        /// </summary>
        /// <param name="index">The index to check terrain for.</param>
        /// <returns>A Tile object containing the terrain type and dice value.</returns>
        public Tile GetTile(int index)
        {
            Tuple<int, int> coords = GetTerrainCoords(index);
            return terrain[coords.Item1][coords.Item2];
        }

        /// <summary>
        /// Give the type of terrain and dice value for the given coordinates of the board.
        /// </summary>
        /// <param name="row">The row of the tile.</param>
        /// <param name="column">The column of the tile. Even row numbers have a length
        /// of 6 and uneven has a length of 7.</param>
        /// <returns>A Tile object containing the terrain type and dice value.</returns>
        public Tile GetTile(int row, int column)
        {
            return terrain[row][column];
        }

        /// <summary>
        /// Gives the id of the player who has build a road at the requested edge.
        /// </summary>
        /// <param name="firstTile">The first index of the tiles to look between.</param>
        /// <param name="secondTile">The second index of the tiles to look between.</param>
        /// <returns>The player id of the player who has build a road here. If empty it returns -1.</returns>
        public int GetRoad(int firstTile, int secondTile)
        {
            var key = new Tuple<int, int>(firstTile < secondTile ? firstTile : secondTile, firstTile < secondTile ? secondTile : firstTile);
            return roads.ContainsKey(key) ? roads[key] : -1;
        }

        /// <summary>
        /// Gives the game piece at the vertex between three different tiles.
        /// </summary>
        /// <param name="firstTile">The first index of the tiles to look between.</param>
        /// <param name="secondTile">The second index of the tiles to look between.</param>
        /// <param name="thirdTile">The third index of the tiles to look between.</param>
        /// <returns>The Piece object at the location, containing the type of token 
        /// (Settlement or City) and the owning player id. If the location is empty
        /// it will return null.</returns>
        public Piece GetPiece(int firstTile, int secondTile, int thirdTile)
        {
            List<int> tiles = new List<int>(3) {firstTile, secondTile, thirdTile};
            tiles.Sort();

            var key = new Tuple<int, int, int>(tiles[0], tiles[1], tiles[2]);
            return settlements.ContainsKey(key) ? settlements[key] : null;
        }

        /// <summary>
        /// Gives the current location of the Robber token.
        /// </summary>
        /// <returns>The index on the board currently containing the robber.</returns>
        public int GetRobberLocation()
        {
            return robberLocation;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Board Terrain:\n");
            foreach(Tile[] row in terrain)
            {
                foreach (Tile tile in row)
                {
                    builder.Append(tile.ToString() + ",");
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }

        //------------Private Methods------------------------------//

        private Tuple<int,int> GetTerrainCoords(int index)
        {
            int row = 0;
            bool longrow = false;
            while(index >= (longrow ? 7 : 6))
            {
                row++;
                index -= longrow ? 7 : 6;
                longrow = !longrow;
            }
            return new Tuple<int, int>(row,index);
        }

        private int GetTerrainIndex(int row, int col)
        {
            int index = 0;
            bool longrow = false;
            while(row > 0)
            {
                row--;
                index += longrow ? 7 : 6;
                longrow = !longrow;
            }
            return index + col;
        }

        private void Shuffle<T>(IList<T> list, int seed)
        {
            Random rng = new Random(seed);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public class Tile
        {
            public Terrain Terrain { get; private set; }
            public int Value { get; private set; }

            public Tile(Terrain terrain, int value)
            {
                this.Terrain = terrain;
                this.Value = value;
            }

            public override string ToString()
            {
                return "[" + Terrain.ToString() + " : " + Value + "]";
            }
        }

        public class Piece
        {
            public Token Token { get; private set; }
            public int Player { get; private set; }

            public Piece(Token token, int player)
            {
                this.Token = token;
                this.Player = player;
            }
        }
    }
}
