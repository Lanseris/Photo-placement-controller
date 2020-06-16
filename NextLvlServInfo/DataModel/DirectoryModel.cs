using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NextLvlServInfo.Auxiliary;


namespace NextLvlServInfo.DataModel
{
    public class DirectoryModel
    {
        public string ShortName { get; set; }

        public DirectoryInfo SelfDirectoryInfo { get; set; }

        public Dictionary<string, int> CurrentLvlNumOfEachFileType { get; set; }

        public Dictionary<string,int> FoldersEachTypeOfFileNum { get; set; }

        public Dictionary<string, int> AggregateNumOfEachFileType { get; set; }

        public int AggregateNumOfFiles { get; set; }

        public List<DirectoryModel> DirectoryModelsList { get; set; }

        //функция для мержа повторяющихся элементов словарей с типами файлов
        public Func<IEnumerable<KeyValuePair<string, int>>,int> Sum  = pairs => pairs.Select(x => x.Value).Sum();

        public DirectoryModel()
        {
            DirectoryModelsList = new List<DirectoryModel>();
            CurrentLvlNumOfEachFileType = new Dictionary<string, int>();
            FoldersEachTypeOfFileNum = new Dictionary<string, int>();
            AggregateNumOfEachFileType = new Dictionary<string, int>();
            AggregateNumOfFiles = 0;
        }


        public void AddToCurrent(string fileType)
        {
            if (!CurrentLvlNumOfEachFileType.ContainsKey(fileType))
            {
                CurrentLvlNumOfEachFileType.Add(fileType, 0);
            }

            CurrentLvlNumOfEachFileType[fileType]++;
        }


        /// <summary>
        /// Заполняет словарь количества типов файлов диррекотрии текущщей модели
        /// </summary>
        /// <returns></returns>
        public void FillNumOfTypesCurrDirectory()
        {
            Guard.FileExistCheck(SelfDirectoryInfo);

            var files = SelfDirectoryInfo.GetFiles();

            foreach (var file in files)
            {
                AddToCurrent(file.Extension);
            }
        }

    }
}
