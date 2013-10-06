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
            List<int> tiles = new List<int>(3) { first, second, third };
            tiles.Sort();

            FirstTile = tiles[0];
            SecondTile = tiles[1];
            ThirdTile = tiles[2];
        }

        public int[] ToArray()
        {
            return new int[] { FirstTile, SecondTile, ThirdTile };
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Intersection)) return false;
            Intersection that = (Intersection)obj;
            return this.FirstTile == that.FirstTile && this.SecondTile == that.SecondTile && this.ThirdTile == that.ThirdTile;
        }

        public override int GetHashCode()
        {
            return FirstTile << 16 | SecondTile << 8 | ThirdTile;
        }

        public override string ToString()
        {
            return "[" + FirstTile + "," + SecondTile + "," + ThirdTile + "]";
        }
    }
}
