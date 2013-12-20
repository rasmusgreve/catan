using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.Log;

namespace AIsOfCatan
{
    //                     19 of each in bank
    public enum Resource { Brick, Lumber, Wool, Grain, Ore }; // names from wikipedia
    //                   3      4       4        4       3          1       X
    public enum Terrain { Hills, Forest, Pasture, Fields, Mountains, Desert, Water }
    //                            14      5             2             2             2
    public enum DevelopmentCard { Knight, VictoryPoint, RoadBuilding, YearOfPlenty, Monopoly }

    public enum Token { Settlement, City };

    public enum HarborType { Brick, Lumber, Wool, Grain, Ore, ThreeForOne };

    //----------------------------------------------------------------------------------------//

    public interface IGameState
    {
        /// <summary>
        /// 
        /// </summary>
        IBoard Board { get; }

        /// <summary>
        /// 
        /// </summary>
        int DevelopmentCards { get; }

        /// <summary>
        /// 
        /// </summary>
        int[] ResourceBank { get; }

        /// <summary>
        /// 
        /// </summary>
        int[] AllPlayerIds { get; }

        /// <summary>
        /// 
        /// </summary>
        int LongestRoadId { get; }

        /// <summary>
        /// 
        /// </summary>
        int LargestArmyId { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        int GetPlayerScore(int playerId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetRoundNumber();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        int GetResourceCount(int playerID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        int GetDevelopmentCardCount(int playerID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        int GetKnightCount(int playerID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        int GetSettlementsLeft(int playerID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        int GetCitiesLeft(int playerID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        int GetRoadsLeft(int playerID);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Resource[] GetOwnResources();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DevelopmentCard[] GetOwnDevelopmentCards();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        int GetResourceBank(Resource res);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        List<LogEvent> GetLatestEvents(int amount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        List<LogEvent> GetEventsSince(DateTime time);

    }
}
