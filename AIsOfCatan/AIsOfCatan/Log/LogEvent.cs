using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    public class LogEvent
    {
        private DateTime time = DateTime.Now;
        public DateTime TimeStamp { get { return time; } }
    }
}
