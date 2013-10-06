using System;
using System.Collections.Generic;
using AIsOfCatan.API;
namespace AIsOfCatan
{
    /// <summary>
    /// This is the model of a game board of Settlers of Catan with everything that is placed on it. The methods needed for this structure is divided into four categories:
    /// Rule Enforcements: Methods for easy evaluation of certain necessary game statistics like longest road and validity of building positions.
    /// Basic Getters: Methods for observing everything on the board, both static information (tiles, numbers and harbors) and the current status of players' pieces.
    /// Traversal: To ease the traversal of the board for any agent with methods for getting the intersections at the ends of an edge, and the edges going from an intersection.
    /// Board Manipulation: The IBoard is immutable so all methods for placing pieces, roads etc. should return a new copy with the wanted change.
    /// </summary>
    public interface IBoard
    {
        #region Rule Enforcement

        /// <summary>
        /// Tells if a specific intersection is free for building.
        /// </summary>
        /// <param name="intersection">The intersection to look at.</param>
        /// <returns>True if the intersections if free, else false.</returns>
        bool CanBuildPiece(Intersection intersection);

        /// <summary>
        /// Tells if a specific edge contains no road.
        /// </summary>
        /// <param name="index1">The edge to look at.</param>
        /// <returns>True if there is no road on the given edge, else false.</returns>
        bool CanBuildRoad(Edge edge);

        /// <summary>
        /// Get the longest road on this board
        /// </summary>
        /// <returns>Dictionary of playerID -> longest road length of that player</returns>
        Dictionary<int, int> GetLongestRoad();

        /// <summary>
        /// Gets the length of the given player's longest road.
        /// </summary>
        /// <param name="playerID">The player's ID.</param>
        /// <returns>The length of the player's longest road.</returns>
        int GetPlayersLongestRoad(int playerID);

        /// <summary>
        /// Checks if the given intersection has no pieces build at the
        /// directly connected intersections (Distance Rule).
        /// </summary>
        /// <param name="index1">The intersection to check for.</param>
        /// <returns>Returns true if the given intersection has no direct neighboring intersections
        /// containing settlements or cities, else false.</returns>
        bool HasNoNeighbors(Intersection intersection);

        /// <summary>
        /// Finds all possible edges where the given player can legally build roads.
        /// </summary>
        /// <param name="playerID">The player to find edges for.</param>
        /// <returns>An array of edges where the given player can legally build a road.</returns>
        Edge[] GetPossibleRoads(int playerID);

        /// <summary>
        /// Finds all possible intersections where the given player can legally build
        /// a settlement (i.e it is currently unoccupied).
        /// </summary>
        /// <param name="playerID">The player to find intersections for.</param>
        /// <returns>An array of intersections where the given player can legally
        /// build a settlement.</returns>
        Intersection[] GetPossibleSettlements(int playerID);

        /// <summary>
        /// Finds all possible intersections where the given player has a settlement.
        /// </summary>
        /// <param name="playerID">The player to find intersections for.</param>
        /// <returns>An array of intersections where the given player can legally
        /// upgrade a settlement to a city.</returns>
        Intersection[] GetPossibleCities(int playerID);

        #endregion

        #region Basic Getters

        /// <summary>
        /// Gives the type of terrain and dice value for a given index of the board.
        /// </summary>
        /// <param name="index">The index to check terrain for.</param>
        /// <returns>A Tile object containing the terrain type and dice value.</returns>
        Tile GetTile(int index);

        /// <summary>
        /// Give the type of terrain and dice value for the given coordinates of the board.
        /// </summary>
        /// <param name="row">The row of the tile.</param>
        /// <param name="column">The column of the tile. Even row numbers have a length
        /// of 6 and uneven has a length of 7.</param>
        /// <returns>A Tile object containing the terrain type and dice value.</returns>
        Tile GetTile(int row, int column);

        /// <summary>
        /// Gives a list of all legal intersections on the board. If this method
        /// proves too slow it should be modified to only calculate the intersections
        /// once.
        /// </summary>
        /// <returns>An array containing all intersections entirely or partly on land.</returns>
        Intersection[] GetAllIntersections();

        /// <summary>
        /// Gives a list of all legal edges on the board.
        /// </summary>
        /// <returns>An array containing all edges around land tiles.</returns>
        Edge[] GetAllEdges();

