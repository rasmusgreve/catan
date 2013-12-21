using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class ReceiveResourcesLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public Resource[] Resources { get; private set; }

        public ReceiveResourcesLogEvent(Resource[] resources, int playerId)
        {
            this.Player = playerId;
            this.Resources = resources;
        }

        public override string ToString()
        {
            return "Player " + Player + " received " + Resources.ToListString();
        }
    }
}
