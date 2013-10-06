using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.API
{
    public class Harbor
    {
        public HarborType Type { get; private set; }
        public Edge Position { get; private set; }

        public Harbor(HarborType type, Edge position)
        {
            this.Type = type;
            this.Position = position;
        }
    }
}
