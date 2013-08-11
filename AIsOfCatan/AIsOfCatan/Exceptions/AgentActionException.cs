using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class AgentActionException : Exception
    {
        public bool StopGame { get; private set; }

        public AgentActionException(bool stopGame = false)
        {
            StopGame = stopGame;
        }
        public AgentActionException(string msg, bool stopGame = false)
            : base(msg)
        {
            StopGame = stopGame;
        }
    }
}
