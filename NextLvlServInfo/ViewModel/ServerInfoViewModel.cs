using NextLvlServInfo.Data;
using NextLvlServInfo.DataModel;
using NextLvlServInfo.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace NextLvlServInfo.ViewModel
{
    public class ServerInfoViewModel: INotifyPropertyChanged /*, INotifyCollectionChanged*/
    {
        private DataManager _dataManager;

        #region тесты библиотеки TreeListView

        public ObservableCollection<TestTreeListNode> TestTreeListNodeObsColl { get; set; }

        #endregion

        #region ObservableCollection tests

        private ObservableCollection<KeyValuePair<string,DirectoryModel>> _directoryModels;

        public ObservableCollection<KeyValuePair<string, DirectoryModel>> DirectoriesDic
        {
            get
            {
                return _directoryModels;
            }
            set
            {
                _directoryModels = value;
            }
        }

        #endregion

        public Dictionary<string, int> FormatNum { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #region ICommand members
        // команда добавления нового объекта
        //TODO потумать как можно обобщить некоторые команды 
        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                  (_addCommand = new RelayCommand(obj =>
                  {
                      SelectLoadFolder();
                  }));
            }
        }

        private RelayCommand _deleteDirectoryCommand;
        public RelayCommand DeleteDirectoryCommand
        {
            get
            {
                return _deleteDirectoryCommand ??
                  (_deleteDirectoryCommand = new RelayCommand(obj =>
                  {
                     // SelectLoadFolder();
                  }));
            }
        }
        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public ServerInfoViewModel()
        {

            #region тесты библиотеки TreeListView

            List<TestTreeListNode> testTreeListNodesList = new List<TestTreeListNode>();

            testTreeListNodesList.Add(new TestTreeListNode("1",
                new List<TestTreeListNode>() {
               new TestTreeListNode("1.1", new List<TestTreeListNode>(),2),
               new TestTreeListNode("1.2", new List<TestTreeListNode>(){
                    new TestTreeListNode("1.2.1", new List<TestTreeListNode>(),12),
                    new TestTreeListNode("1.2.2", new List<TestTreeListNode>(),13),
                    new TestTreeListNode("1.2.3", new List<TestTreeListNode>(),14),
               },3),
               new TestTreeListNode("1.3", new List<TestTreeListNode>(),4),
               new TestTreeListNode("1.4", new List<TestTreeListNode>(),5),
                }, 1));

            TestTreeListNodeObsColl = new ObservableCollection<TestTreeListNode>(testTreeListNodesList);

            #endregion

            _dataManager = new DataManager();
            DirectoriesDic = new ObservableCollection<KeyValuePair<string, DirectoryModel>>(_dataManager.DirectoryModelDic);

            //TODO изменить на подсчёт в выбраном элементе и добавить этот список в DirectoryModel
            FormatNum = _dataManager.FormatNumSum(_dataManager.DirectoryModelDic.FirstOrDefault().Value);
        }

        #region Property update

        //Реализация INotifyPropertyChanged
        //Список свойств, обновляющихся через этот метод:
        //1 - DirectoriesDic - обновление дерева папок
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }



        #endregion

        /// <summary>
        /// Обработчик собития выбора папки
        /// </summary>
        public void SelectLoadFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    #region Заполнение данными новой дериктории

                    DirectoryModel newDirectoryModel =  _dataManager.CreateDirectoryModel(folderBrowserDialog.SelectedPath, folderBrowserDialog.SelectedPath.Split('\\').Last());

                    _dataManager.FillDirectoryModel(newDirectoryModel);
                    _dataManager.AddDirectory(newDirectoryModel);

                    DirectoriesDic.Add(new KeyValuePair<string, DirectoryModel>(newDirectoryModel.ShortName, newDirectoryModel));

                    #endregion
                }
            }
        }

        /// <summary>
        /// Обновление зависимых свойств
        /// </summary>
        private void UpdateDependentProperty()
        {
        }
    }

    #region тест библиотеки TreeListNode
    public class TestTreeListNode
    {
        public string NodeName { get; set; }

        public int Size { get; set; }

        public List<TestTreeListNode> TestTreeListNodesList { get; set; }

        public TestTreeListNode()
        {
            TestTreeListNodesList = new List<TestTreeListNode>();
        }
        public TestTreeListNode(string name, List<TestTreeListNode> testTreeListNodes, int size = 1)
        {
            NodeName = name??"Просто нода";
            Size = size;
            TestTreeListNodesList = testTreeListNodes?? new List<TestTreeListNode>();
        }
    } 
    #endregion
}
