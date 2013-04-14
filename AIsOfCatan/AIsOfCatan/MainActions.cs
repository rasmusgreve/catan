using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public class MainActions : GameActions
    {
        private Player player;
        private GameController controller;
        private bool valid;
        private bool hasPlayedDevCard;
        private bool isAfterDieRoll;
        public MainActions(Player player, GameController controller, bool hasPlayedDevCard = false, bool isAfterDieRoll = false)
        {
            this.player = player;
            this.controller = controller;
            valid = true;
            this.hasPlayedDevCard = hasPlayedDevCard;
            this.isAfterDieRoll = isAfterDieRoll;
        }

        public void DieRoll()
        {
            isAfterDieRoll = false;
        }

        public void Invalidate()
        {
            valid = false;
        }

        //Development cards

        public DevelopmentCard DrawDevelopmentCard()
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to draw developmentcard before the die roll");
            return controller.DrawDevelopmentCard(player);
        }

        public void PlayKnight()
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (hasPlayedDevCard)
                throw new IllegalActionException("Max one development card can be played each turn");
            hasPlayedDevCard = true;
            controller.PlayKnight(player);
        }

        public bool PlayRoadBuilding(int firstTile1, int secondTile1, int firstTile2, int secondTile2)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (hasPlayedDevCard)
                throw new IllegalActionException("Max one development card can be played each turn");
            hasPlayedDevCard = true;
            return controller.PlayRoadBuilding(player, firstTile1, secondTile1, firstTile2, secondTile2);
        }

        public void PlayYearOfPlenty(Resource resource1, Resource resource2)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (hasPlayedDevCard)
                throw new IllegalActionException("Max one development card can be played each turn");
            hasPlayedDevCard = true;
            controller.PlayYearOfPlenty(player, resource1, resource2);
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

        public Trade[] ProposeTrade(Trade trade)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to propose trade before the die roll");
            return controller.ProposeTrade(player, trade);
        }

        public void Trade(int otherPlayer)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to trade before the die roll");
            controller.CompleteTrade(player, otherPlayer);
        }

        //Building

        public bool BuildHouse(int firstTile, int secondTile, int thirdTile)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to build before the die roll");
            return controller.BuildHouse(player, firstTile, secondTile, thirdTile);
        }

        public bool BuildCity(int firstTile, int secondTile, int thirdTile)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to build before the die roll");
            return controller.BuildCity(player, firstTile, secondTile, thirdTile);
        }

        public bool BuildRoad(int firstTile, int secondTile)
        {
            if (!valid) throw new IllegalActionException("Tried to perform an action on an invalid GameAction");
            if (!isAfterDieRoll) throw new IllegalActionException("Tried to build before the die roll");
            return controller.BuildRoad(player, firstTile, secondTile);
        }
    }
}
