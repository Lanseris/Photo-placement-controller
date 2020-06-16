using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLvlServInfo.Auxiliary.Extensions;

namespace NextLvlServInfo.Auxiliary
{
    public static class Guard
    {
        /// <summary>
        /// Вызывает <typeparamref name="TException"/> с указанным сообщением
        /// когда утверждение верно.
        /// </summary>
        /// <typeparam name="TException">Тип вызываемого исключения.</typeparam>
        /// <param name="assertion"> Условие для проверки. Если true, тогда вызывается исключение <typeparamref name="TException"/>.</param>
        /// <param name="message"> Строка сообщения для вызываемого исключения.</param>
        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion)
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Вызывает <typeparamref name="TException"/> с указанным сообщением
        /// когда утверждение верно.
        /// </summary>
        /// <typeparam name="TException"> Тип вызываемого исключения.</typeparam>
        /// <param name="assertion">Условие для проверки. Если true, тогда вызывается исключение <typeparamref name="TException"/>.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void Against<TException>(Func<bool> assertion, string message) where TException : Exception
        {
            //Execute the lambda and if it evaluates to true then throw the exception.
            if (assertion())
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный экземпляр
        /// не унаследован напрямую от типа <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">Базовый тип для проверки</typeparam>
        /// <param name="instance">Объект для которого проверяется наследование от типа <typeparamref name="TBase"/>.</param>
        /// <param name="paramName">Наименование экемпляра</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void InheritsFrom<TBase>(object instance, string paramName = "", string message = "Экземпляр {0} не унаследован напрямую от типа {1}") // where TBase : Type
        {
            InheritsFrom<TBase>(instance.GetType(), message.Args(paramName, typeof(TBase)));
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный тип
        /// не унаследован напрямую от типа <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">Базовый тип для проверки.</typeparam>
        /// <param name="type">Тип <see cref="Type"/> для проверки унаследованости от типа <typeparamref name="TBase"/>.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void InheritsFrom<TBase>(Type type, string message = "")
        {
            if (type.BaseType != typeof(TBase))
                throw new InvalidOperationException(message.NotEmpty("Тип {0} не наследован напрямую от типа {1}".Args(type, typeof(TBase))));
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный экземпляр
        /// не унаследован от типа <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">Базовый тип для проверки</typeparam>
        /// <param name="instance">Объект для которого проверяется наследование от типа <typeparamref name="TBase"/>.</param>
        /// <param name="paramName">Наименование экемпляра</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsSubclassOf<TBase>(object instance, string paramName = "", string message = "Экземпляр {0} не унаследован от типа {1}")
        {
            if (!instance.GetType().IsSubclassOf(typeof(TBase)))
                throw new InvalidOperationException(message.Args(paramName, typeof(TBase)));
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный экземпляр
        /// не реализует интерфейс <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface">Интерфейс который должен реализовывать эеземпляр.</typeparam>
        /// <param name="instance">Экземпляр, для которого проверяется реазизация интерфейса <typeparamref name="TInterface"/>.</param>
        /// <param name="paramName">Наименование экемпляра</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void Implements<TInterface>(object instance, string paramName = "", string message = "Экземпляр {0} не реализует интерфейс {1}")
        {
            Implements<TInterface>(instance.GetType(), message.Args(paramName, typeof(TInterface)));
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный тип
        /// не реализует интерфейс <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface">Тип интерфейса <paramref name="type"/> который должен быть реализован.</typeparam>
        /// <param name="type">Тип <see cref="Type"/>, для которого проверяется реализация интерфейса <typeparamref name="TInterface"/>.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void Implements<TInterface>(Type type, string message = "")
        {
            if (!typeof(TInterface).IsAssignableFrom(type))
                throw new InvalidOperationException(message.NotEmpty("Тип {0} не реализует интерфейс {1}".Args(type, typeof(TInterface))));
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный экземпляр
        /// не указанного типа.
        /// </summary>
        /// <typeparam name="TType">Тип на соотвествие которому проверяется <paramref name="instance"/>.</typeparam>
        /// <param name="instance">Экземпляр, для которого выполняется проверка.</param>
        /// <param name="paramName">Наименование экемпляра</param>
        /// <param name="message">Сообщение для исключения <see cref="InvalidOperationException"/>.</param>
        public static void TypeOf<TType>(object instance, string paramName = "", string message = "Экземпляр {0} не является типом {1}")
        {
            if (!(instance is TType))
                throw new InvalidOperationException(message.Args(paramName, typeof(TType)));
        }

        /// <summary>
        /// Вызывает исключение типа <typeparamref name="TException"/>, когда экземпляр объекта не равен другому экземпляру объекта.
        /// </summary>
        /// <typeparam name="TException">Тип вызываемого исключения.</typeparam>
        /// <param name="compare">Сравниваемый экземпляр объекта </param>
        /// <param name="instance">Экземпляр объекта, с которым сравнивается.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsEqual<TException>(object compare, object instance, string message) where TException : Exception
        {
            if (compare == instance)
            {
                return;
            }

            var notNullMember = compare != null ? compare : instance;
            var otherMember = compare != notNullMember ? compare : instance;
            if (notNullMember.Equals(otherMember))
            {
                return;
            }

            throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Вызывает <see cref="Exception"/>, когда экземпляр объекта не равен другому экземпляру объекта.
        /// </summary>
        /// <param name="compare">Сравниваемый экземпляр объекта </param>
        /// <param name="instance">Экземпляр объекта, с которым сравнивается.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsEqual(object compare, object instance, string message)
        {
            IsEqual<Exception>(compare, instance, message);
        }

        /// <summary>
        /// Вызывает <see cref="ArgumentNullException"/>, когда экземпляр объекта является значением null.
        /// </summary>
        /// <param name="instance">Экземпляр объекта.</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsNotNull(object instance, string paramName = "", string message = "Экземпляр является значением null")
        {
            if (ReferenceEquals(instance, null))
                throw new ArgumentNullException(paramName, message);
        }

        /// <summary>
        /// Вызывает <see cref="ArgumentNullException"/>, когда заданная строка является значением null, пустой строкой или строкой, состоящей только из пробельных символов.
        /// </summary>
        /// <param name="value">Заданная строка.</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsNotEmpty(string value, string paramName = "", string message = "Строка не заполнена")
        {
            if (value.IsEmpty())
                throw new ArgumentNullException(paramName, message);
        }

        /// <summary>
        /// Вызывает <see cref="ArgumentNullException"/>, когда заданный идентификатор является значением null или Guid.Empty.
        /// </summary>
        /// <param name="value">Заданный идентификатор.</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsNotEmpty(Guid? value, string paramName = "", string message = "Пустой идентификатор")
        {
            if (value == null || value == Guid.Empty)
                throw new ArgumentNullException(paramName, message);
        }

        /// <summary>
        /// Вызывает <see cref="ArgumentNullException"/>, когда <param name="value"/> является значением null или не содержит элементы.
        /// </summary>
        /// <typeparam name="T">Тип элемента коллекции.</typeparam>
        /// <param name="value">Заданная коллекция.</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsNotEmpty<T>(IEnumerable<T> value, string paramName = "", string message = "Коллекция не содержит элементы")
        {
            if (value.IsNullOrEmpty())
                throw new ArgumentNullException(paramName, message);
        }


        /// <summary>
        /// Вызывает исключение типа <typeparamref name="TException"/>, когда заданное значение истинное
        /// </summary>
        /// <typeparam name="TException">Тип вызываемого исключения.</typeparam>
        /// <param name="value">Заданное значение</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsFalse<TException>(bool value, string paramName = "", string message = "Истинное значение") where TException : Exception
        {
            IsTrue<TException>(!value, paramName, message);
        }

        /// <summary>
        /// Вызывает <see cref="ArgumentException"/>, когда заданное значение истинное
        /// </summary>
        /// <param name="value">Заданное значение</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsFalse(bool value, string paramName = "", string message = "Истинное значение")
        {
            IsFalse<ArgumentException>(value, paramName, message);
        }

        /// <summary>
        /// Вызывает исключение типа <typeparamref name="TException"/>, когда заданное значение ложное
        /// </summary>
        /// <typeparam name="TException">Тип вызываемого исключения.</typeparam>
        /// <param name="value">Заданное значение</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsTrue<TException>(bool value, string paramName = "", string message = "Ложное значение") where TException : Exception
        {
            if (!value)
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Вызывает <see cref="ArgumentException"/>, когда заданное значение ложное
        /// </summary>
        /// <param name="value">Заданное значение</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsTrue(bool value, string paramName = "", string message = "Ложное значение")
        {
            IsTrue<ArgumentException>(value, paramName, message);
        }


        #region Моё

        /// <summary>
        /// Проверка строки на пустоту или заполненность пустыми символами
        /// </summary>
        /// <param name="value">Заданное значение</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsNullOrWhiteSpace(string value, string paramName = "", string message = "Пустая строка")
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(paramName ,message);
        }

        /// <summary>
        /// Проверка на существование файла
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        public static void FileExistCheck(DirectoryInfo dir, string paramName = "", string message = "Файл не найден")
        {
            if (!dir.Exists)
                throw new FileNotFoundException(message, dir.FullName);
        }


        #endregion
    }
}
