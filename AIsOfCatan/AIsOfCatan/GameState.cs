using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
        //                     19 of each in bank
    public enum Resources {Brick, Lumber, Wool, Grain, Ore}; // names from wikipedia
    //                   3      4       4        4       3          1       X
    public enum Terrain {Hills, Forest, Pasture, Fields, Mountains, Desert, Water} 
    //                            14      5             2             2             2
    public enum DevelopmentCards {Knight, VictoryPoint, RoadBuilding, YearOfPlenty, Monopoly}

    public enum Tokens {Road, Settlement, City};

    //----------------------------------------------------------------------------------------//

    public class GameState
    {
        private static readonly int[] WaterTiles = new int[]{0,1,2,3,4,5,6,7,11,12,13,18,19,25,26,31,32,33,37,38,39,40,41,42,43,44};

        private Terrain[][] terrain;
        private Dictionary<Tuple<int, int>, int> roads;
        private Dictionary<Tuple<int, int, int>, Piece> settlements;

        public GameState(int boardSeed)
        {
            // initialize fields
            roads = new Dictionary<Tuple<int, int>, int>(22);
            settlements = new Dictionary<Tuple<int, int, int>, Piece>(22);

            // create board
            terrain = new Terrain[7][];
            bool longrow = false;
            for(int i = 0; i < 7; i++)
            {
                terrain[i] = new Terrain[longrow ? 7 : 6];
                longrow = !longrow;
            }

            // place water
            foreach (int water in WaterTiles)
            {
                Tuple<int, int> coords = GetTerrainCoords(water);
                terrain[coords.Item1][coords.Item2] = Terrain.Water;
            }

            // generate random board
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
            Shuffle(terrainPool,boardSeed);

            for(int i = 0; i < 45; i++)
            {
                if(!WaterTiles.Contains(i))
                {
                    Tuple<int, int> coords = GetTerrainCoords(i);
                    terrain[coords.Item1][coords.Item2] = terrainPool.First();
                    terrainPool.RemoveAt(0);
                }
            }

            // test
            foreach (Terrain[] row in terrain)
            {
                foreach (Terrain tile in row)
                {
                    System.Diagnostics.Debug.Write(tile.ToString() + ",");
                    Console.Write(tile.ToString() + ",");
                }
                System.Diagnostics.Debug.WriteLine("");
                Console.WriteLine();
            }
        }

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

        public class Piece
        {
            public Tokens Token { get; private set; }
            public int Player { get; private set; }

            public Piece(Tokens token, int player)
            {
                this.Token = token;
                this.Player = player;
            }
        }
    }
}
