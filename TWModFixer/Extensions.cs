using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TWModFixer
{
    public static class Extensions
    {
        public static int IndexSelector<T>(this T[] coll, T value)
        {
            var index = Array.IndexOf(coll, value);

            if (index == -1)
                return 100000;

            return index;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> coll)
        {
            return coll == null || coll.Count() == 0;
        }
    }
}
