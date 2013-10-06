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
            FirstTile = first < second ? first : second;
            SecondTile = first < second ? second : first;
        }

        public int[] ToArray()
        {
            return new int[] { FirstTile, SecondTile};
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Edge)) return false;
            Edge that = (Edge)obj;
            return this.FirstTile == that.FirstTile && this.SecondTile == that.SecondTile;
        }

        public override int GetHashCode()
        {
            return FirstTile << 16 | SecondTile;
        }

        public override string ToString()
        {
            return "[" + FirstTile + "," + SecondTile + "]";
        }
    }
}
