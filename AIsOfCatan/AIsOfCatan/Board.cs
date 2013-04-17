using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public class Board
    {
        private static readonly int[] WaterTiles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 11, 12, 13, 18, 19, 25, 26, 31, 32, 33, 37, 38, 39, 40, 41, 42, 43, 44 };
        private static readonly int[] placementOrder = new int[] { 8, 14, 20, 27, 34, 35, 36, 30, 24, 17, 10, 9, 15, 21, 28, 29, 23, 16, 22 };
        private static readonly int[] valueOrder = new int[] { 5, 2, 6, 3, 8, 10, 9, 12, 11, 4, 8, 10, 9, 4, 5, 6, 3, 11 };

        public static int GetRowLength(int row)
        {
            return row % 2 == 0 ? 6 : 7;
        }

        private Tile[][] terrain;
        private Dictionary<Tuple<int, int>, int> roads;
        private Dictionary<Tuple<int, int, int>, Piece> settlements;
        private int robberLocation;

        private Board(Tile[][] terrain, Dictionary<Tuple<int, int>, int> roads, 
            Dictionary<Tuple<int, int, int>, Piece> settlements, int robber)
        {
            this.terrain = terrain;
            this.roads = roads;
            this.settlements = settlements;
            this.robberLocation = robber;
        }

        private Board()
        {
            // initialize fields
            roads = new Dictionary<Tuple<int, int>, int>(22);
            settlements = new Dictionary<Tuple<int, int, int>, Piece>(22);

            // create board
            terrain = new Tile[7][];
            bool longrow = false;
            for (int i = 0; i < 7; i++)
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
        }

        public Board(int terrainSeed, int numberSeed) : this()
        {
            InitTerrain(terrainSeed, numberSeed, true);
        }

        public Board(int terrainSeed) : this()
        {
            InitTerrain(terrainSeed, 0, false);
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
            var key = Get2Tuple(firstTile, secondTile);
            return roads.ContainsKey(key) ? roads[key] : -1;
        }

        /// <summary>
        /// Gives a (copy) of the dictionary holding all roads currently build on the board.
        /// </summary>
        /// <returns>A dictionary with all roads on the board.</returns>
        public Dictionary<Tuple<int,int>,int> GetAllRoads()
        {
            return new Dictionary<Tuple<int, int>, int>(roads);
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
            var key = Get3Tuple(firstTile, secondTile, thirdTile);
            return settlements.ContainsKey(key) ? settlements[key] : null;
        }

        /// <summary>
        /// Get all pieces built adjacent to the given tile index.
        /// </summary>
        /// <param name="index">The location of the tile.</param>
        /// <returns>A list of all the valid Pieces.</returns>
        public List<Piece> GetPieces(int index)
        {
            List<Piece> result = new List<Piece>();
            AddPiece(result, index, index - 7, index - 1);
            AddPiece(result, index, index - 7, index - 6);
            AddPiece(result, index, index - 6, index + 1);
            AddPiece(result, index, index + 1, index + 7);
            AddPiece(result, index, index + 6, index + 7);
            AddPiece(result, index, index + 6, index - 1);
            return result;
        } 

        /// <summary>
        /// Gives the current location of the Robber token.
        /// </summary>
        /// <returns>The index on the board currently containing the robber.</returns>
        public int GetRobberLocation()
        {
            return robberLocation;
        }

        /// <summary>
        /// Gives the resulting Board from moving the robber to
        /// the specified location.
        /// </summary>
        /// <param name="index">The index on the board to move the
        /// robber.</param>
        /// <returns>The resulting board.</returns>
        public Board MoveRobber(int index)
        {
            return new Board(terrain, new Dictionary<Tuple<int, int>, int>(roads), 
                new Dictionary<Tuple<int, int, int>, 
                    Piece>(settlements), index);
        }

        /// <summary>
        /// Places the given Piece on the specified position on the Board and
        /// returns the resulting Board.
        /// </summary>
        /// <param name="index1">The first index of the tiles to look between.</param>
        /// <param name="index2">The second index of the tiles to look between.</param>
        /// <param name="index3">The third index of the tiles to look between.</param>
        /// <param name="p">The Piece to place on the Board.</param>
        /// <returns>The resulting Board from placing the Piece.</returns>
        public Board PlacePiece(int index1, int index2, int index3, Piece p)
        {
            var newSettlements = new Dictionary<Tuple<int, int, int>, Piece>(settlements);
            newSettlements.Add(Get3Tuple(index1, index2, index3), p);
            return new Board(terrain, new Dictionary<Tuple<int, int>, int>(roads), newSettlements, robberLocation);
        }
        
        /// <summary>
        /// Places a road on the specified position on the Board and
        /// returns the resulting Board.
        /// </summary>
        /// <param name="index1">The first index of the tiles to look between.</param>
        /// <param name="index2">The second index of the tiles to look between.</param>
        /// <param name="playerID">The player who owns the road.</param>
        /// <returns>The resulting Board from placing the road.</returns>
        public Board PlaceRoad(int index1, int index2, int playerID)
        {
            var newRoads = new Dictionary<Tuple<int, int>, int>(roads);
            newRoads.Add(Get2Tuple(index1, index2), playerID);
            return new Board(terrain, newRoads, new Dictionary<Tuple<int, int, int>, Piece>(settlements), robberLocation);
        }

        /// <summary>
        /// Gives all edges (places to build roads) adjacent to the given intersection.
        /// </summary>
        /// <param name="index1">The first index of the tiles enclosing the intersection.</param>
        /// <param name="index2">The second index of the tiles enclosing the intersection.</param>
        /// <param name="index3">The third index of the tiles enclosing the intersection.</param>
        /// <returns>An array of 2-int-tuples with the (up to three) edges next to the intersection.</returns>
        public Tuple<int, int>[] GetAdjacentEdges(int index1, int index2, int index3)
        {
            var o = Get3Tuple(index1,index2,index3);
            return new Tuple<int,int>[]{new Tuple<int,int>(o.Item1,o.Item2), new Tuple<int,int>(o.Item2,o.Item3), new Tuple<int,int>(o.Item1,o.Item3)};
        }

        /// <summary>
        /// Gives all intersections (places to build settlements and cities)
        /// adjacent to the give edge.
        /// </summary>
        /// <param name="index1">The first index of the tiles to look between.</param>
        /// <param name="index2">The second index of the tiles to look between.</param>
        /// <returns>An array of 3-int-tuples with the (up to two) intersections at the ends of the edge.</returns>
        public Tuple<int,int,int>[] GetAdjacentIntersections(int index1, int index2)
        {
            var n1 = GetAdjacentTiles(index1);
            return GetAdjacentTiles(index2).Where(t => n1.Contains(t)).Select(t => new Tuple<int, int, int>(index1, index2, t)).ToArray();
        }

        /// <summary>
        /// Gets all tiles that are adjacent to the given tile.
        /// </summary>
        /// <param name="index">The tile to look around.</param>
        /// <returns>A list of all the (legal) adjacent tiles.</returns>
        public List<int> GetAdjacentTiles(int index)
        {
            List<int> result = new List<int>();
            if (index - 7 > 0) result.Add(index - 7);
            if (index - 6 > 0) result.Add(index - 6);
            if (index - 1 > 0) result.Add(index - 1);
            if (index + 1 < 45) result.Add(index + 1);
            if (index + 6 < 45) result.Add(index + 6);
            if (index + 7 < 45) result.Add(index + 7);
            return result;
        }

        /// <summary>
        /// Checks if the given intersection has no pieces build at the
        /// directly connected intersections (Distance Rule).
        /// </summary>
        /// <param name="index1">The first index of the tiles enclosing the intersection.</param>
        /// <param name="index2">The second index of the tiles enclosing the intersection.</param>
        /// <param name="index3">The third index of the tiles enclosing the intersection.</param>
        /// <returns>Returns true if the given intersection has no direct neighboring intersections
        /// containing settlements or cities, else false.</returns>
        public bool HasNoNeighbors(int index1, int index2, int index3)
        {
            Tuple<int,int,int> tuple = Get3Tuple(index1,index2,index3);
            return GetAdjacentEdges(tuple.Item1, tuple.Item2, tuple.Item3).
                SelectMany(edge => GetAdjacentIntersections(edge.Item1, edge.Item2)).
                All(inter => inter.Equals(tuple) || GetPiece(inter.Item1, inter.Item2, inter.Item3) == null);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Board Terrain:\n");
            foreach (Tile[] row in terrain)
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

        private void InitTerrain(int terrainSeed, int numberSeed, bool randomNumbers)
        {
            // generate random board

            // construct pool
            List<Terrain> terrainPool = new List<Terrain>(19);
            for (int i = 0; i < 4; i++)
            {
                if (i < 3) terrainPool.Add(Terrain.Hills);
                if (i < 3) terrainPool.Add(Terrain.Mountains);
                terrainPool.Add(Terrain.Pasture);
                terrainPool.Add(Terrain.Fields);
                terrainPool.Add(Terrain.Forest);
            }
            terrainPool.Add(Terrain.Desert);
            List<int> numberPool = new List<int>(valueOrder);

            Shuffle(terrainPool, terrainSeed); // shuffle
            if (randomNumbers) Shuffle(numberPool, numberSeed);

            // place randomized tiles
            bool desertFound = false;
            for (int i = 0; i < placementOrder.Length; i++)
            {

                Tuple<int, int> coords = GetTerrainCoords(placementOrder[i]);
                if (terrainPool[i] == Terrain.Desert)
                {
                    terrain[coords.Item1][coords.Item2] = new Tile(terrainPool[i], 0);
                    desertFound = true;
                    robberLocation = placementOrder[i]; // place the robber
                }
                else
                {
                    terrain[coords.Item1][coords.Item2] = new Tile(terrainPool[i], numberPool[i - (desertFound ? 1 : 0)]);
                }
            }
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

        private int GetTerrainIndex(int row, int col)
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

        private void AddPiece(List<Piece> list, int index1, int index2, int index3)
        {
            Piece hit = this.GetPiece(index1, index2, index3);
            if(hit != null) list.Add(hit);
        }

        private Tuple<int,int> Get2Tuple(int first, int second)
        {
            return new Tuple<int, int>(first < second ? first : second, first < second ? second : first);
        } 

        private Tuple<int,int,int> Get3Tuple(int first, int second, int third)
        {
            List<int> tiles = new List<int>(3) { first, second, third };
            tiles.Sort();

            return new Tuple<int, int, int>(tiles[0], tiles[1], tiles[2]);
        } 

        public class Tile
        {
            public Terrain Terrain { get; private set; }
            public int Value { get; internal set; }

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
