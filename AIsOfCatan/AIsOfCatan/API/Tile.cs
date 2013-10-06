using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.API
{
    public class Tile
    {
        public Terrain Terrain { get; private set; }
        public int Value { get; internal set; }

        public Tile(Terrain terrain, int value)
        {
            this.Terrain = terrain;
            this.Value = value;
        }

        public override string ToString()
        {
            return "[" + Terrain.ToString() + " : " + Value + "]";
        }
    }
}
