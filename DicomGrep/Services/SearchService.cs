using DicomGrep.Extensions;
using DicomGrep.Models;
using DicomGrep.Services.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FellowOakDicom;

namespace DicomGrep.Services
{
    public class SearchService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public delegate void ListFileCompletedDelegate(object sender, ListFileCompletedEventArgs e);
        public event ListFileCompletedDelegate FileListCompleted;

        public delegate void OnLoadDicomFileDelegate(object sender, OnLoadDicomFileEventArgs e);
        public event OnLoadDicomFileDelegate OnLoadDicomFile;

        public delegate void OnCompletDicomFileDelegate(object sender, OnCompleteDicomFileEventArgs e);
        public event OnCompletDicomFileDelegate OnCompletDicomFile;

        public delegate void OnSearchCompleteDelegate(object sender, OnSearchCompleteEventArgs e);
        public event OnSearchCompleteDelegate OnSearchComplete;

        private SearchCriteria criteria;
        private List<string> filenameList = new List<string>();
        private List<string> matchFilenameList = new List<string>();
        private CancellationToken token;

        private int searchedFileCount = 0;
        private int matchFileCount = 0;


        public void Search(SearchCriteria criteria, CancellationTokenSource tokenSource)
        {
            logger.Info(criteria.ToString());

            this.criteria = criteria;
            this.token = tokenSource.Token;

            searchedFileCount = 0;
            

            if (criteria.SearchInResults)
            {
                // use the filename list from previous search result
                filenameList.Clear();
                filenameList.AddRange(matchFilenameList);
            }
            else
            {
                // construct new filename list
                CreateFilenameList();
            }

            FileListCompleted?.Invoke(this, new ListFileCompletedEventArgs(filenameList));

            matchFilenameList.Clear();
            matchFileCount = 0;

            ParallelOptions options = new ParallelOptions 
            {
                MaxDegreeOfParallelism = criteria.SearchThreads,
                CancellationToken = this.token
            };
            try
            {
                Parallel.ForEach(filenameList, options, (filename, loopStat) =>
                 {
                     options.CancellationToken.ThrowIfCancellationRequested();
                     SearchInDicomFile(filename);
                 });
                OnSearchComplete?.Invoke(this, new OnSearchCompleteEventArgs { Reason = Enums.ReasonEnum.Normal });
            }
            catch (OperationCanceledException)
            {
                OnSearchComplete?.Invoke(this, new OnSearchCompleteEventArgs { Reason = Enums.ReasonEnum.UserCancelled });
                logger.Info("User cancelled the search.");
            }
            finally
            {
                tokenSource.Dispose();
            }
        }



        private void CreateFilenameList()
        {
            filenameList.Clear();

            if (Directory.Exists(criteria.SearchPath))
            {
                LookupDirectory(criteria.SearchPath);
            }
        }

