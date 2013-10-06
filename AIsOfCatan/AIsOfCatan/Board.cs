using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.API;

namespace AIsOfCatan
{
    public class Board : IBoard
    {
        private static readonly int[] WaterTiles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 11, 12, 13, 18, 19, 25, 26, 31, 32, 33, 37, 38, 39, 40, 41, 42, 43, 44 };
        private static readonly int[] placementOrder = new int[] { 8, 14, 20, 27, 34, 35, 36, 30, 24, 17, 10, 9, 15, 21, 28, 29, 23, 16, 22 };
        private static readonly int[] valueOrder = new int[] { 11, 2, 9, 4, 3, 6, 4, 11, 10, 3, 9, 6, 5, 8, 5, 10, 8, 12 };
        private static readonly Edge[] harborEdges = new Edge[] { new Edge(1, 8), new Edge(3, 9),
                                                                                        new Edge(11, 17), new Edge(24, 25),
                                                                                        new Edge(30, 37), new Edge(35, 42),
                                                                                        new Edge(34, 40), new Edge(26, 27),
                                                                                        new Edge(13, 14)};

        public static int GetRowLength(int row)
        {
            return row % 2 == 0 ? 6 : 7;
        }

        private Intersection[] allIntersections = null; // to minimize computation
        private Edge[] allEdges = null;

        private Tile[][] terrain;
        private Harbor[] harbors;
        private Dictionary<Edge, int> roads;
        private Dictionary<Intersection, Piece> settlements;
        private int robberLocation;

