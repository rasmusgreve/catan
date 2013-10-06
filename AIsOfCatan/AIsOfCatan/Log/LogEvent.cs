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
       
        protected static string ListToString<T>(T variable)
        {

            var enumerable = variable as System.Collections.IEnumerable;

            if (enumerable != null)
            {
                StringBuilder builder = new StringBuilder("[");
                foreach (var item in enumerable)
                {
                    builder.Append(ListToString(item) + "/");
                }
                builder.Remove(builder.Length - 1, 1); // remove last slash
                builder.Append("]");
                return builder.ToString();
            }
            else
            {
                return variable.ToString();
            }
        }
    }
}
