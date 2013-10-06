using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.API
{
    public class Edge
    {
        public int FirstTile { get; private set; }
        public int SecondTile { get; private set; }

        public Edge(int first, int second)
        {
            FirstTile = first;
            SecondTile = second;
        }
    }
}
