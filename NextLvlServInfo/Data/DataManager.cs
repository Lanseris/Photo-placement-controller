using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLvlServInfo.DataModel;
using NextLvlServInfo.Auxiliary;
using System.IO;
using System.Text.RegularExpressions;

namespace NextLvlServInfo.Data
{
    public class DataManager
    {
        public Dictionary<string, DirectoryModel> DirectoryModelDic { get; set; }

        private string _nameOfCacheFile;

        private List<string> _directoriesPathsList;

        /// <summary>
        /// Конструктор
        /// </summary>
        public DataManager()
        {


            _nameOfCacheFile = "DirectoriesPaths.txt";
            _directoriesPathsList = GetDirectoriesPathsFromFile();

            DirectoryModelDic = new Dictionary<string, DirectoryModel>();

            _fillDirectoryModelDic(_directoriesPathsList);
            
            //TestInit();
        }

        /// <summary>
        /// Загрузка путей к папкам из текстового файла
        /// </summary>
        /// <returns></returns>
        public List<string> GetDirectoriesPathsFromFile()
        {
            List<string> directoriesPathsList = new List<string>();

            if (File.Exists(_nameOfCacheFile))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(_nameOfCacheFile))
                    {
                        string path;
                        List<string> readingErrorList = new List<string>();

                        while (!sr.EndOfStream)
                        {
                            path = sr.ReadLine();
                            if (Directory.Exists(path))
                            {
                                directoriesPathsList.Add(path);
                            }
                            else
                            {
                                //TODO воткнуть сюда запись в логи
                                readingErrorList.Add("Папка "+path+" не найдена.");
                            }
                        }

                    }
                }
                catch (Exception)
                {
                   //TODO сюда тоже логер и бросать ошибку наверх
                    throw;
                }
            }

            return directoriesPathsList;
        }


        /// <summary>
        /// оставил для тестов, поидее не нужен больше
        /// </summary>
        public void TestInit()
        {
            string testPath = @"C:\Users\Vasiliy.Kononov\Pictures";

         DirectoryModel testDirectoryModel = CreateDirectoryModel(testPath, "Изображения");


            var testFileTypeDic = FillDirectoryModel(testDirectoryModel);

            DirectoryModelDic.Add(testDirectoryModel.ShortName, testDirectoryModel);


        }

        /// <summary>
        /// Создание модели дерриктории
        /// </summary>
        /// <param name="path">расположение дирректории</param>
        /// <param name="shortName">краткое название</param>
        /// <returns></returns>
        public DirectoryModel CreateDirectoryModel(string path, string shortName)
        {
            Guard.IsNullOrWhiteSpace(path, nameof(path));

            DirectoryModel directoryModel = new DirectoryModel();

            directoryModel.SelfDirectoryInfo = new DirectoryInfo(path);
            
            if (!directoryModel.SelfDirectoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(path);
            }

            if (!string.IsNullOrWhiteSpace(shortName))
            {
                directoryModel.ShortName = shortName.Trim();
            }
            else
            {
                directoryModel.ShortName = string.Empty;
            }
            return directoryModel;
        }



        /// <summary>
        /// Заполнение модели директории в соответствии с уже заданными в ней параметрами (рекурсия!!!) 
        /// TODO так вроде делать не хорошо, изменять состояние объекта, передаваемого в качестве параметра
        /// </summary>
        /// <param name="directoryModel"></param>
        /// <returns></returns>
        public Dictionary<string, int> FillDirectoryModel(DirectoryModel directoryModel)
        {
            Guard.IsNotNull(directoryModel, nameof(directoryModel));

            directoryModel.FillNumOfTypesCurrDirectory();

            DirectoryModel createdDirectoryModel = null;

            foreach (string directoryFullName in Directory.GetDirectories(directoryModel.SelfDirectoryInfo.FullName))
            {
                createdDirectoryModel = CreateDirectoryModel(directoryFullName, directoryFullName.Split('\\').Last());
                
                directoryModel.DirectoryModelsList.Add(createdDirectoryModel);

                //добавляет количетсво файлов каждого типа из каждой папки
                directoryModel.FoldersEachTypeOfFileNum = DictionaryМerge<string,int>(
                    directoryModel.FoldersEachTypeOfFileNum ,
                    FillDirectoryModel(createdDirectoryModel),
                    directoryModel.Sum
                    );
            }

            directoryModel.AggregateNumOfEachFileType =
                     DictionaryМerge<string, int>(
                         directoryModel.CurrentLvlNumOfEachFileType,
                         directoryModel.FoldersEachTypeOfFileNum,
                         directoryModel.Sum
                         );

            directoryModel.AggregateNumOfFiles = directoryModel.AggregateNumOfEachFileType.Values.Sum();

            return directoryModel?.AggregateNumOfEachFileType??new Dictionary<string, int>();
        }

        /// <summary>
        /// Инициализация, заполнение и добавление в словарь Моделей директорий на основе переданного списка путей к директориям
        /// </summary>
        /// <param name="pathsEnum"></param>
        private void _fillDirectoryModelDic(IEnumerable<string> pathsEnum)
        {
            //Guard.IsNotEmpty<string>(pathsEnum,nameof(pathsEnum));

            DirectoryModel directoryModel;

            foreach (var path in pathsEnum)
            {
                //TODO отсюда может прилететь ошибка, обработать где-то
                directoryModel = CreateDirectoryModel(path, path.Split('\\').Last());

                FillDirectoryModel(directoryModel);

                //TODO отсюда может прилететь ошибка, обработать где-то
                DirectoryModelDic.Add(directoryModel.ShortName, directoryModel);
            }
        }

        /// <summary>
        /// добавление в список базовой дирректории
        /// </summary>
        /// <param name="directoryModel"></param>
        public void AddDirectory(DirectoryModel directoryModel)
        {
            Guard.IsNotNull(directoryModel, nameof(directoryModel));

            //TODO попробовать добавить в гварда
            if (DirectoryModelDic.ContainsKey(directoryModel.SelfDirectoryInfo.FullName))
                throw new ArgumentException("Путь " + directoryModel.SelfDirectoryInfo.FullName + " уже присутствует в списке.");

            DirectoryModelDic.Add(directoryModel.SelfDirectoryInfo.Name, directoryModel);


            using(StreamWriter sw = new StreamWriter(_nameOfCacheFile, true))
            {
                sw.WriteLine(directoryModel.SelfDirectoryInfo.FullName);
            }
        }

        /// <summary>
        /// Мержит словари с одинаковыми типами и задаваемой функцией
        /// TODO СТОИТ ЗАПИХНУТЬ В ОТДЕЛЬНЫЙ ВСПОМОГАТЕЛЬНЫЙ КЛАСС
        /// </summary>
        /// <typeparam name="T">тип ключа</typeparam>
        /// <typeparam name="E">тип значения</typeparam>
        /// <param name="firstDictionary">первый словарь</param>
        /// <param name="secondDictionary">второй словарь</param>
        /// <param name="resultSelector">функция, отвечающая за слияние значений, в том случае, если ключи дублируются</param>
        /// <returns></returns>
        private Dictionary<T, E> DictionaryМerge<T, E>(Dictionary<T, E> firstDictionary, Dictionary<T, E> secondDictionary, Func<IEnumerable<KeyValuePair<T, E>>, E> resultSelector)
        {
            return firstDictionary.Concat(secondDictionary).
                GroupBy(x => x.Key, (key, IEnumKVP) => new { Key = key, Count = resultSelector(IEnumKVP) }).
                ToDictionary(k => k.Key, e => e.Count);
        }


        /// <summary>
        /// (РЕКУРСИЯ!!!)Нахождение всех папок форматов и подсчёт количества фотографий в одинаковых 
        /// </summary>
        /// <param name="directoryModel">директория, в которой будет производиться поиск</param>
        /// <returns></returns>
        public Dictionary<string, int> FormatNumSum(DirectoryModel directoryModel)
        {
            Dictionary<string, int> formatesDic = new Dictionary<string, int>();

            if (directoryModel == null)
                return formatesDic;

            Regex rx = new Regex(@"\d{1,5}[x|х]\d{1,5}[x|х]\d{1,5}",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (rx.IsMatch(directoryModel.ShortName))
            {
                DictionaryHelper.AddToDictionary(formatesDic,
                    new KeyValuePair<string, int>(directoryModel.ShortName, 
                        directoryModel.AggregateNumOfEachFileType.Values.Sum()),(int a,int b)=>a+b);

            }

            foreach (var item in directoryModel.DirectoryModelsList)
            {
                formatesDic = DictionaryМerge<string,int>(formatesDic, FormatNumSum(item),(IEnumerable<KeyValuePair<string, int>> pairs) => pairs.Select(x => x.Value).Sum());
            }

            return formatesDic;
        }
    }
}
