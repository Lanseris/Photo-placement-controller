using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLvlServInfo.Auxiliary
{
    public static class DictionaryHelper
    {
        public static void AddToDictionary<T, E>(Dictionary<T, E> dictionary, KeyValuePair<T, E> itemForAdd, Func<E, E, E> mergeFunc)
        {
            if (dictionary.ContainsKey(itemForAdd.Key))
            {
                dictionary[itemForAdd.Key] = mergeFunc(dictionary[itemForAdd.Key], itemForAdd.Value);
            }
            else
            {
                dictionary.Add(itemForAdd.Key, itemForAdd.Value);
            }
        }
    }
}