        private void LookupDirectory(string directoryPath)
        {
            try
            {
                if (token.IsCancellationRequested)
                    return;
                else
                    Array.ForEach(Directory.GetFiles(directoryPath, criteria.FileTypes), fn => filenameList.Add(fn));
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unable to access files in: '{directoryPath}'");
            }

            if (criteria.IncludeSubfolders)
            {
                try
                {
                    foreach (string subdirectory in Directory.GetDirectories(directoryPath))
                    {
                        if (token.IsCancellationRequested)
                            return;
                        else
                            LookupDirectory(subdirectory);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Unable to access directory: '{directoryPath}'");
                }
            }
        }


        private void SearchInDicomFile(string filePath)
        {
            ResultDicomFile resultDicomFile = null;
            bool anyMatch = false;
            try
            {
                OnLoadDicomFile?.Invoke(this, new OnLoadDicomFileEventArgs(filePath));

                DicomFile dicomFile = DicomFile.Open(filePath, FileReadOption.ReadLargeOnDemand, 16 * 1024);
                
                IList<ResultDicomItem> resultDicomItems = null;
                //new DicomDatasetWalker(dicomFile.Dataset).Walk(new DatasetWalker());

                string patientName = string.Empty;
                string sopClassName = string.Empty;
                DicomUID sopClassUID = null;


                if (dicomFile.Dataset.TryGetSingleValue<DicomUID>(DicomTag.SOPClassUID, out sopClassUID))
                {
                    // compare the sop class uid
                    if (!string.IsNullOrWhiteSpace(criteria.SearchSopClassUid) && sopClassUID.UID != criteria.SearchSopClassUid)
                    {
                        return;
                    }
                    sopClassName = sopClassUID?.Name;
                }
                dicomFile.Dataset.TryGetString(DicomTag.PatientName, out patientName);

                CompareDicomTagAndValue(dicomFile.FileMetaInfo, ref resultDicomItems);
                CompareDicomTagAndValue(dicomFile.Dataset, ref resultDicomItems);

                resultDicomFile = new ResultDicomFile(filePath, sopClassName, sopClassUID?.UID, patientName,
                    resultDicomItems);
                anyMatch = resultDicomItems?.Count > 0;
                if (anyMatch)
                {
                    matchFilenameList.Add(filePath);
                    Interlocked.Increment(ref matchFileCount);
                }


            }
            catch (Exception ex)
            {
                if (ex is DicomDataException) // normally caused by incorrect Dicom file format
                {
                    logger.Error(ex, $"'{filePath}' is not a valid DICOM file.");
                }
                else
                {
                    logger.Warn(ex);
                }
            }
            finally
            {
                Interlocked.Increment(ref searchedFileCount);
                OnCompletDicomFile?.Invoke(this,
                    new OnCompleteDicomFileEventArgs(filePath, resultDicomFile, anyMatch, searchedFileCount,
                        matchFileCount));
            }
        }

        private IList<ResultDicomItem> CompareDicomTagAndValue(DicomDataset dataset, ref IList<ResultDicomItem> resultDicomItems)
        {
            foreach (DicomItem dicomItem in dataset)
            {
                // dig into sub sequence
                if (dicomItem.ValueRepresentation == DicomVR.SQ)
                {
                    foreach (DicomDataset innerDataset in ((DicomSequence)dicomItem).Items)
                    {
                        CompareDicomTagAndValue(innerDataset, ref resultDicomItems);
                    }
                }
                else
                {
                    // check the tag first
                    if ( string.IsNullOrEmpty(criteria.SearchTagFlattened) ||
                         dicomItem.Tag.ToString("J",null) == criteria.SearchTagFlattened) // both of the string are in UPPER case
                    {
                        // skip binary VRs
                        if (dicomItem.ValueRepresentation == DicomVR.OB ||
                            dicomItem.ValueRepresentation == DicomVR.OF ||
                            dicomItem.ValueRepresentation == DicomVR.OW)
                        {
                            continue;
                        }
                        // then compare tag value
                        else
                        {
                            // dicomItem
                            if ((dicomItem is DicomElement { Count: > 0 } element))
                            {
                                string valueString = element.Get<string>();
                                byte[] rawValue = new byte[element.Buffer.Size];
                                Array.Copy(element.Buffer.Data, rawValue, element.Buffer.Size);

                                // best guess for VR=UN
                                if (dicomItem.ValueRepresentation == DicomVR.UN)
                                {
                                    byte[] bytes = element.Get<byte[]>();
                                    if (bytes != null && bytes.Length > 1)
                                    {
                                        valueString = Encoding.ASCII.GetString(bytes);
                                    }
                                }
                                
                                if ( string.IsNullOrWhiteSpace(criteria.SearchText) || CompareString(valueString, criteria, false))
                                {
                                    //handle match
                                    if (resultDicomItems == null)
                                    {
                                        resultDicomItems = new List<ResultDicomItem>();
                                    }

                                    resultDicomItems.Add(new ResultDicomItem(element.Tag, valueString, rawValue));

                                    //Console.WriteLine($"match value: {dicomItem.ToString()}, {valueString}");
                                    
                                }
                            }

                            //the following code is too slow!
                            /*
                            for (int i = 0; i < ((DicomElement)dicomItem).Count; i++)
                            {
                                string valueString = ((DicomElement)dicomItem).Get<string>(i);
                                if (CompareString(valueString, _criteria, false))
                                {
                                    ......
                                    break;
                                }
                            }
                            */
                        }
                    }

                }
            }
            return resultDicomItems;
        }

        private bool CompareString(string refString, SearchCriteria criteria, bool isDicomTag)
        {
            if (string.IsNullOrEmpty(refString))
            {
                return false;
            }

            return CompareString(refString, criteria.SearchText, criteria.CaseSensitive, criteria.WholeWord);

        }

        private bool CompareString(string refString, string testString, bool caseSensitive, bool wholeWord)
        {
            if (wholeWord)
            {
                if (caseSensitive)
                    return refString.Equals(testString, StringComparison.InvariantCulture);
                else
                    return refString.Equals(testString, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                if (caseSensitive)
                    return refString.CaseInsensitiveContains(testString, StringComparison.InvariantCulture);
                else
                    return refString.CaseInsensitiveContains(testString, StringComparison.InvariantCultureIgnoreCase);
            }
        }


    }
}
