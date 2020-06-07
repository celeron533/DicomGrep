using Caliburn.Micro;
using DicomGrep.Models;
using DicomGrep.Services.EventArgs;
using DicomGrep.Services.Interfaces;
using DicomGrep.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Action = System.Action;

namespace DicomGrep.ViewModels
{
    public class MainViewModel : PropertyChangedBase, IMainViewModel
    {

        private string _windowTitle = "Dicom Grep";

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                if (!Equals(_windowTitle, value))
                {
                    _windowTitle = value;
                    NotifyOfPropertyChange(() => this.WindowTitle);
                }
            }
        }


        private SearchCriteria _searchCriteria = new SearchCriteria();
        public SearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (!Equals(_searchCriteria, value))
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => this.SearchCriteria);
                }
            }
        }

        private SearchResultCollection _searchResult = new SearchResultCollection();
        public SearchResultCollection SearchResult
        {
            get { return _searchResult; }
            set
            {
                if (!Equals(_searchResult, value))
                {
                    _searchResult = value;
                    NotifyOfPropertyChange(() => this.SearchResult);
                }
            }
        }

        private int _totalFileCount;
        public int TotalFileCount
        {
            get { return _totalFileCount; }
            set
            {
                if (!Equals(_totalFileCount, value))
                {
                    _totalFileCount = value;
                    NotifyOfPropertyChange(() => this.TotalFileCount);
                }
            }
        }

        private int _processingFileIndex;
        public int ProcessingFileIndex
        {
            get { return _processingFileIndex; }
            set
            {
                if (!Equals(_processingFileIndex, value))
                {
                    _processingFileIndex = value;
                    NotifyOfPropertyChange(() => this.ProcessingFileIndex);
                }
            }
        }

        private string _processingFilename;
        public string ProcessingFilename
        {
            get { return _processingFilename; }
            set
            {
                if (!Equals(_processingFilename, value))
                {
                    _processingFilename = value;
                    NotifyOfPropertyChange(() => this.ProcessingFilename);
                }
            }
        }



        private bool _canSearch = true;

        public bool CanSearch
        {
            get { return _canSearch; }
            set
            {
                if (!Equals(_canSearch, value))
                {
                    _canSearch = value;
                    NotifyOfPropertyChange(() => this.CanSearch);
                }
            }
        }

        private bool _canCancel = false;

        public bool CanCancel
        {
            get { return _canCancel; }
            set
            {
                if (!Equals(_canCancel, value))
                {
                    _canCancel = value;
                    NotifyOfPropertyChange(() => this.CanCancel);
                }
            }
        }

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public Action BrowseFolderAction => this.BrowseFolder;

        public Action SearchAction => this.Search;
        public Action CancelAction => this.Cancel;

        ISearchService searchService = IoC.Get<ISearchService>();
        IDicomSearchService dicomSearchService = IoC.Get<IDicomSearchService>();

        public void BrowseFolder()
        {
            IFolderSelectionService folderSelectionService = IoC.Get<IFolderSelectionService>();
            string searchPath = this.SearchCriteria.SearchPath;

            if (folderSelectionService.SelectFolder(ref searchPath))
            {
                this.SearchCriteria.SearchPath = searchPath;
            }
        }

        public void Search()
        {
            SearchResult.Clear();

            searchService.SearchStarted += SearchService_SearchStarted;
            searchService.FileListCompleted += SearchService_FileListCompleted;
            searchService.SearchFileStatus += SearchService_SearchFileStatus;
            searchService.SearchCompleted += SearchService_SearchCompleted;

            dicomSearchService.SearchFileCompleted += DicomSearchService_SearchFileCompleted;

            cancellationTokenSource = new CancellationTokenSource();

            searchService.Search(_searchCriteria, dicomSearchService);
        }



        private void SearchService_SearchStarted(object sender, EventArgs e)
        {
            this.CanSearch = false;
            this.CanCancel = true;
        }

        private void SearchService_SearchCompleted(object sender, EventArgs e)
        {
            this.CanSearch = true;
            this.CanCancel = false;

            searchService.FileListCompleted -= SearchService_FileListCompleted;
            searchService.SearchFileStatus -= SearchService_SearchFileStatus;
            searchService.SearchStarted -= SearchService_SearchStarted;
            searchService.SearchCompleted -= SearchService_SearchCompleted;

            dicomSearchService.SearchFileCompleted -= DicomSearchService_SearchFileCompleted;
        }


        private void SearchService_SearchFileStatus(object sender, SearchFileStatusEventArgs e)
        {
            ProcessingFileIndex = e.Index + 1;
            ProcessingFilename = e.Filename;
        }

        private void DicomSearchService_SearchFileCompleted(object sender, SearchFileCompletedEventArgs e)
        {
            if (e.FileResult.ResultItemCollection != null && e.FileResult.ResultItemCollection.Any())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    () => SearchResult.Add(e.FileResult)
                );
            }
            //throw new NotImplementedException();
        }

        private void SearchService_FileListCompleted(object sender, FileListCompletedEventArgs e)
        {
            TotalFileCount = e.TotalFileCount;
        }

        public void Cancel()
        {
            searchService.Cancel();
        }
    }
}
