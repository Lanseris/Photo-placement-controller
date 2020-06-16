using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLvlServInfo.Auxiliary.Extensions
{
    public static class ObjectExt
    {
        public static T Of<T>(this object o)
        {
            return (T)o;
        }


        public static bool Is<T>(this object o)
        {
            return o is T;
        }


        public static T As<T>(this object o) where T : class
        {
            return o as T;
        }


        public static void AddUnique<T>(this IList<T> list, T obj)
        {
            if (!list.Contains(obj))
                list.Add(obj);
        }


        public static T Find<T>(this T[] objects, Predicate<T> action)
        {
            return Array.Find<T>(objects, action);
        }

        public static T[] FindAll<T>(this T[] objects, Predicate<T> action)
        {
            return Array.FindAll<T>(objects, action);
        }
    }
}
