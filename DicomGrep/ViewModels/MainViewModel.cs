using DicomGrep.Models;
using DicomGrep.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DicomGrep.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly SearchService searchService = new SearchService();
        private readonly FolderPickupService folderPickupService = new FolderPickupService();
        private readonly ConfigurationService configurationService = new ConfigurationService();

        private Object obj = new Object();
        private Object obj2 = new Object();

        #region Search Criteria

        private string _searchPath;
        public string SearchPath
        {
            get { return _searchPath; }
            set { SetProperty(ref _searchPath, value); }
        }

        public ObservableCollection<string> SearchPathHistory { set; get; } = new ObservableCollection<string>();


        private string _fileTypes;
        public string FileTypes
        {
            get { return _fileTypes; }
            set { SetProperty(ref _fileTypes, value); }
        }

        public ObservableCollection<string> FileTypesHistory { set; get; } = new ObservableCollection<string>();


        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        public ObservableCollection<string> SearchTextHistory { set; get; } = new ObservableCollection<string>();



        private bool _searchDicomTag;
        public bool SearchDicomTag
        {
            get { return _searchDicomTag; }
            set { SetProperty(ref _searchDicomTag, value); }
        }

        private bool _searchDicomValue;
        public bool SearchDicomValue
        {
            get { return _searchDicomValue; }
            set { SetProperty(ref _searchDicomValue, value); }
        }

        private bool _caseSensitive;
        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set { SetProperty(ref _caseSensitive, value); }
        }

        private bool _wholeWord;
        public bool WholeWord
        {
            get { return _wholeWord; }
            set { SetProperty(ref _wholeWord, value); }
        }

        private bool _includeSubfolders;
        public bool IncludeSubfolders
        {
            get { return _includeSubfolders; }
            set { SetProperty(ref _includeSubfolders, value); }
        }

        private bool _includePrivateTag;
        public bool IncludePrivateTag
        {
            get { return _includePrivateTag; }
            set { SetProperty(ref _includePrivateTag, value); }
        }

        #endregion Search Criteria END

        private int _totalFileCount;
        public int TotalFileCount
        {
            get { return _totalFileCount; }
            set { SetProperty(ref _totalFileCount, value); }
        }

        private int _searchedFileCount;
        public int SearchedFileCount
        {
            get { return _searchedFileCount; }
            set { SetProperty(ref _searchedFileCount, value); }
        }

        private int _matchedFileCount;
        public int MatchedFileCount
        {
            get { return _matchedFileCount; }
            set { SetProperty(ref _matchedFileCount, value); }
        }

        private string _currentFile;
        public string CurrentFile
        {
            get { return _currentFile; }
            set { SetProperty(ref _currentFile, value); }
        }


        #region Command

        private ICommand _folderPickupCommand;
        public ICommand FolderPickupCommand
        {
            get 
            {
                return _folderPickupCommand ?? (_folderPickupCommand = new RelayCommand<object>(_ => 
                {
                    DoPickupFolder();
                }));
            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommand<object>(_ =>
                {
                    DoSearch();
                }));
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(_ =>
                {
                    System.Windows.MessageBox.Show("Responding to button click event...");
                }));
            }
        }

        #endregion Command END


        public ObservableCollection<ResultDicomFile> MatchedFileList { set; get; } = new ObservableCollection<ResultDicomFile>();

        private ResultDicomFile _selectedMatchedFile;
        public ResultDicomFile SelectedMatchedFile
        {
            get { return _selectedMatchedFile; }
            set { SetProperty(ref _selectedMatchedFile, value); }
        }


        public MainViewModel()
        {
            configurationService.LoadConfiguration();

            SearchPathHistory = new ObservableCollection<string>(configurationService.GetConfiguration().SearchPathHistory);
            FileTypesHistory = new ObservableCollection<string>(configurationService.GetConfiguration().FileTypesHistory);
            SearchTextHistory = new ObservableCollection<string>(configurationService.GetConfiguration().SearchTextHistory);

            SearchPath = SearchPathHistory[0];
            FileTypes = FileTypesHistory[0];
            SearchText = SearchTextHistory[0];


            SearchDicomTag = true;
            SearchDicomValue = true;

            IncludeSubfolders = true;

            this.searchService.FileListCompleted += (sender, arg) => { this.TotalFileCount = arg.Count; };

            this.searchService.OnLoadDicomFile += (sender, arg) =>
            {
                lock (obj)
                {
                    CurrentFile = arg.Filename;
                }
            };

            this.searchService.OnCompletDicomFile += (sender, arg) =>
            {
                lock (obj2)
                {
                    if (arg.IsMatched)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                            MatchedFileList.Add(arg.ResultDicomFile));
                        MatchedFileCount++;
                    }
                    SearchedFileCount++;
                }
            };

        }

        private void DoSearch()
        {
            SearchCriteria criteria = new SearchCriteria
            {
                SearchPath = SearchPath,
                FileTypes = FileTypes,
                SearchText = SearchText,
                SearchDicomTag = SearchDicomTag,
                SearchDicomValue = SearchDicomValue,
                CaseSensitive = CaseSensitive,
                WholeWord = WholeWord,
                IncludeSubfolders = IncludeSubfolders,
                IncludePrivateTag = IncludePrivateTag
            };



            MatchedFileList.Clear();

            this.TotalFileCount = 0;
            this.SearchedFileCount = 0;
            this.MatchedFileCount = 0;

            Task.Run(() =>
            {
                this.searchService.Search(criteria);
            });
        }

        private void DoPickupFolder()
        {
            string folder = SearchPath;
            if (folderPickupService.SelectFolder(ref folder))
            {
                SearchPath = folder;
            }
        }

    }
}
