using DicomGrepCore.Entities;
using DicomGrepCore.Services.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DicomGrepCore.Enums;
using FellowOakDicom;
using System.Xml.Linq;
using System.Diagnostics;
using DicomGrepCore.Services.Interfaces;

namespace DicomGrepCore.Services
{
    /// <summary>
    /// The core logic of the search function
    /// </summary>
    public class SearchService : ISearchService
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
                MaxDegreeOfParallelism = criteria.SearchThreads <= 0 ? Environment.ProcessorCount : criteria.SearchThreads,
                CancellationToken = this.token
            };
            try
            {
                Parallel.ForEach(filenameList, options, (filename, loopStat) =>
                 {
                     options.CancellationToken.ThrowIfCancellationRequested();
                     SearchInDicomFile(filename);
                 });
                OnSearchComplete?.Invoke(this, new OnSearchCompleteEventArgs { Reason = Enums.CompleteReasonEnum.Normal });
            }
            catch (OperationCanceledException)
            {
                OnSearchComplete?.Invoke(this, new OnSearchCompleteEventArgs { Reason = Enums.CompleteReasonEnum.UserCancelled });
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
                    DicomSequence sequenceItem = (DicomSequence)dicomItem;

                    if (string.IsNullOrEmpty(criteria.SearchText) &&
                        (string.IsNullOrWhiteSpace(criteria.SearchTag) ||
                         CompareDicomTag(dicomItem.Tag, criteria.DicomSearchTag)))
                    {
                        resultDicomItems ??= new List<ResultDicomItem>();
                        resultDicomItems.Add(new ResultDicomItem(dicomItem.Tag, $"Count = {sequenceItem.Items.Count}", []));
                    }

                    foreach (DicomDataset innerDataset in sequenceItem.Items)
                    {
                        CompareDicomTagAndValue(innerDataset, ref resultDicomItems);
                    }
                }
                else
                {
                    // check the tag first
                    if (string.IsNullOrWhiteSpace(criteria.SearchTag) ||
                         CompareDicomTag(dicomItem.Tag, criteria.DicomSearchTag))
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

                                if (string.IsNullOrWhiteSpace(criteria.SearchText) || CompareString(valueString, criteria, false))
                                {
                                    //handle match
                                    resultDicomItems ??= new List<ResultDicomItem>();

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

        private bool CompareDicomTag(DicomTag dicomTag, DicomTag criteriaDicomTag)
        {
            return criteriaDicomTag.Equals(dicomTag);
        }

        private bool CompareString(string refString, SearchCriteria _criteria, bool isDicomTag)
        {
            if (string.IsNullOrEmpty(refString))
            {
                return false;
            }

            string testString = _criteria.SearchText;

            switch (criteria.MatchPattern)
            {
                default:
                case MatchPatternEnum.Normal:
                    testString = Regex.Escape(testString);
                    break;
                case MatchPatternEnum.Wildcard:
                    testString = Regex.Escape(testString);
                    testString = testString.Replace(@"\*", ".*").Replace(@"\?", ".{1}");
                    break;
                case MatchPatternEnum.Regex:
                    break;
            }

            RegexOptions options = RegexOptions.Compiled | RegexOptions.Singleline;
            if (!_criteria.CaseSensitive)
            {
                options |= RegexOptions.IgnoreCase;
            }

            if (_criteria.WholeWord)
            {
                // regex match start
                if (!testString.StartsWith('^'))
                {
                    testString = "^" + testString;
                }

                // regex match end
                if (!testString.EndsWith('$'))
                {
                    testString += "$";
                }
            }

            return Regex.IsMatch(refString, testString, options);
        }

    }
}
