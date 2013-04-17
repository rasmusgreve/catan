
using System.Collections.Generic;

namespace AIsOfCatan
{
    public class MainActions : IGameActions
    {
        private readonly Player player;
        private readonly GameController controller;
        private bool valid;
        private bool hasPlayedDevCard;
        private bool isAfterDieRoll;
        public MainActions(Player player, GameController controller)
        {
            this.player = player;
            this.controller = controller;
            valid = true;
            this.hasPlayedDevCard = false;
            this.isAfterDieRoll = false;
        }

        /// <summary>
        /// Allow the build actions to be called
        /// </summary>
        public void DieRoll()
        {
            isAfterDieRoll = false;
        }

        /// <summary>
        /// Invalidate this action object making all methods throw IllegalActionExceptions if called
        /// </summary>
        public void Invalidate()
        {
            valid = false;
        }

        //Development cards

        public GameState DrawDevelopmentCard()
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to draw developmentcard before the die roll");
            return controller.DrawDevelopmentCard(player);
        }

        public GameState PlayKnight()
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (hasPlayedDevCard)
                throw new IllegalActionException("Max one development card can be played each turn");
            hasPlayedDevCard = true;
            return controller.PlayKnight(player);
        }

        public GameState PlayRoadBuilding(int firstTile1, int secondTile1, int firstTile2, int secondTile2)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (hasPlayedDevCard)
                throw new IllegalActionException("Max one development card can be played each turn");
            hasPlayedDevCard = true;
            return controller.PlayRoadBuilding(player, firstTile1, secondTile1, firstTile2, secondTile2);
        }

        public GameState PlayYearOfPlenty(Resource resource1, Resource resource2)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (hasPlayedDevCard)
                throw new IllegalActionException("Max one development card can be played each turn");
            hasPlayedDevCard = true;
            return controller.PlayYearOfPlenty(player, resource1, resource2);
        }

        public GameState PlayMonopoly(Resource resource)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (hasPlayedDevCard)
                throw new IllegalActionException("Max one development card can be played each turn");
            hasPlayedDevCard = true;
            return controller.PlayMonopoly(player, resource);
        }

        //Trading

        public Dictionary<int, Trade> ProposeTrade(Trade trade)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to propose trade before the die roll");
            return controller.ProposeTrade(player, trade);
        }

        public GameState Trade(int otherPlayer)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to trade before the die roll");
            return controller.CompleteTrade(player, otherPlayer);
        }

        //Building

        public GameState BuildSettlement(int firstTile, int secondTile, int thirdTile)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to build before the die roll");
            return controller.BuildSettlement(player, firstTile, secondTile, thirdTile);
        }

        public GameState BuildCity(int firstTile, int secondTile, int thirdTile)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to build before the die roll");
            return controller.BuildCity(player, firstTile, secondTile, thirdTile);
        }

        public GameState BuildRoad(int firstTile, int secondTile)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to build before the die roll");
            return controller.BuildRoad(player, firstTile, secondTile);
        }
    }
}
