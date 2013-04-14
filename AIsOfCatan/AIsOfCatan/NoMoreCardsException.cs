using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    /// <summary>
    /// Thrown on attempts to draw cards from an empty pile
    /// </summary>
    class NoMoreCardsException : Exception
    {
        public NoMoreCardsException()
        {
        }
        public NoMoreCardsException(string message)
            : base(message)
        {
        }
    }
}
