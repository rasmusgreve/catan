using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.API
{
    public class Intersection
    {
        public int FirstTile { get; private set; }
        public int SecondTile { get; private set; }
        public int ThirdTile { get; private set; }

        public Intersection(int first, int second, int third)
        {
            FirstTile = first;
            SecondTile = second;
            ThirdTile = third;
        }
    }
}
