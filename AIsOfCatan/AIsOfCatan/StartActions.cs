using System.Linq;

namespace AIsOfCatan
{
    public class StartActions
    {
        private readonly Player player;
        private readonly GameController controller;
        private bool settlementBuilt = false;
        private bool roadBuilt = false;
        private int[] settlementPosition;
        public StartActions(Player player, GameController controller)
        {
            this.player = player;
            this.controller = controller;
        }

        /// <summary>
        /// Internal method used for handing out resources
        /// </summary>
        public int[] GetSettlementPosition()
        {
            return settlementPosition.ToArray();
        }

        /// <summary>
        /// IMPORTANT! May only be called once!
        /// Must be called before BuildRoad
        /// </summary>
        /// <param name="firstTile"></param>
        /// <param name="secondTile"></param>
        /// <param name="thirdTile"></param>
        /// <returns></returns>
        public bool BuildSettlement(int firstTile, int secondTile, int thirdTile)
        {
            if (settlementBuilt) throw new IllegalActionException("Only one settlement may be built in a turn during the startup");
            settlementPosition = new int[] { firstTile, secondTile, thirdTile};
            controller.BuildFirstSettlement(player, firstTile, secondTile, thirdTile);
            settlementBuilt = true;
            return true;
        }

        /// <summary>
        /// IMPORTANT! May only be called once!
        /// Must be called after BuildSettlement
        /// </summary>
        /// <param name="firstTile"></param>
        /// <param name="secondTile"></param>
        /// <returns></returns>
        public bool BuildRoad(int firstTile, int secondTile)
        {
            if (roadBuilt) throw new IllegalActionException("Only one road may be built in a turn during the startup");
            if (!settlementBuilt) throw new IllegalActionException("The settlement must be placed before the road");
            if (!(settlementPosition.Contains(firstTile) && settlementPosition.Contains(secondTile)))
                throw new IllegalBuildPositionException("The road must be placed next to the settlement");
            controller.BuildFirstRoad(player, firstTile,secondTile);
            roadBuilt = true;
            return true;
        }
    }
}