        /// <summary>
        /// Gives the game piece at the intersection between three different tiles.
        /// </summary>
        /// <param name="intersection">The intersection on the board to look.</param>
        /// <returns>The Piece object at the location, containing the type of token 
        /// (Settlement or City) and the owning player id. If the location is empty
        /// it will return null.</returns>
        Piece GetPiece(Intersection intersection);

        /// <summary>
        /// Get all pieces built adjacent to the given tile index.
        /// </summary>
        /// <param name="index">The location of the tile.</param>
        /// <returns>A list of all the valid Pieces.</returns>
        Piece[] GetPieces(int index);

        /// <summary>
        /// Gives a (copy) of the dictionary holding all settlements and cities 
        /// currently build on the board.
        /// </summary>
        /// <returns>A dictionary with all settlements and cities on the board.</returns>
        Dictionary<Intersection, Piece> GetAllPieces();

        /// <summary>
        /// Gives the id of the player who has build a road at the requested edge.
        /// </summary>
        /// <param name="edge">The edge where the road should be.</param>
        /// <returns>The player id of the player who has build a road here. If empty it returns -1.</returns>
        int GetRoad(Edge edge);

        /// <summary>
        /// Gives a (copy) of the dictionary holding all roads currently build on the board.
        /// </summary>
        /// <returns>A dictionary with all roads on the board.</returns>
        Dictionary<Edge, int> GetAllRoads();

        /// <summary>
        /// Gives an array (size 9) of Harbors containing positions (edges) and
        /// HarborType's for those positions.
        /// </summary>
        /// <returns>The array of Harbors on the board.</returns>
        Harbor[] GetHarbors();

        /// <summary>
        /// Gives an array of Harbors that the given player has a settlement or
        /// city adjacent to.
        /// </summary>
        /// <param name="playerID">The player's ID.</param>
        /// <returns>An array of (unique) HarborTypes that the given player has.</returns>
        HarborType[] GetPlayersHarbors(int playerID);

        /// <summary>
        /// Gives the current location of the Robber token.
        /// </summary>
        /// <returns>The index on the board currently containing the robber.</returns>
        int GetRobberLocation();

        #endregion

        #region Traversal

        /// <summary>
        /// Gives all edges (places to build roads) adjacent to the given intersection. Edges
        /// between two water tiles are excluded.
        /// </summary>
        /// <param name="intersection">The intersection.</param>
        /// <returns>An array of edges with the (up to three) edges next to the intersection.</returns>
        Edge[] GetAdjacentEdges(Intersection intersection);

        /// <summary>
        /// Gives all intersections (places to build settlements and cities)
        /// adjacent to the give edge.
        /// </summary>
        /// <param name="edge">The edge to look at.</param>
        /// <returns>An array of 3-int-tuples with the (up to two) intersections at the ends of the edge.</returns>
        Intersection[] GetAdjacentIntersections(Edge edge);

        /// <summary>
        /// Gets all tiles that are adjacent to the given tile.
        /// </summary>
        /// <param name="index">The tile to look around.</param>
        /// <returns>A list of all the (legal) adjacent tiles.</returns>
        List<int> GetAdjacentTiles(int index);

        #endregion

        #region Board Manipulation

        /// <summary>
        /// Gives the resulting Board from moving the robber to
        /// the specified location.
        /// </summary>
        /// <param name="index">The index on the board to move the
        /// robber.</param>
        /// <returns>The resulting board.</returns>
        IBoard MoveRobber(int index);

        /// <summary>
        /// Places the given Piece on the specified position on the Board and
        /// returns the resulting Board.
        /// </summary>
        /// <param name="intersection">The intersection to place at.</param>
        /// <param name="p">The Piece to place on the Board.</param>
        /// <returns>The resulting Board from placing the Piece.</returns>
        IBoard PlacePiece(Intersection intersection, Piece p);

        /// <summary>
        /// Places a road on the specified position on the Board and
        /// returns the resulting Board.
        /// </summary>
        /// <param name="edge">The edge to place at.</param>
        /// <param name="playerID">The player who owns the road.</param>
        /// <returns>The resulting Board from placing the road.</returns>
        IBoard PlaceRoad(Edge edge, int playerID);

        #endregion
    }
}
