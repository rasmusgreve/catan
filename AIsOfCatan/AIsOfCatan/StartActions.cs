using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public class StartActions
    {
        private Player player;
        private GameController controller;
        private bool houseBuilt = false;
        private bool roadBuilt = false;
        private int[] housePosition;
        public StartActions(Player player, GameController controller)
        {
            this.player = player;
            this.controller = controller;
        }

        /// <summary>
        /// Internal method used for handing out resources
        /// </summary>
        public int[] GetHousePosition()
        {
            return housePosition.ToArray();
        }

        public bool BuildHouse(int firstTile, int secondTile, int thirdTile)
        {
            if (houseBuilt) throw new IllegalActionException("Only one house may be built in a turn during the startup");
            housePosition = new int[] { firstTile, secondTile, thirdTile };

            throw new NotImplementedException();
        }

        public bool BuildRoad(int firstTile, int secondTile)
        {
            if (roadBuilt) throw new IllegalActionException("Only one road may be built in a turn during the startup");
            if (!houseBuilt) throw new IllegalActionException("The house must be placed before the road");
            if (!(housePosition.Contains(firstTile) && housePosition.Contains(secondTile)))
                throw new IllegalBuildPositionException("The road must be placed next to the house");

            throw new NotImplementedException();
        }
    }
}
