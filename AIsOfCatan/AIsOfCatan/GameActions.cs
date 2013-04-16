using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public interface GameActions
    {

        GameState PlayKnight();

        GameState PlayRoadBuilding(int firstTile1, int secondTile1, int firstTile2, int secondTile2);

        GameState PlayYearOfPlenty(Resource resource1, Resource resource2);

        GameState PlayMonopoly(Resource resource);

        Dictionary<int, Trade> ProposeTrade(Trade trade);

        GameState Trade(int otherPlayer);

        DevelopmentCard DrawDevelopmentCard();

        GameState BuildSettlement(int firstTile, int secondTile, int thirdTile);

        GameState BuildCity(int firstTile, int secondTile, int thirdTile);

        GameState BuildRoad(int firstTile, int secondTile);

    }
}
