using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public interface GameActions
    {
         DevelopmentCard DrawDevelopmentCard();

         void PlayKnight();

         void PlayRoadBuilding(int firstTile1, int secondTile1, int firstTile2, int secondTile2);

         void PlayYearOfPlenty(Resource resource1, Resource resource2);

         GameState PlayMonopoly(Resource resource);

         Trade[] ProposeTrade(Trade trade);

         void Trade(int otherPlayer);

         bool BuildHouse(int firstTile, int secondTile, int thirdTile);

         bool BuildCity(int firstTile, int secondTile, int thirdTile);

         bool BuildRoad(int firstTile, int secondTile);

    }
}
