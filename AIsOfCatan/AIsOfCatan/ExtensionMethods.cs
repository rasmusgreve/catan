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

        public static string ToDeepString<T>(this IEnumerable<T> enumerable)
        {
            StringBuilder builder = new StringBuilder("[");
            foreach (var item in enumerable)
            {
                IEnumerable<T> sublist = item as IEnumerable<T>;
                if (sublist != null)
                {
                    builder.Append(sublist.ToDeepString() + "/");
                }
                else
                {
                    builder.Append(item.ToString() + "/");
                }
            }
            builder.Remove(builder.Length - 1, 1); // remove last slash
            builder.Append("]");
            return builder.ToString();
        }
    }
}
