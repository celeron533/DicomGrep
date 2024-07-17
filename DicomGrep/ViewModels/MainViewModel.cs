using DicomGrep.Enums;
using DicomGrep.Models;
using DicomGrep.Utils;
using DicomGrep.Views;
using DicomGrepCore.Entities;
using DicomGrepCore.Services;
using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DicomGrepCore.Enums;
using DicomGrep.Services;
using DicomGrep.Services.Interfaces;
using DicomGrepCore.Services.Interfaces;

namespace DicomGrep.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        ISearchService searchService;
        IDictionaryService dictionaryService;

        IConfigurationService configurationService;
        IFolderPickupService folderPickupService;
        ISopClassLookupService sopClassLookupService;
        IDicomDictionaryLookupService dicomDictionaryLookupService;
        IExportService exportService;
        ITagValueDetailService tagValueDetailService;
        IFileOperationService fileOperationService;

        Configuration CurrentConfiguration;

        CancellationTokenSource tokenSource;
        private Object obj = new Object();
        private Object obj2 = new Object();

        #region Search Criteria

        private string _searchPath;
        public string SearchPath
        {
            get => _searchPath;
            set => SetProperty(ref _searchPath, value);
        }

        public ObservableCollection<string> SearchPathHistory { set; get; }


        private string _fileTypes;
        public string FileTypes
        {
            get => _fileTypes;
            set => SetProperty(ref _fileTypes, value);
        }

        public ObservableCollection<string> FileTypesHistory { set; get; }

        private string _sopClassUid;
        public string SopClassUid
        {
            get => _sopClassUid;
            set => SetProperty(ref _sopClassUid, value);
        }

        public ObservableCollection<string> SopClassUidHistory { set; get; }

        private string _tag;
        public string Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }

        public ObservableCollection<string> DicomTagHistory { set; get; }


        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public ObservableCollection<string> SearchTextHistory { set; get; }



        private bool _caseSensitive;
        public bool CaseSensitive
        {
            get => _caseSensitive;
            set => SetProperty(ref _caseSensitive, value);
        }

        private bool _wholeWord;
        public bool WholeWord
        {
            get => _wholeWord;
            set => SetProperty(ref _wholeWord, value);
        }

        private MatchPatternEnum _matchPattern;

        public MatchPatternEnum MatchPattern
        {
            get => _matchPattern;
            set => SetProperty(ref _matchPattern, value);
        }

        private bool _includeSubfolders;
        public bool IncludeSubfolders
        {
            get => _includeSubfolders;
            set => SetProperty(ref _includeSubfolders, value);
        }

        private int _searchThreads;
        public int SearchThreads
        {
            get => _searchThreads;
            set => SetProperty(ref _searchThreads, value);
        }

        #endregion Search Criteria END

        public ObservableCollection<int> CPULogicCores { set; get; } = new ObservableCollection<int>();

        private int _totalFileCount;
        public int TotalFileCount
        {
            get => _totalFileCount;
            set => SetProperty(ref _totalFileCount, value);
        }

        private int _searchedFileCount;
        public int SearchedFileCount
        {
            get => _searchedFileCount;
            set => SetProperty(ref _searchedFileCount, value);
        }

        private int _matchFileCount;
        public int MatchFileCount
        {
            get => _matchFileCount;
            set => SetProperty(ref _matchFileCount, value);
        }

        private string _currentFile;
        public string CurrentFile
        {
            get => _currentFile;
            set => SetProperty(ref _currentFile, value);
        }


        private MainStatusEnum _mainStatus;
        public MainStatusEnum MainStatus
        {
            get => _mainStatus;
            set => SetProperty(ref _mainStatus, value);
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
                return _searchCommand ?? (_searchCommand = new RelayCommand<object>(_ => DoSearch(false), CanExecuteSearchCommand));
            }
        }

        public bool CanExecuteSearchCommand(object obj)
        {
            return this.MainStatus != MainStatusEnum.Working;
        }

        private ICommand _searchInResultsCommand;
        public ICommand SearchInResultsCommand
        {
            get
            {
                return _searchInResultsCommand ?? (_searchInResultsCommand = new RelayCommand<object>(_ => DoSearch(true), CanExecuteSearchCommand));
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(_ => DoCancel(), CanExecuteCancelCommand));
            }
        }

        public bool CanExecuteCancelCommand(object obj)
        {
            return this.MainStatus == MainStatusEnum.Working;
        }

        private ICommand _fileOperationCommand;
        public ICommand FileOperationCommand
        {
            get
            {
                return _fileOperationCommand ?? (_fileOperationCommand = new RelayCommand<FileOperationsEnum>(foe => DoFileOperation(foe), _ => SelectedMatchFile != null));
            }
        }

        private ICommand _sopClassLookupCommand;
        public ICommand SopClassLookupCommand
        {
            get
            {
                return _sopClassLookupCommand ?? (_sopClassLookupCommand = new RelayCommand<object>(_ => DoSopClassLookup()));
            }
        }

        private ICommand _dicomDictionaryLookupCommand;
        public ICommand DicomDictionaryLookupCommand
        {
            get
            {
                return _dicomDictionaryLookupCommand ?? (_dicomDictionaryLookupCommand = new RelayCommand<object>(_ => DoDicomDictionaryLookup()));
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

        private ICommand _exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new RelayCommand<object>(_ => DoExport()));
            }
        }

        private ICommand _detailsCommand;
        public ICommand DetailsCommand
        {
            get
            {
                return _detailsCommand ?? (_detailsCommand = new RelayCommand<ResultDicomItem>(item => DoDetails(item)));
            }
        }

        #endregion Command END


        public ObservableCollection<ResultDicomFile> MatchFileList { set; get; } = new ObservableCollection<ResultDicomFile>();

        private ResultDicomFile _selectedMatchFile;
        public ResultDicomFile SelectedMatchFile
        {
            get => _selectedMatchFile;
            set => SetProperty(ref _selectedMatchFile, value);
        }


        public MainViewModel(IConfigurationService _configurationService, IFolderPickupService _folderPickupService,
            ISopClassLookupService _sopClassLookupService, IDicomDictionaryLookupService _dicomDictionaryLookupService,
            IExportService _exportService, ITagValueDetailService _tagValueDetailService,
            IFileOperationService _fileOperationService, ISearchService _searchService, IDictionaryService _dictionaryService)
        {
            configurationService = _configurationService;
            folderPickupService = _folderPickupService;
            sopClassLookupService = _sopClassLookupService;
            dicomDictionaryLookupService = _dicomDictionaryLookupService;
            exportService = _exportService;
            tagValueDetailService = _tagValueDetailService;
            fileOperationService = _fileOperationService;
            searchService = _searchService;
            dictionaryService = _dictionaryService; ;

            // dummy data for designer preview
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                for (int i = 1; i <= 5; i++)
                {
                    ResultDicomItem resultDicomItem1 = new ResultDicomItem(new DicomTag(0x0010, 0x0020), "John Doe", new byte[0]);
                    ResultDicomItem resultDicomItem2 = new ResultDicomItem(new DicomTag(0x0123, 0x99, "DicomGrep Creator"), "C533", new byte[0]);
                    List<ResultDicomItem> items = new List<ResultDicomItem>
                    {
                        resultDicomItem1,
                        resultDicomItem2
                    };

                    ResultDicomFile resultDicomFile = new ResultDicomFile($"C:\\DICOM\\preview\\sampleFile{i}.dcm", "RT Plan Storage", "1.2.840.10008.5.1.4.1.1.481.5", $"Patient{i}", items);
                    MatchFileList.Add(resultDicomFile);
                }
                SelectedMatchFile = MatchFileList.First();
            }

            dictionaryService.ReadAndAppendCustomDictionaries();

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
            MatchPattern = CurrentConfiguration.SearchCriteria.MatchPattern;
            IncludeSubfolders = CurrentConfiguration.SearchCriteria.IncludeSubfolders;
            //SearchInResults = CurrentConfiguration.SearchCriteria.SearchInResults;
            SearchThreads = Math.Min(CurrentConfiguration.SearchCriteria.SearchThreads, Environment.ProcessorCount);

            CPULogicCores = new ObservableCollection<int>(GetCPULogicCoresList());

            this.searchService.FileListCompleted += SearchService_FileListCompleted;

            this.searchService.OnLoadDicomFile += SearchService_OnLoadDicomFile;

            this.searchService.OnCompletDicomFile += SearchService_OnCompletDicomFile;

            this.searchService.OnSearchComplete += SearchService_OnSearchComplete;

        }

        private IEnumerable<int> GetCPULogicCoresList()
        {
            // 0 means auto
            for (int i = 0; i <= Environment.ProcessorCount; i++)
            {
                yield return i;
            }
        }

        private void SearchService_FileListCompleted(object sender, DicomGrepCore.Services.EventArgs.ListFileCompletedEventArgs e)
        {
            this.TotalFileCount = e.Count;
        }

        private void SearchService_OnLoadDicomFile(object sender, DicomGrepCore.Services.EventArgs.OnLoadDicomFileEventArgs e)
        {
            lock (obj)
            {
                CurrentFile = e.Filename;
            }
        }

        private void SearchService_OnCompletDicomFile(object sender, DicomGrepCore.Services.EventArgs.OnCompleteDicomFileEventArgs e)
        {
            lock (obj2)
            {
                if (e.IsMatch)
                {
                    App.Current.Dispatcher.Invoke(() => MatchFileList.Add(e.ResultDicomFile));
                    MatchFileCount = e.MatchFileCount;
                }
                SearchedFileCount = e.SearchedFileCount;
            }
        }

        private void SearchService_OnSearchComplete(object sender, DicomGrepCore.Services.EventArgs.OnSearchCompleteEventArgs e)
        {
            this.MainStatus = MainStatusEnum.Complete;
            InvalidateRequerySuggested();
        }

        private void DoSearch(bool searchInResults = false)
        {
            this.MainStatus = MainStatusEnum.Working;
            SearchCriteria criteria = new SearchCriteria
            {
                SearchPath = SearchPath,
                FileTypes = FileTypes,
                SearchSopClassUid = SopClassUid,
                SearchTag = Tag,
                SearchText = SearchText,
                CaseSensitive = CaseSensitive,
                WholeWord = WholeWord,
                MatchPattern = MatchPattern,
                IncludeSubfolders = IncludeSubfolders,
                SearchInResults = searchInResults,  // from parameter
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

            MatchFileList.Clear();
            SelectedMatchFile = null;

            this.TotalFileCount = 0;
            this.SearchedFileCount = 0;
            this.MatchFileCount = 0;

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

        private void DoDicomDictionaryLookup()
        {
            string tag = Tag;
            if (dicomDictionaryLookupService.SelectDicomDictionaryEntry(ref tag))
            {
                Tag = tag;
            }
        }

        private void DoExport()
        {
            exportService.Export(new List<ResultDicomFile>(MatchFileList));
        }

        private void DoDetails(ResultDicomItem item)
        {
            tagValueDetailService.InspectTagValue(item);
        }

        private void DoFileOperation(FileOperationsEnum foe)
        {
            if (SelectedMatchFile == null)
            {
                return;
            }
            switch (foe)
            {
                case FileOperationsEnum.OpenDirectory:
                    fileOperationService.OpenDirectory(SelectedMatchFile.FullFilename);
                    break;
                case FileOperationsEnum.OpenFile:
                    fileOperationService.OpenFile(SelectedMatchFile.FullFilename);
                    break;
                case FileOperationsEnum.CopyFullNamePath:
                    fileOperationService.CopyFullNamePath(SelectedMatchFile.FullFilename);
                    break;
                case FileOperationsEnum.CopyName:
                    fileOperationService.CopyName(SelectedMatchFile.FullFilename);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
