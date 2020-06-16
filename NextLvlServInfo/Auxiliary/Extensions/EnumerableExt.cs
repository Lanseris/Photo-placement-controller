using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLvlServInfo.Auxiliary.Extensions
{
    public static class EnumerableExt
    {
        public static bool IsNotEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source != null && source.Any();
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || !source.Any();
        }
    }
}
