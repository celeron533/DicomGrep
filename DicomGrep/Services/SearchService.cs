using Dicom;
using Dicom.Serialization;
using DicomGrep.Models;
using DicomGrep.Services.Interfaces;
using System;
using System.Collections.Generic;
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

        public event EventHandler SearchStarted;
        public event EventHandler SearchCompleted;
        public event FileListCompletedDelegate FileListCompleted;
        public event SearchFileStatusDelegate SearchFileStatus;
        public event SearchFileMatchDelegate SearchFileMatch;


        private IList<string> filenameList = new List<string>();
        private SearchResultCollection searchResultDictionary = new SearchResultCollection();

        private CancellationTokenSource tokenSource;

        public void Search(SearchCriteria searchCriteria)
        {
            tokenSource= new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                SearchStarted?.Invoke(this, null);

                _criteria = searchCriteria;

                filenameList.Clear();
                searchResultDictionary.Clear();

                if (string.IsNullOrWhiteSpace(_criteria.FileExtension))
                {
                    _criteria.FileExtension = "*.*";
                }

                // gather all qualified file names first
                if (Directory.Exists(_criteria.SearchPath))
                {
                    ProcessDirectory(_criteria.SearchPath);
                }
                // todo: raise event: filename listed
                FileListCompleted?.Invoke(this, new EventArgs.FileListCompletedEventArgs() { TotalFileCount = filenameList.Count });

                // search in file content
                int fileIndex = 0;

                //Parallel.ForEach(filenameList, filename =>
                //{
                //    SearchFileStatus?.Invoke(this,
                //                        new EventArgs.SearchFileStatusEventArgs
                //                        {
                //                            Filename = filename,
                //                            Index = fileIndex
                //                        });
                //    //Console.WriteLine("Processing file '{0}'.", filename);

                //    SearchInDicomFile(filename);
                //    fileIndex++;
                //});

                foreach (var filename in filenameList)
                {
                    if (tokenSource.Token.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        break;
                        //SearchCompleted?.Invoke(this, null);
                        //tokenSource.Token.ThrowIfCancellationRequested();
                    }

                    SearchFileStatus?.Invoke(this,
                        new EventArgs.SearchFileStatusEventArgs
                        {
                            Filename = filename,
                            Index = fileIndex
                        });
                    //Console.WriteLine("Processing file '{0}'.", filename);

                    SearchInDicomFile(filename);
                    fileIndex++;
                }

                SearchCompleted?.Invoke(this, null);
            }, tokenSource.Token);
        }

        public void Cancel()
        {
            tokenSource.Cancel();
        }


        private void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory, _criteria.FileExtension);
            foreach (string fileName in fileEntries)
            {
                filenameList.Add(fileName);
            }

            // Recurse into subdirectories of this directory.
            if (_criteria.IncludeSubfolders)
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    ProcessDirectory(subdirectory);
                }
            }
        }


        private void SearchInDicomFile(string path)
        {
            try
            {
                DicomFile dicomFile = DicomFile.Open(path);
                //new DicomDatasetWalker(dicomFile.Dataset).Walk(new DatasetWalker());

                CompareDicomTagAndValue(dicomFile.FileMetaInfo, path);
                CompareDicomTagAndValue(dicomFile.Dataset, path);
            }
            catch (Exception ex)
            {
                //event for error logging
            }
        }

        private bool CompareDicomTagAndValue(DicomDataset dataset, string filename)
        {
            foreach (DicomItem dicomItem in dataset)
            {
                if (_criteria.SearchDicomTag)
                {
                    if (CompareString(dicomItem.Tag.ToString(), _criteria, true))
                    {
                        //handle match
                        searchResultDictionary.AddResult(filename, dicomItem.Tag, "* match Tag");
                        SearchFileMatch?.Invoke(this, new EventArgs.SearchFileMatchEventArgs { Filename = filename, DicomTag = dicomItem.Tag, ValueString = "* match Tag", SearchResult = searchResultDictionary });
                        //Console.WriteLine($"match tag: {dicomItem.ToString()}");
                    }
                }
                if (_criteria.SearchDicomValue)
                {
                    // contains sub sequence
                    if (dicomItem.ValueRepresentation == DicomVR.SQ)
                    {
                        foreach (DicomDataset innerDataset in ((DicomSequence)dicomItem).Items)
                        {
                            CompareDicomTagAndValue(innerDataset, filename);
                        }
                    }
                    // skip binary VRs
                    else if (dicomItem.ValueRepresentation == DicomVR.OB ||
                        dicomItem.ValueRepresentation == DicomVR.OF ||
                        dicomItem.ValueRepresentation == DicomVR.OW)
                    {
                        continue;
                    }
                    // compare value
                    else
                    {
                        //dicomItem
                        if ((dicomItem is DicomElement) && (((DicomElement)dicomItem).Count > 0))
                        {
                            var valueString = ((DicomElement)dicomItem).Get<string>();
                            if (CompareString(valueString, _criteria, false))
                            {
                                //handle match
                                searchResultDictionary.AddResult(filename, dicomItem.Tag, valueString);
                                SearchFileMatch?.Invoke(this, new EventArgs.SearchFileMatchEventArgs { Filename = filename, DicomTag = dicomItem.Tag, ValueString = valueString, SearchResult = searchResultDictionary });
                                //Console.WriteLine($"match value: {dicomItem.ToString()}, {valueString}");
                                break;
                            }
                        }

                        //the following code is too slow!
                        /*
                        for (int i = 0; i < ((DicomElement)dicomItem).Count; i++)
                        {
                            string valueString = ((DicomElement)dicomItem).Get<string>(i);
                            if (CompareString(valueString, _criteria, false))
                            {
                                //handle match
                                searchResultDictionary.AddResult(filename, dicomItem.Tag, valueString);
                                SearchFileMatch?.Invoke(this, new EventArgs.SearchFileMatchEventArgs { Filename = filename, DicomTag = dicomItem.Tag, ValueString = valueString, SearchResult = searchResultDictionary });
                                //Console.WriteLine($"match value: {dicomItem.ToString()}, {valueString}");
                                break;
                            }
                        }
                        */
                    }
                }
            }
            return true;
        }

        private bool CompareString(string refString, SearchCriteria criteria, bool isDicomTag)
        {
            // Dicom tag: case insensitive, ignore some characters
            if (isDicomTag)
            {
                return CompareString(refString.Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", ""),
                                    criteria.Keyword.Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", ""),
                                    false, criteria.WholeWord);
            }
            else
            {
                return CompareString(refString, criteria.Keyword, criteria.CaseSensitive, criteria.WholeWord);
            }
        }

        private bool CompareString(string refString, string testString, bool caseSensitive, bool wholeWord)
        {
            if (!caseSensitive)
            {
                refString = refString.ToUpper();
                testString = testString.ToUpper();
            }

            if (wholeWord)
            {
                return refString.Equals(testString);
            }
            else
            {
                return refString.Contains(testString);
            }
        }
    }
}
