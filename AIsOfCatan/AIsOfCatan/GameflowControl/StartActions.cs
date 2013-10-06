using System.Linq;

namespace AIsOfCatan
{
    public class StartActions : IStartActions
    {
        private readonly Player player;
        private readonly GameController controller;
        private bool settlementBuilt = false;
        private bool roadBuilt = false;
        private int[] settlementPosition;
        private int[] roadPosition;
        public StartActions(Player player, GameController controller)
        {
            this.player = player;
            this.controller = controller;
        }

        /// <summary>
        /// Returns whether the start action has been completed
        /// </summary>
        public bool IsComplete()
        {
            return roadBuilt && settlementBuilt;
        }

        /// <summary>
        /// Internal method used for handing out resources
        /// </summary>
        public int[] GetSettlementPosition()
        {
            return settlementPosition.ToArray();
        }

        /// <summary>
        /// Internal method used for handing out resources
        /// </summary>
        public int[] GetRoadPosition()
        {
            return roadPosition.ToArray();
        }


        /// <summary>
        /// Build a settlement on the board
        /// If you try to build too close to another building a IllegalBuildPosition is thrown
        /// Must be called before BuildRoad, otherwise an IllegalActionException is thrown
        /// </summary>
        /// <param name="firstTile">The index of the first tile in the intersection</param>
        /// <param name="secondTile">The index of the second tile in the intersection</param>
        /// <param name="thirdTile">The index of the third tile in the intersection</param>
        public void BuildSettlement(int firstTile, int secondTile, int thirdTile)
        {
            if (settlementBuilt) throw new IllegalActionException("Only one settlement may be built in a turn during the startup");
            settlementPosition = new int[] { firstTile, secondTile, thirdTile};
            controller.BuildFirstSettlement(player, firstTile, secondTile, thirdTile);
            settlementBuilt = true;
        }

        /// <summary>
        /// Build a road on the board
        /// If you try to build at a position not connected to the newly placed settlement an IllegalBuildPositionException is thrown
        /// If you try to build more than one road an IllegalActionException is thrown
        /// </summary>
        /// <param name="firstTile">The first tile that the road will be along</param>
        /// <param name="secondTile">The second tile that the road will be along</param>
        public void BuildRoad(int firstTile, int secondTile)
        {
            if (roadBuilt) throw new IllegalActionException("Only one road may be built in a turn during the startup");
            if (!settlementBuilt) throw new IllegalActionException("The settlement must be placed before the road");
            if (!(settlementPosition.Contains(firstTile) && settlementPosition.Contains(secondTile)))
                throw new IllegalBuildPositionException("The road must be placed next to the settlement");
            roadPosition = new int[] { firstTile, secondTile };
            controller.BuildFirstRoad(player, firstTile,secondTile);
            roadBuilt = true;
        }
    }
}
