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

        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            Random rng = new Random(seed);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string ToDeepString<T>(this IEnumerable<List<T>> enumerable)
        {
            if (enumerable.Count() < 1) return "[]";

            StringBuilder builder = new StringBuilder("[");
            foreach( List<T> item in enumerable)
            {
                builder.Append(item.ToListString() + "/");
            }
            builder.Remove(builder.Length - 1, 1); // remove last slash
            builder.Append("]");
            return builder.ToString();
        }

        public static string ToListString<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.Count() < 1) return "[]";

            StringBuilder builder = new StringBuilder("[");
            foreach (var item in enumerable)
            {
                builder.Append(item.ToString() + "/");
            }
            builder.Remove(builder.Length - 1, 1); // remove last slash
            builder.Append("]");
            return builder.ToString();
        }
    }
}
