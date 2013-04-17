using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public interface IStartActions
    {
        /// <summary>
        /// Build a settlement on the board
        /// If you try to build too close to another building a IllegalBuildPosition is thrown
        /// Must be called before BuildRoad, otherwise an IllegalActionException is thrown
        /// If you try to build more than one settlement an IllegalActionException is thrown
        /// </summary>
        /// <param name="firstTile">The index of the first tile in the intersection</param>
        /// <param name="secondTile">The index of the second tile in the intersection</param>
        /// <param name="thirdTile">The index of the third tile in the intersection</param>
        void BuildSettlement(int firstTile, int secondTile, int thirdTile);

        /// <summary>
        /// Build a road on the board
        /// If you try to build at a position not connected to the newly placed settlement an IllegalBuildPositionException is thrown
        /// If you try to build more than one road an IllegalActionException is thrown
        /// </summary>
        /// <param name="firstTile">The first tile that the road will be along</param>
        /// <param name="secondTile">The second tile that the road will be along</param>
        void BuildRoad(int firstTile, int secondTile);

    }
}
