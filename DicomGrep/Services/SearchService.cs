using Dicom;
using Dicom.Serialization;
using DicomGrep.Models;
using DicomGrep.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DicomGrep.Services
{
    public class SearchService : ISearchService
    {
        SearchCriteria _criteria;
        IDicomSearchService _dicomSearchService;

        public event EventHandler SearchStarted;
        public event EventHandler SearchCompleted;
        public event FileListCompletedDelegate FileListCompleted;
        public event SearchFileStatusDelegate SearchFileStatus;

        private List<string> filenameList = new List<string>();
        private CancellationTokenSource tokenSource = new CancellationTokenSource();


        public async void Search(SearchCriteria searchCriteria, IDicomSearchService dicomSearchService)
        {
            await Task.Run(() =>
            {

                this._dicomSearchService = dicomSearchService;
                _criteria = searchCriteria;

                SearchStarted?.Invoke(this, null);

                #region List all files

                filenameList.Clear();

                if (string.IsNullOrWhiteSpace(_criteria.FileExtension))
                {
                    _criteria.FileExtension = "*.*";
                }

                // gather all qualified file names first
                if (Directory.Exists(_criteria.SearchPath))
                {
                    ProcessDirectory(_criteria.SearchPath);
                }

                FileListCompleted?.Invoke(this, new EventArgs.FileListCompletedEventArgs() { FileNames = filenameList });

                #endregion List all files


                // search in file content
                int fileIndex = 0;

                foreach (var filename in filenameList)
                {
                    if (tokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    SearchFileStatus?.Invoke(this,
                                        new EventArgs.SearchFileStatusEventArgs
                                        {
                                            Filename = filename,
                                            Index = fileIndex
                                        });
                    //Console.WriteLine("Processing file '{0}'.", filename);

                    _dicomSearchService.SearchInDicomFile(filename, _criteria);
                    fileIndex++;
                }

                SearchCompleted?.Invoke(this, null);

            }, tokenSource.Token);
        }

        public void Cancel()
        {
            tokenSource?.Cancel();
        }


        private void ProcessDirectory(string targetDirectory)
        {
            try
            {
                filenameList.AddRange(Directory.GetFiles(targetDirectory, _criteria.FileExtension));

                // Recurse into subdirectories of this directory
                if (_criteria.IncludeSubfolders)
                {
                    foreach (string subdirectory in Directory.GetDirectories(targetDirectory))
                    {
                        ProcessDirectory(subdirectory);
                    }
                }
            }
            catch (Exception ex)// no permission on some folder
            {
                // todo: exception event
            }
        }



    }
}
