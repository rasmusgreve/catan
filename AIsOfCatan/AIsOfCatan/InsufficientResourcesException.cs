using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    /// <summary>
    /// Thrown when a player tries to buy something but haven't got sufficient resources
    /// </summary>
    class InsufficientResourcesException : Exception
    {
        public InsufficientResourcesException()
        {
        }
        public InsufficientResourcesException(string message)
            : base(message)
        {
        }
    }
}
