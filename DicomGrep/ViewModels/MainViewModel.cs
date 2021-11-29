using DicomGrep.Enums;
using DicomGrep.Models;
using DicomGrep.Services;
using DicomGrep.Utils;
using DicomGrep.Views;
using FellowOakDicom;
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
        // todo: using DI
        private readonly SearchService searchService = new SearchService();
        private readonly FolderPickupService folderPickupService = new FolderPickupService();
        private readonly ConfigurationService configurationService = new ConfigurationService();
        private readonly FileOperationService fileOperationService = new FileOperationService();
        private readonly SopClassLookupService sopClassLookupService = new SopClassLookupService();
        private readonly DicomTagLookupService dicomTagLookupService = new DicomTagLookupService();
        private readonly DialogService dialogService = new DialogService();

        Configuration CurrentConfiguration;

        CancellationTokenSource tokenSource;
        private Object obj = new Object();
        private Object obj2 = new Object();

        #region Search Criteria

        private string _searchPath;
        public string SearchPath
        {
            get { return _searchPath; }
            set { SetProperty(ref _searchPath, value); }
        }

        public ObservableCollection<string> SearchPathHistory { set; get; }


        private string _fileTypes;
        public string FileTypes
        {
            get { return _fileTypes; }
            set { SetProperty(ref _fileTypes, value); }
        }

        public ObservableCollection<string> FileTypesHistory { set; get; }

        private string _sopClassUid;
        public string SopClassUid
        {
            get { return _sopClassUid; }
            set { SetProperty(ref _sopClassUid, value); }
        }

        public ObservableCollection<string> SopClassUidHistory { set; get; }

        private string _tag;
        public string Tag
        {
            get { return _tag; }
            set { SetProperty(ref _tag, value); }
        }

        public ObservableCollection<string> DicomTagHistory { set; get; }


        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        public ObservableCollection<string> SearchTextHistory { set; get; }



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

        private int _searchThreads;
        public int SearchThreads
        {
            get { return _searchThreads; }
            set { SetProperty(ref _searchThreads, value); }
        }

        public ObservableCollection<int> CPULogicCores { set; get; } = new ObservableCollection<int>();

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


        private bool _canSearch = true;
        public bool CanSearch
        {
            get { return _canSearch; }
            set { SetProperty(ref _canSearch, value); }
        }

        private bool _canCancel = false;
        public bool CanCancel
        {
            get { return _canCancel; }
            set { SetProperty(ref _canCancel, value); }
        }

        #region Command

        private ICommand _folderPickupCommand;
        public ICommand FolderPickupCommand
        {
            get
            {
                return _folderPickupCommand ?? (_folderPickupCommand = new RelayCommand<object>(_ => DoPickupFolder()));
            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommand<object>(_ => DoSearch(), _ => CanSearch));
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(_ => DoCancel(), _ => CanCancel));
            }
        }

        private ICommand _fileOperationCommand;
        public ICommand FileOperationCommand
        {
            get
            {
                return _fileOperationCommand ?? (_fileOperationCommand = new RelayCommand<FileOperationsEnum>(foe => DoFileOperation(foe), _ => SelectedMatchedFile != null));
            }
        }

        private ICommand _sopClassLookupCommand;
        public ICommand SopClassLookupCommand
        {
            get
            {
                return _sopClassLookupCommand ?? (_sopClassLookupCommand = new RelayCommand<object>(_=> DoSopClassLookup()));
            }
        }

        private ICommand _dicomtagLookupCommand;
        public ICommand DicomTagLookupCommand
        {
            get
            {
                return _dicomtagLookupCommand ?? (_dicomtagLookupCommand = new RelayCommand<object>(_=> DoDicomTagLookup()));
            }
        }

        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                return _exitCommand ?? (_exitCommand = new RelayCommand<object>(_ => App.Current.Shutdown()));
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
            // todo: choose a better place to initialize the fo-dicom?
            DicomDictionary.EnsureDefaultDictionariesLoaded(true);

            configurationService.Load();

            CurrentConfiguration = configurationService.GetConfiguration();

            SearchPathHistory = new ObservableCollection<string>(CurrentConfiguration.SearchPathHistory ?? new List<string>());
            FileTypesHistory = new ObservableCollection<string>(CurrentConfiguration.FileTypesHistory ?? new List<string>());
            SopClassUidHistory = new ObservableCollection<string>(CurrentConfiguration.SopClassUidHistory ?? new List<string>());
            DicomTagHistory = new ObservableCollection<string>(CurrentConfiguration.DicomTagHistory ?? new List<string>());
            SearchTextHistory = new ObservableCollection<string>(CurrentConfiguration.SearchTextHistory ?? new List<string>());

            SearchPath = CurrentConfiguration.SearchCriteria.SearchPath;
            FileTypes = CurrentConfiguration.SearchCriteria.FileTypes;
            SopClassUid = CurrentConfiguration.SearchCriteria.SearchSopClassUid;
            Tag = CurrentConfiguration.SearchCriteria.SearchTag;
            SearchText = CurrentConfiguration.SearchCriteria.SearchText;
            CaseSensitive = CurrentConfiguration.SearchCriteria.CaseSensitive;
            WholeWord = CurrentConfiguration.SearchCriteria.WholeWord;
            IncludeSubfolders = CurrentConfiguration.SearchCriteria.IncludeSubfolders;
            SearchThreads = Math.Min(CurrentConfiguration.SearchCriteria.SearchThreads, Environment.ProcessorCount);

            CPULogicCores = new ObservableCollection<int>(GetCPULogicCoresList());

            this.searchService.FileListCompleted += SearchService_FileListCompleted;

            this.searchService.OnLoadDicomFile += SearchService_OnLoadDicomFile;

            this.searchService.OnCompletDicomFile += SearchService_OnCompletDicomFile;

            this.searchService.OnSearchComplete += SearchService_OnSearchComplete;

        }

        private IEnumerable<int> GetCPULogicCoresList()
        {
            for (int i = 1; i <= Environment.ProcessorCount; i++)
            {
                yield return i;
            }
        }

        private void SearchService_FileListCompleted(object sender, Services.EventArgs.ListFileCompletedEventArgs e)
        {
            this.TotalFileCount = e.Count;
        }

        private void SearchService_OnLoadDicomFile(object sender, Services.EventArgs.OnLoadDicomFileEventArgs e)
        {
            lock (obj)
            {
                CurrentFile = e.Filename;
            }
        }

        private void SearchService_OnCompletDicomFile(object sender, Services.EventArgs.OnCompleteDicomFileEventArgs e)
        {
            lock (obj2)
            {
                if (e.IsMatched)
                {
                    App.Current.Dispatcher.Invoke(() => MatchedFileList.Add(e.ResultDicomFile));
                    MatchedFileCount = e.MatchedFileCount;
                }
                SearchedFileCount = e.SearchedFileCount;
            }
        }

        private void SearchService_OnSearchComplete(object sender, Services.EventArgs.OnSearchCompleteEventArgs e)
        {
            this.CanSearch = true;
            this.CanCancel = false;
        }

        private void DoSearch()
        {
            SearchCriteria criteria = new SearchCriteria
            {
                SearchPath = SearchPath,
                FileTypes = FileTypes,
                SearchSopClassUid = SopClassUid,
                SearchTag = Tag,
                SearchText = SearchText,
                CaseSensitive = CaseSensitive,
                WholeWord = WholeWord,
                IncludeSubfolders = IncludeSubfolders,
                SearchThreads = SearchThreads
            };

            Util.PushToList(SearchPath, SearchPathHistory, CurrentConfiguration.HistoryCapacity);
            Util.PushToList(FileTypes, FileTypesHistory, CurrentConfiguration.HistoryCapacity);
            Util.PushToList(SopClassUid, SopClassUidHistory, CurrentConfiguration.HistoryCapacity);
            Util.PushToList(Tag, DicomTagHistory, CurrentConfiguration.HistoryCapacity);
            Util.PushToList(SearchText, SearchTextHistory, CurrentConfiguration.HistoryCapacity);

            CurrentConfiguration.SearchCriteria = criteria;
            CurrentConfiguration.SearchPathHistory = new List<string>(SearchPathHistory);
            CurrentConfiguration.FileTypesHistory = new List<string>(FileTypesHistory);
            CurrentConfiguration.SopClassUidHistory = new List<string>(SopClassUidHistory);
            CurrentConfiguration.DicomTagHistory = new List<string>(DicomTagHistory);
            CurrentConfiguration.SearchTextHistory = new List<string>(SearchTextHistory);

            configurationService.Save();

            MatchedFileList.Clear();
            SelectedMatchedFile = null;

            this.TotalFileCount = 0;
            this.SearchedFileCount = 0;
            this.MatchedFileCount = 0;

            this.CanCancel = true;
            this.CanSearch = false;

            tokenSource = new CancellationTokenSource();

            // todo: move to SearchAsync()
            Task.Run(() =>
            {
                this.searchService.Search(criteria, tokenSource);
            }, tokenSource.Token);
        }

        private void DoCancel()
        {
            tokenSource.Cancel();
            //searchService.Cancel();
        }

        private void DoPickupFolder()
        {
            string folder = SearchPath;
            if (folderPickupService.SelectFolder(ref folder))
            {
                SearchPath = folder;
            }
        }

        private void DoSopClassLookup()
        {
            string uid = SopClassUid;
            if (sopClassLookupService.SelectSopClass(ref uid))
            {
                SopClassUid = uid;
            }
        }

        private void DoDicomTagLookup()
        {
            string tag = Tag;
            if (dicomTagLookupService.SelectDicomTag(ref tag))
            {
                Tag = tag;
            }
        }

        private void DoFileOperation(FileOperationsEnum foe)
        {
            if (SelectedMatchedFile == null)
            {
                return;
            }
            switch (foe)
            {
                case FileOperationsEnum.OpenDirectory:
                    fileOperationService.OpenDirectory(SelectedMatchedFile.FullFilename);
                    break;
                case FileOperationsEnum.OpenFile:
                    fileOperationService.OpenFile(SelectedMatchedFile.FullFilename);
                    break;
                case FileOperationsEnum.CopyFullNamePath:
                    fileOperationService.CopyFullNamePath(SelectedMatchedFile.FullFilename);
                    break;
                case FileOperationsEnum.CopyName:
                    fileOperationService.CopyName(SelectedMatchedFile.FullFilename);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
