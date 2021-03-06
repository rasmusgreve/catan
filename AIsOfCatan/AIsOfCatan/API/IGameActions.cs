﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.API;

namespace AIsOfCatan
{
    public interface IGameActions
    {
        /// <summary>
        /// Play a knight development card
        /// This allows you to move the robber to another tile and draw a card from an opponent with a building on the tile
        /// If you don't have a knight on your hand a InsufficientResourcesException is thrown
        /// A knight is automatically removed from your hand
        /// The largest army special card is relocated if playing this knight causes it to be
        /// Notice! You may only play one development card in each turn. This includes both the BeforeDiceRoll and PerformTurn methods
        /// </summary>
        /// <returns>The new state of the game after the robber has been moved and resources changed hands</returns>
        GameState PlayKnight();

        /// <summary>
        /// Play a road building development card
        /// This allows you to build two roads free of charge
        /// If you don't have a RoadBuilding card on your hand a InsufficientResourcesException is thrown
        /// If you don't have any road pieces left a IllegalActionException is thrown
        /// If you try to place a road at a position where a road is already present a IllegalBuildPositionException is thrown
        /// If you only have one road piece left, the position to place it must be passed as firstTile1, secondTile1 (the others are ignored)
        /// Notice! You may only play one development card in each turn. This includes both the BeforeDiceRoll and PerformTurn methods
        /// </summary>
        /// <param name="firstEdge">The first edge to build a road along.</param>
        /// <param name="firstEdge">The second edge to build a road along.</param>
        /// <returns>The new state of the game after the roads are built</returns>
        GameState PlayRoadBuilding(Edge firstEdge, Edge secondEdge);

        /// <summary>
        /// Play a year of plenty development card
        /// This allows you to draw two resources cards of your own choice from the bank 
        /// If you don't have a YearOfPlenty card on your hand a InsufficientResourcesException is thrown
        /// If the resource bank doesn't have enough cards to fulfill the request a NoMoreCardsException is thrown
        /// Notice! You may only play one development card in each turn. This includes both the BeforeDiceRoll and PerformTurn methods
        /// </summary>
        /// <param name="resource1">The type of resource for the first card</param>
        /// <param name="resource2">The type of resource for the second card</param>
        /// /// <returns>The new state of the game after the resources have been drawn</returns>
        GameState PlayYearOfPlenty(Resource resource1, Resource resource2);

        /// <summary>
        /// Play a Monopoly development card
        /// This will make all players give all their resources of the type you choose to you
        /// If you don't have a Monopoly card on your hand a InsufficientResourcesException is thrown
        /// All resources of the given type is removed from players hands and all given to the playing player
        /// Notice! You may only play one development card in each turn. This includes both the BeforeDiceRoll and PerformTurn methods
        /// </summary>
        /// <param name="resource">The resource to get monopoly on</param>
        /// <returns>The new state of the game after the resources have changed hands</returns>
        GameState PlayMonopoly(Resource resource);

        /// <summary>
        /// Propose a trade with the other players
        /// </summary>
        /// <param name="give">A list of possible combinations of resources you want to trade away</param>
        /// <param name="take">A list of possible combinations of resources you want to get in return</param>
        /// <returns>A dictionary of PlayerID => ITrade telling what the other players responded. 
        /// If you wish to complete a trade call Trade(otherPlayer) with the id of the player you wish to trade with</returns>
        Dictionary<int, ITrade> ProposeTrade(List<List<Resource>> give, List<List<Resource>> take);

        /// <summary>
        /// Complete a proposed trade with a given player
        /// </summary>
        /// <param name="otherPlayer">The id of the player you wish to trade with</param>
        /// <returns>The updated game state after trading</returns>
        GameState Trade(int otherPlayer);

        /// <summary>
        /// Trades resources with the bank for the specific wanted resource at fixed rates.
        /// The player needs to have 4 of the resource (3 if he has a 3for1 harbor or 2 if he has the specific resource's harbor)
        /// in order for the trade to be valid.
        /// If you don't have enough resources a InsufficientResourcesException is thrown
        /// </summary>
        /// <param name="giving">The resource to pay</param>
        /// <param name="receiving">The resource to receive</param>
        /// <returns>The new state of the game after the resources have been paid and received</returns>
        GameState TradeBank(Resource giving, Resource receiving);

        /// <summary>
        /// Draw a development card from the pile at the cost of (1 x Grain, 1 x Wool, 1 x Ore)
        /// If you don't have enough resources a InsufficientResourcesException is thrown
        /// If the development card stack is empty a NoMoreCardsException is thrown
        /// Resources to pay for the card are removed from your hand and returned to the resource bank
        /// </summary>
        /// <returns>The drawn development card</returns>
        GameState DrawDevelopmentCard();

        /// <summary>
        /// Build a settlement on the board at the cost of (1 x Lumber, 1 x Brick, 1 x Wool, 1 x Grain)
        /// If you don't have enough resources to build a settlement a InsufficientResourcesException is thrown
        /// If you try to build too close to another building, or not connected to one of your roads a IllegalBuildPosition is thrown
        /// If you don't have any more settlement pieces left to place a IllegalActionException is thrown
        /// The required resources are taken from your hand and placed back at the resource bank
        /// If the settlement is placed at a harbor, the harbor can be used immediately (rules p. 7 - footnote 12)
        /// </summary>
        /// <param name="intersection">The intersection to build at</param>
        /// <returns>The state of the game after the settlement has been placed</returns>
        GameState BuildSettlement(Intersection intersection);

        /// <summary>
        /// Upgrade an existing settlement to a city at the cost of (2 x Grain, 3 x Ore)
        /// If you don't have enough resources to build a city a InsufficientResourcesException is thrown
        /// If you try to build at a position where you don't have a settlement a IllegalBuildPosition is thrown
        /// If you don't have any more city pieces left to place a IllegalActionException is thrown
        /// The required resources are taken from your hand and placed back at the resource bank
        /// The settlement previously on the location is given back to you and can be placed again later
        /// </summary>
        /// <param name="intersection">The intersection to upgrade at</param>
        /// <returns>The state of the game after the settlement has been upgraded to a city</returns>
        GameState BuildCity(Intersection intersection);

        /// <summary>
        /// Build a road on the board at the cost of (1 x Lumber, 1 x Brick)
        /// If you don't have enough resources to build a road an InsufficientResourcesException is thrown
        /// If you try to build at a position not connected to another or your roads, settlements or cities an IllegalBuildPositionException is thrown
        /// If you don't have any more road pieces left to place an IllegalActionException is thrown
        /// </summary>
        /// <param name="edge">The edge to build a road at</param>
        /// <returns>The state of the game after the road has been built</returns>
        GameState BuildRoad(Edge edge);
    }
}
