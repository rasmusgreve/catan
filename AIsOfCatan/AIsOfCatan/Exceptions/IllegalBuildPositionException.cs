using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    /// <summary>
    /// Thrown when a build fails because it's in an illegal position
    /// </summary>
    class IllegalBuildPositionException : Exception
    {
        public IllegalBuildPositionException()
        {
        }
        public IllegalBuildPositionException(string message)
            : base(message)
        {
        }
    }
}
