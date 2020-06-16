using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLvlServInfo.Auxiliary.Extensions
{
    /// <summary>
    /// Методы расширений для базовых типов
    /// </summary>
    public static class CommonExtensions
    {
        #region value IsEmpty, IsNotEmpty

        /// <summary>
        /// 	Определяет, является ли указанное значение пустым.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">Значение.</param>
        /// <returns>
        /// 	<c>true</c> если значение пусто; иначе <c>false</c>.
        /// </returns>
        public static bool IsEmpty<T>(this T value) where T : struct
        {
            return value.Equals(default(T));
        }

        /// <summary>
        /// 	Определяет, является ли указанное значение не пустым.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">Значение.</param>
        /// <returns>
        /// 	<c>true</c> если значение не пусто; иначе, <c>false</c>.
        /// </returns>
        public static bool IsNotEmpty<T>(this T value) where T : struct
        {
            return (value.IsEmpty() == false);
        }

        #endregion

        #region T[] IsEmpty, IsNotEmpty

        /// <summary>
        /// Указывает, что массив НЕ содержит элементы.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this T[] source)
        {
            return source == null || source.Length == 0;
        }

        /// <summary>
        /// Указывает, что массив содержит элементы.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this T[] source)
        {
            return !source.IsEmpty();
        }

        #endregion

        /// <summary>
        /// Выполняет метод заданное количество раз
        /// </summary>
        /// <param name="count">Количество выполнений</param>
        /// <param name="action">Метод</param>
        public static void ForEach(this int count, Action<int> action)
        {
            for (var i = 0; i < count; i++)
                action(i);
        }
    }
}
