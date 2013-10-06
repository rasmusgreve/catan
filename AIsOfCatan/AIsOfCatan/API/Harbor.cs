using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.API
{
    public class Harbor
    {
        public HarborType Type { get; private set; }
        public Tuple<int, int> Position { get; private set; }

        public Harbor(HarborType type, Tuple<int, int> position)
        {
            this.Type = type;
            this.Position = position;
        }
    }
}
