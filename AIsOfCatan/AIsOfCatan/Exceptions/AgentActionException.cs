using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class AgentActionException : Exception
    {
        public AgentActionException ()
        {
            
        }
        public AgentActionException(string msg) : base(msg)
        {
            
        }
    }
}
