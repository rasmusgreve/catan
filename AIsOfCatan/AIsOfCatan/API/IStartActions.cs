using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.API;

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
        /// <param name="intersection">The intersection to build at.</param>
        void BuildSettlement(Intersection intersection);

        /// <summary>
        /// Build a road on the board
        /// If you try to build at a position not connected to the newly placed settlement an IllegalBuildPositionException is thrown
        /// If you try to build more than one road an IllegalActionException is thrown
        /// </summary>
        /// <param name="edge">The edge the road will be along.</param>
        void BuildRoad(Edge edge);

    }
}
