using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public class Board : IBoard
    {
        private static readonly int[] WaterTiles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 11, 12, 13, 18, 19, 25, 26, 31, 32, 33, 37, 38, 39, 40, 41, 42, 43, 44 };
        private static readonly int[] placementOrder = new int[] { 8, 14, 20, 27, 34, 35, 36, 30, 24, 17, 10, 9, 15, 21, 28, 29, 23, 16, 22 };
        private static readonly int[] valueOrder = new int[] { 11, 2, 9, 4, 3, 6, 4, 11, 10, 3, 9, 6, 5, 8, 5, 10, 8, 12 };
        private static readonly Tuple<int, int>[] harborEdges = new Tuple<int, int>[] { new Tuple<int, int>(1, 8), new Tuple<int, int>(3, 9),
                                                                                        new Tuple<int, int>(11, 17), new Tuple<int, int>(24, 25),
                                                                                        new Tuple<int, int>(30, 37), new Tuple<int, int>(35, 42),
                                                                                        new Tuple<int, int>(34, 40), new Tuple<int, int>(26, 27),
                                                                                        new Tuple<int, int>(13, 14)};

        public static int GetRowLength(int row)
        {
            return row % 2 == 0 ? 6 : 7;
        }

        private Tuple<int, int, int>[] allIntersections = null; // to minimize computation
        private Tuple<int, int>[] allEdges = null;

        private Tile[][] terrain;
        private Harbor[] harbors;
        private Dictionary<Tuple<int, int>, int> roads;
        private Dictionary<Tuple<int, int, int>, Piece> settlements;
        private int robberLocation;

        private Board(Tile[][] terrain, Dictionary<Tuple<int, int>, int> roads, 
            Dictionary<Tuple<int, int, int>, Piece> settlements, int robber, 
            Harbor[] harbors, Tuple<int, int, int>[] inter, Tuple<int, int>[] edges) : this()
        {
            this.allEdges = edges;
            this.allIntersections = inter;

            this.terrain = terrain;
            this.roads = roads;
            this.settlements = settlements;
            this.robberLocation = robber;
            this.harbors = harbors;
        }

        private Board()
        {
            // initialize fields
            roads = new Dictionary<Tuple<int, int>, int>(22);
            settlements = new Dictionary<Tuple<int, int, int>, Piece>(22);
            harbors = new Harbor[9];

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
            GetAllEdges();
            GetAllIntersections();
        }

        public Board(int terrainSeed) : this()
        {
            InitTerrain(terrainSeed, 0, false);
            GetAllEdges();
            GetAllIntersections();
        }

        public Dictionary<int, int> GetLongestRoad()
        {
            // find longest road for each player
            //      player, length
            var playersLongest = new Dictionary<int, int>(4);

            // floodfill from each road segment to see if it constitutes the longest road.
            var visited = new HashSet<Tuple<int, int>>();
            foreach (var road in roads)
            {
                if (visited.Contains(road.Key)) continue; // already explored
                visited.Add(road.Key);

                // get ends 
                var ends = this.GetAdjacentIntersections(road.Key.Item1, road.Key.Item2);
                var tempVisited = new HashSet<Tuple<int, int>>();
                tempVisited.Add(road.Key);
                int first  = CountRoadLengthFromIntersection(road.Value, ends[0], tempVisited, visited);
                int second = CountRoadLengthFromIntersection(road.Value, ends[1], tempVisited, visited);

                int result = first + 1 + second;
                if (!playersLongest.ContainsKey(road.Value) || playersLongest[road.Value] < result)
                {
                    playersLongest[road.Value] = result;
                }
            }

            return playersLongest;
        }

        public int GetPlayersLongestRoad(int playerID)
        {
            int highest = 0;
            // floodfill from each road segment to see if it constitutes the longest road.
            HashSet<Tuple<int, int>> visited = new HashSet<Tuple<int, int>>();
            foreach (var road in roads.Where(r => GetRoad(r.Key.Item1,r.Key.Item2) == playerID))
            {
                if (visited.Contains(road.Key)) continue; // already explored
                visited.Add(road.Key);

                // get ends 
                Tuple<int, int, int>[] ends = this.GetAdjacentIntersections(road.Key.Item1, road.Key.Item2);
                HashSet<Tuple<int, int>> tempVisited = new HashSet<Tuple<int, int>>();
                tempVisited.Add(road.Key);
                int first = CountRoadLengthFromIntersection(road.Value, ends[0], tempVisited, visited);
                int second = CountRoadLengthFromIntersection(road.Value, ends[1], tempVisited, visited);

                int result = first + 1 + second;
                if (highest < result)
                {
                    highest = result;
                }
            }
            return highest;
        }

        public Tile GetTile(int index)
        {
            Tuple<int, int> coords = GetTerrainCoords(index);
            return terrain[coords.Item1][coords.Item2];
        }

        public Tile GetTile(int row, int column)
        {
            return terrain[row][column];
        }

        public bool CanBuildPiece(int index1, int index2, int index3)
        {
            Tuple<int, int, int> tuple = Get3Tuple(index1, index2, index3);
            if (GetTile(index1).Terrain == Terrain.Water && GetTile(index2).Terrain == Terrain.Water && GetTile(index3).Terrain == Terrain.Water)
                return false;

            return !settlements.ContainsKey(tuple);
        }

        public int GetRoad(int firstTile, int secondTile)
        {
            var key = Get2Tuple(firstTile, secondTile);
            return roads.ContainsKey(key) ? roads[key] : -1;
        }

        public Dictionary<Tuple<int,int>,int> GetAllRoads()
        {
            return new Dictionary<Tuple<int, int>, int>(roads);
        }

        public bool CanBuildRoad(int index1, int index2)
        {
            Tuple<int, int> tuple = Get2Tuple(index1, index2);
            if (!IsLegalEdge(index1, index2)) return false;

            return !roads.ContainsKey(tuple);
        }

        public Piece GetPiece(int firstTile, int secondTile, int thirdTile)
        {
            var key = Get3Tuple(firstTile, secondTile, thirdTile);
            return settlements.ContainsKey(key) ? settlements[key] : null;
        }

        public Dictionary<Tuple<int,int,int>,Piece> GetAllPieces()
        {
            return new Dictionary<Tuple<int, int, int>, Piece>(settlements);
        } 

        public Piece[] GetPieces(int index)
        {
            List<Piece> result = new List<Piece>();
            AddPiece(result, index, index - 7, index - 1);
            AddPiece(result, index, index - 7, index - 6);
            AddPiece(result, index, index - 6, index + 1);
            AddPiece(result, index, index + 1, index + 7);
            AddPiece(result, index, index + 6, index + 7);
            AddPiece(result, index, index + 6, index - 1);
            return result.ToArray();
        }

        public Tuple<int, int, int>[] GetAllIntersections()
        {
            if(allIntersections == null){
                List<Tuple<int, int, int>> result = new List<Tuple<int, int, int>>(22);
                for (int r = 0; r < 7; r++)
                {
                    for (int c = 0; c < Board.GetRowLength(r); c++)
                    {
                        Tuple<int, int, int> south = null;
                        Tuple<int,int,int> southeast = null;

                        if (r % 2 == 0)
                        {
                            if(r + 1 < 7 && c + 1 < Board.GetRowLength(r + 1)) 
                                south = new Tuple<int, int, int>(GetTerrainIndex(r, c), GetTerrainIndex(r + 1, c), GetTerrainIndex(r + 1, c + 1));
                            if (r + 1 < 7 && c + 1 < Board.GetRowLength(r)) 
                                southeast = new Tuple<int, int, int>(GetTerrainIndex(r, c), GetTerrainIndex(r, c + 1), GetTerrainIndex(r + 1, c + 1));
                        }
                        else
                        {
                            if (r + 1 < 7 && c - 1 >= 0 && c < 6)
                                south = new Tuple<int, int, int>(GetTerrainIndex(r, c), GetTerrainIndex(r + 1, c - 1), GetTerrainIndex(r + 1, c));
                            if (r + 1 < 7 && c < 6)
                                southeast = new Tuple<int, int, int>(GetTerrainIndex(r, c), GetTerrainIndex(r, c + 1), GetTerrainIndex(r + 1, c));
                        }
                    
                        if (south != null && (GetTile(south.Item1).Terrain != Terrain.Water || GetTile(south.Item2).Terrain != Terrain.Water || GetTile(south.Item1).Terrain != Terrain.Water))
                            result.Add(south);
                        if (southeast != null && (GetTile(southeast.Item1).Terrain != Terrain.Water || GetTile(southeast.Item2).Terrain != Terrain.Water || GetTile(southeast.Item1).Terrain != Terrain.Water))
                            result.Add(southeast);
                    }
                }
                allIntersections = result.ToArray();

            }
            return allIntersections.ToArray();
        }

        public Tuple<int, int>[] GetAllEdges()
        {
            if(allEdges == null){
                HashSet<Tuple<int,int>> result = new HashSet<Tuple<int,int>>();
                for (int i = 0; i < 45; i++)
                {
                    this.GetAdjacentTiles(i).Where(j => this.IsLegalEdge(i, j)).ForEach(j => result.Add(this.Get2Tuple(i, j)));
                }
                allEdges = result.ToArray();
            }
            
            return allEdges.ToArray();
        }

        public int GetRobberLocation()
        {
            return robberLocation;
        }

        public IBoard MoveRobber(int index)
        {
            return new Board(terrain, new Dictionary<Tuple<int, int>, int>(roads), 
                new Dictionary<Tuple<int, int, int>, 
                    Piece>(settlements), index, harbors, allIntersections, allEdges);
        }

        public IBoard PlacePiece(int index1, int index2, int index3, Piece p)
        {
            var newSettlements = new Dictionary<Tuple<int, int, int>, Piece>(settlements);
            newSettlements[Get3Tuple(index1, index2, index3)] = p;
            return new Board(terrain, new Dictionary<Tuple<int, int>, int>(roads), newSettlements, robberLocation, harbors, allIntersections, allEdges);
        }
        
        public IBoard PlaceRoad(int index1, int index2, int playerID)
        {
            var newRoads = new Dictionary<Tuple<int, int>, int>(roads);
            newRoads.Add(Get2Tuple(index1, index2), playerID);
            return new Board(terrain, newRoads, new Dictionary<Tuple<int, int, int>, Piece>(settlements), robberLocation, harbors, allIntersections, allEdges);
        }

        public Tuple<int, int>[] GetAdjacentEdges(int index1, int index2, int index3)
        {
            var o = Get3Tuple(index1,index2,index3);
            List<Tuple<int, int>> result = new List<Tuple<int, int>>(3);
            if (IsLegalEdge(index1, index2)) result.Add(new Tuple<int,int>(o.Item1,o.Item2));
            if (IsLegalEdge(index2, index3)) result.Add(new Tuple<int, int>(o.Item2, o.Item3));
            if (IsLegalEdge(index1, index3)) result.Add(new Tuple<int, int>(o.Item1, o.Item3));
            return result.ToArray();
        }

        public Tuple<int,int,int>[] GetAdjacentIntersections(int index1, int index2)
        {
            var n1 = GetAdjacentTiles(index1);
            return GetAdjacentTiles(index2)
                .Where(t => n1.Contains(t))
                .Where(t => GetTile(index1).Terrain != Terrain.Water || GetTile(index2).Terrain != Terrain.Water || GetTile(t).Terrain != Terrain.Water)
                .Select(t => Get3Tuple(index1,index2,t))
                .ToArray();
        }

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

        public bool HasNoNeighbors(int index1, int index2, int index3)
        {
            Tuple<int,int,int> tuple = Get3Tuple(index1,index2,index3);
            return GetAdjacentEdges(tuple.Item1, tuple.Item2, tuple.Item3).
                SelectMany(edge => GetAdjacentIntersections(edge.Item1, edge.Item2)).
                All(inter => inter.Equals(tuple) || GetPiece(inter.Item1, inter.Item2, inter.Item3) == null);
        }

        public Harbor[] GetHarbors()
        {
            return this.harbors.ToArray();
        }

        public HarborType[] GetPlayersHarbors(int playerID)
        {
            HashSet<HarborType> result = new HashSet<HarborType>();
            foreach (var h in harbors)
            {
                var corners = GetAdjacentIntersections(h.Position.Item1, h.Position.Item2);
                foreach (Tuple<int, int, int> pos in corners)
                {
                    Piece curPiece = GetPiece(pos.Item1,pos.Item2,pos.Item3);
                    if (curPiece != null && curPiece.Player == playerID) result.Add(h.Type);
                }
            }
            return result.ToArray();
        }

        public Tuple<int, int>[] GetPossibleRoads(int playerID)
        {
            List<Tuple<int, int>> result = new List<Tuple<int, int>>(15);
            foreach(Tuple<int,int> edge in this.GetAllEdges()){
                // must be empty
                if (roads.ContainsKey(edge)) continue;
                
                
                foreach (Tuple<int, int, int> inter in this.GetAdjacentIntersections(edge.Item1, edge.Item2))
                {
                    // must not be other players piece at intersection
                    Piece atInter = this.GetPiece(inter.Item1,inter.Item2,inter.Item3);
                    if (atInter != null && atInter.Player != playerID) continue;

                    // must have one of the players roads connected to one of the end edges
                    if(this.GetAdjacentEdges(inter.Item1,inter.Item2,inter.Item3).Any(e => this.GetRoad(e.Item1,e.Item2) == playerID))
                    {
                        result.Add(edge);
                        break;
                    }  
                }
            }
            return result.ToArray();
        }

        public Tuple<int, int, int>[] GetPossibleSettlements(int playerID)
        {
            return new HashSet<Tuple<int,int,int>>(this.roads.Keys.Where(k => roads[k] == playerID)
                .SelectMany(e => this.GetAdjacentIntersections(e.Item1,e.Item2)))
                .Where(i => HasNoNeighbors(i.Item1,i.Item2,i.Item3) 
                    && !this.settlements.ContainsKey(i)).ToArray();
        }

        public Tuple<int, int, int>[] GetPossibleCities(int playerID)
        {
            return this.settlements.Keys.Where(i => settlements[i].Player == playerID && settlements[i].Token == Token.Settlement).ToArray();
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

            List<HarborType> harborPool = new List<HarborType>(9);
            harborPool.Add(HarborType.Brick);
            harborPool.Add(HarborType.Grain);
            harborPool.Add(HarborType.Lumber);
            harborPool.Add(HarborType.Ore);
            harborPool.Add(HarborType.Wool);
            for(int i = 0; i < 4; i++) harborPool.Add(HarborType.ThreeForOne);

            // shuffles
            Shuffle(terrainPool, terrainSeed);
            Shuffle(harborPool, terrainSeed);
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

            // place harbors random at positions
            for (int i = 0; i < 9; i++) this.harbors[i] = new Harbor(harborPool[i], harborEdges[i]);
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

        private int CountRoadLengthFromIntersection(int playerID, Tuple<int, int,int> curInt, HashSet<Tuple<int, int>> visited, HashSet<Tuple<int,int>> globalVisited)
        {
            // check for break
            Piece curPiece = GetPiece(curInt.Item1,curInt.Item2,curInt.Item3);
            if(curPiece != null && curPiece.Player != playerID) return 0; // no more edges this direction

            // find connections
            var edgesOut = GetAdjacentEdges(curInt.Item1, curInt.Item2, curInt.Item3).Where(e => !visited.Contains(e) && GetRoad(e.Item1,e.Item2) == playerID).ToArray(); // temp array

            int highest = 0;
            foreach (var edge in edgesOut)
            {
                // add edges to visited
                visited.Add(edge);
                globalVisited.Add(edge);

                Tuple<int,int,int> otherEnd = GetAdjacentIntersections(edge.Item1, edge.Item2).First(i => !i.Equals(curInt));
                int depthSearch = CountRoadLengthFromIntersection(playerID, otherEnd, visited, globalVisited);

                if (1 + depthSearch > highest) highest = 1 + depthSearch;

                visited.Remove(edge);
            }

            return highest;
        }

        private Boolean IsLegalEdge(int index1, int index2)
        {
            return GetTile(index1).Terrain != Terrain.Water || GetTile(index2).Terrain != Terrain.Water;
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

        public class Harbor
        {
            public HarborType Type { get; private set; }
            public Tuple<int,int> Position { get; private set; }

            public Harbor(HarborType type, Tuple<int,int> position){
                this.Type = type;
                this.Position = position;
            }
        }
    }
}