        private Board(Tile[][] terrain, Dictionary<Edge, int> roads,
            Dictionary<Intersection, Piece> settlements, int robber,
            Harbor[] harbors, Intersection[] inter, Edge[] edges)
            : this()
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
            roads = new Dictionary<Edge, int>(22);
            settlements = new Dictionary<Intersection, Piece>(22);
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
            var visited = new HashSet<Edge>();
            foreach (var road in roads)
            {
                if (visited.Contains(road.Key)) continue; // already explored
                visited.Add(road.Key);

                // get ends 
                var ends = this.GetAdjacentIntersections(road.Key);
                var tempVisited = new HashSet<Edge>();
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
            HashSet<Edge> visited = new HashSet<Edge>();
            foreach (var road in roads.Where(r => GetRoad(r.Key) == playerID))
            {
                if (visited.Contains(road.Key)) continue; // already explored
                visited.Add(road.Key);

                // get ends 
                Intersection[] ends = this.GetAdjacentIntersections(road.Key);
                HashSet<Edge> tempVisited = new HashSet<Edge>();
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

        public bool CanBuildPiece(Intersection intersection)
        {
            if (GetTile(intersection.FirstTile).Terrain == Terrain.Water && GetTile(intersection.SecondTile).Terrain == Terrain.Water && GetTile(intersection.ThirdTile).Terrain == Terrain.Water)
                return false;

            return !settlements.ContainsKey(intersection);
        }

        public int GetRoad(Edge edge)
        {
            return roads.ContainsKey(edge) ? roads[edge] : -1;
        }

        public Dictionary<Edge,int> GetAllRoads()
        {
            return new Dictionary<Edge, int>(roads);
        }

        public bool CanBuildRoad(Edge edge)
        {
            if (!IsLegalEdge(edge)) return false;

            return !roads.ContainsKey(edge);
        }

        public Piece GetPiece(Intersection intersection)
        {
            return settlements.ContainsKey(intersection) ? settlements[intersection] : null;
        }

        public Dictionary<Intersection,Piece> GetAllPieces()
        {
            return new Dictionary<Intersection, Piece>(settlements);
        } 

        public Piece[] GetPieces(int index)
        {
            List<Piece> result = new List<Piece>();
            AddPiece(result, new Intersection(index, index - 7, index - 1));
            AddPiece(result, new Intersection(index, index - 7, index - 6));
            AddPiece(result, new Intersection(index, index - 6, index + 1));
            AddPiece(result, new Intersection(index, index + 1, index + 7));
            AddPiece(result, new Intersection(index, index + 6, index + 7));
            AddPiece(result, new Intersection(index, index + 6, index - 1));
            return result.ToArray();
        }

        public Intersection[] GetAllIntersections()
        {
            if(allIntersections == null){
                List<Intersection> result = new List<Intersection>(22);
                for (int r = 0; r < 7; r++)
                {
                    for (int c = 0; c < Board.GetRowLength(r); c++)
                    {
                        Intersection south = null;
                        Intersection southeast = null;

                        if (r % 2 == 0)
                        {
                            if(r + 1 < 7 && c + 1 < Board.GetRowLength(r + 1))
                                south = new Intersection(GetTerrainIndex(r, c), GetTerrainIndex(r + 1, c), GetTerrainIndex(r + 1, c + 1));
                            if (r + 1 < 7 && c + 1 < Board.GetRowLength(r))
                                southeast = new Intersection(GetTerrainIndex(r, c), GetTerrainIndex(r, c + 1), GetTerrainIndex(r + 1, c + 1));
                        }
                        else
                        {
                            if (r + 1 < 7 && c - 1 >= 0 && c < 6)
                                south = new Intersection(GetTerrainIndex(r, c), GetTerrainIndex(r + 1, c - 1), GetTerrainIndex(r + 1, c));
                            if (r + 1 < 7 && c < 6)
                                southeast = new Intersection(GetTerrainIndex(r, c), GetTerrainIndex(r, c + 1), GetTerrainIndex(r + 1, c));
                        }
                    
                        if (south != null && (GetTile(south.FirstTile).Terrain != Terrain.Water || GetTile(south.SecondTile).Terrain != Terrain.Water))
                            result.Add(south);
                        if (southeast != null && (GetTile(southeast.FirstTile).Terrain != Terrain.Water || GetTile(southeast.SecondTile).Terrain != Terrain.Water))
                            result.Add(southeast);
                    }
                }
                allIntersections = result.ToArray();

            }
            return allIntersections.ToArray();
        }

        public Edge[] GetAllEdges()
        {
            if(allEdges == null){
                HashSet<Edge> result = new HashSet<Edge>();
                for (int i = 0; i < 45; i++)
                {
                    this.GetAdjacentTiles(i).Where(j => this.IsLegalEdge(new Edge(i, j))).ForEach(j => result.Add(new Edge(i, j)));
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
            return new Board(terrain, new Dictionary<Edge, int>(roads), 
                new Dictionary<Intersection, Piece>(settlements), 
                index, harbors, allIntersections, allEdges);
        }

        public IBoard PlacePiece(Intersection intersection, Piece p)
        {
            var newSettlements = new Dictionary<Intersection, Piece>(settlements);
            newSettlements[intersection] = p;
            return new Board(terrain, new Dictionary<Edge, int>(roads), newSettlements, robberLocation, harbors, allIntersections, allEdges);
        }
        
        public IBoard PlaceRoad(Edge edge, int playerID)
        {
            var newRoads = new Dictionary<Edge, int>(roads);
            newRoads[edge] = playerID;
            return new Board(terrain, newRoads, new Dictionary<Intersection, Piece>(settlements), robberLocation, harbors, allIntersections, allEdges);
        }

        public Edge[] GetAdjacentEdges(Intersection intersection)
        {
            List<Edge> result = new List<Edge>(3);
            if (IsLegalEdge(new Edge(intersection.FirstTile, intersection.SecondTile))) result.Add(new Edge(intersection.FirstTile,intersection.SecondTile));
            if (IsLegalEdge(new Edge(intersection.SecondTile, intersection.ThirdTile))) result.Add(new Edge(intersection.SecondTile, intersection.ThirdTile));
            if (IsLegalEdge(new Edge(intersection.FirstTile, intersection.ThirdTile))) result.Add(new Edge(intersection.FirstTile, intersection.ThirdTile));
            return result.ToArray();
        }

        public Intersection[] GetAdjacentIntersections(Edge edge)
        {
            var n1 = GetAdjacentTiles(edge.FirstTile);
            return GetAdjacentTiles(edge.SecondTile)
                .Where(t => n1.Contains(t))
                .Where(t => IsLegalEdge(edge) || GetTile(t).Terrain != Terrain.Water)
                .Select(t => new Intersection(edge.FirstTile, edge.SecondTile, t))
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

        public bool HasNoNeighbors(Intersection intersection)
        {
            return GetAdjacentEdges(intersection).
                SelectMany(edge => GetAdjacentIntersections(edge)).
                All(inter => inter.Equals(intersection) || GetPiece(inter) == null);
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
                var corners = GetAdjacentIntersections(h.Position);
                foreach (Intersection pos in corners)
                {
                    Piece curPiece = GetPiece(pos);
                    if (curPiece != null && curPiece.Player == playerID) result.Add(h.Type);
                }
            }
            return result.ToArray();
        }

        public Edge[] GetPossibleRoads(int playerID)
        {
            List<Edge> result = new List<Edge>(15);
            foreach(Edge edge in this.GetAllEdges()){
                // must be empty
                if (roads.ContainsKey(edge)) continue;


                foreach (Intersection inter in this.GetAdjacentIntersections(edge))
                {
                    // must not be other players piece at intersection
                    Piece atInter = this.GetPiece(inter);
                    if (atInter != null && atInter.Player != playerID) continue;

                    // must have one of the players roads connected to one of the end edges
                    if(this.GetAdjacentEdges(inter).Any(e => this.GetRoad(e) == playerID))
                    {
                        result.Add(edge);
                        break;
                    }  
                }
            }
            return result.ToArray();
        }

        public Intersection[] GetPossibleSettlements(int playerID)
        {
            return new HashSet<Intersection>(this.roads.Keys.Where(k => roads[k] == playerID)
                .SelectMany(e => this.GetAdjacentIntersections(e)))
                .Where(i => HasNoNeighbors(i) 
                    && !this.settlements.ContainsKey(i)).ToArray();
        }

        public Intersection[] GetPossibleCities(int playerID)
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
            terrainPool.Shuffle(terrainSeed);
            harborPool.Shuffle(terrainSeed);
            if (randomNumbers) numberPool.Shuffle(numberSeed);

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

        private void AddPiece(List<Piece> list, Intersection intersection)
        {
            Piece hit = this.GetPiece(intersection);
            if(hit != null) list.Add(hit);
        }

        private int CountRoadLengthFromIntersection(int playerID, Intersection curInt, HashSet<Edge> visited, HashSet<Edge> globalVisited)
        {
            // check for break
            Piece curPiece = GetPiece(curInt);
            if(curPiece != null && curPiece.Player != playerID) return 0; // no more edges this direction

            // find connections
            var edgesOut = GetAdjacentEdges(curInt).Where(e => !visited.Contains(e) && GetRoad(e) == playerID).ToArray(); // temp array

            int highest = 0;
            foreach (var edge in edgesOut)
            {
                // add edges to visited
                visited.Add(edge);
                globalVisited.Add(edge);

                Intersection otherEnd = GetAdjacentIntersections(edge).First(i => !i.Equals(curInt));
                int depthSearch = CountRoadLengthFromIntersection(playerID, otherEnd, visited, globalVisited);

                if (1 + depthSearch > highest) highest = 1 + depthSearch;

                visited.Remove(edge);
            }

            return highest;
        }

        private Boolean IsLegalEdge(Edge edge)
        {
            return GetTile(edge.FirstTile).Terrain != Terrain.Water || GetTile(edge.SecondTile).Terrain != Terrain.Water;
        }
    }
}
