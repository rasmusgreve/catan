using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public static class ExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> list, System.Action<T> action)
        {
            foreach (T item in list)
                action(item);
        }
    }
}
