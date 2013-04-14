using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    interface GameActions
    {
        public DevelopmentCard DrawDevelopmentCard();

        public void PlayKnight();

        public bool PlayRoadBuilding(int firstTile1, int secondTile1, int firstTile2, int secondTile2);

        public void PlayYearOfPlenty(Resource resource1, Resource resource2);

        public GameState PlayMonopoly(Resource resource);

        public Trade[] ProposeTrade(Trade trade);

        public void Trade(int otherPlayer);

        public bool BuildHouse(int firstTile, int secondTile, int thirdTile);

        public bool BuildCity(int firstTile, int secondTile, int thirdTile);

        public bool BuildRoad(int firstTile, int secondTile);

    }
}
