using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    /// <summary>
    /// Thrown when a player tries to perform an illegal action on the GameController
    /// </summary>
    class IllegalActionException : Exception
    {
        public IllegalActionException()
        {
        }
        public IllegalActionException(string message)
            : base(message)
        {
        }
    }
}
