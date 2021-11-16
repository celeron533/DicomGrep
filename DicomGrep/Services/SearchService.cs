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
        public delegate void ListFileCompletedDelegate(object sender, ListFileCompletedEventArgs e);
        public event ListFileCompletedDelegate FileListCompleted;

        public delegate void OnLoadDicomFileDelegate(object sender, OnLoadDicomFileEventArgs e);
        public event OnLoadDicomFileDelegate OnLoadDicomFile;

        public delegate void OnCompletDicomFileDelegate(object sender, OnCompleteDicomFileEventArgs e);
        public event OnCompletDicomFileDelegate OnCompletDicomFile;

        public delegate void OnSearchCompleteDelegate(object sender, OnSearchCompleteEventArgs e);
        public event OnSearchCompleteDelegate OnSearchComplete;

        private SearchCriteria criteria;
        private IList<string> filenameList = new List<string>();
        private CancellationToken token;

        private int searchedFileCount = 0;
        private int matchedFileCount = 0;


        public void Search(SearchCriteria criteria, CancellationTokenSource tokenSource)
        {
            this.criteria = criteria;
            this.token = tokenSource.Token;

            searchedFileCount = 0;
            matchedFileCount = 0;

            CreateFilenameList();

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

            FileListCompleted?.Invoke(this, new ListFileCompletedEventArgs(filenameList));
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
            { }

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
                { }
            }
        }


        private void SearchInDicomFile(string filePath)
        {
            ResultDicomFile resultDicomFile = null;
            bool isMatched = false;
            try
            {
                OnLoadDicomFile?.Invoke(this, new OnLoadDicomFileEventArgs(filePath));

                DicomFile dicomFile = DicomFile.Open(filePath, FileReadOption.ReadLargeOnDemand, 16 * 1024);
                
                IList<ResultDicomItem> resultDicomItems = null;
                //new DicomDatasetWalker(dicomFile.Dataset).Walk(new DatasetWalker());

                string patientName = string.Empty;
                string sopClassName = string.Empty;
                DicomUID sopClassUID = null;

                dicomFile.Dataset.TryGetString(DicomTag.PatientName, out patientName);
                if (dicomFile.Dataset.TryGetSingleValue<DicomUID>(DicomTag.SOPClassUID, out sopClassUID))
                {
                    sopClassName = sopClassUID?.Name;
                }


                CompareDicomTagAndValue(dicomFile.FileMetaInfo, ref resultDicomItems);
                CompareDicomTagAndValue(dicomFile.Dataset, ref resultDicomItems);

                resultDicomFile = new ResultDicomFile(filePath, sopClassName, sopClassUID?.UID, patientName,
                    resultDicomItems);
                isMatched = resultDicomItems?.Count > 0;
                if (isMatched)
                {
                    Interlocked.Increment(ref matchedFileCount);
                }


            }
            catch (Exception ex)
            {
                //event for error logging
                //throw;

                if (ex is DicomDataException) // normally caused by incorrect Dicom file format
                {
                    // log
                }
                else
                {

                }
            }
            finally
            {
                Interlocked.Increment(ref searchedFileCount);
                OnCompletDicomFile?.Invoke(this,
                    new OnCompleteDicomFileEventArgs(filePath, resultDicomFile, isMatched, searchedFileCount,
                        matchedFileCount));
            }
        }

        private IList<ResultDicomItem> CompareDicomTagAndValue(DicomDataset dataset, ref IList<ResultDicomItem> resultDicomItems)
        {
            foreach (DicomItem dicomItem in dataset)
            {
                // contains sub sequence
                if (dicomItem.ValueRepresentation == DicomVR.SQ)
                {
                    foreach (DicomDataset innerDataset in ((DicomSequence)dicomItem).Items)
                    {
                        CompareDicomTagAndValue(innerDataset, ref resultDicomItems);
                    }
                }
                else
                {

                    if (criteria.SearchDicomTag)
                    {
                        if (CompareString(dicomItem.Tag.ToString(), criteria, true))
                        {
                            //handle match
                            var valueString = ((DicomElement)dicomItem).Get<string>();

                            if (resultDicomItems == null)
                            {
                                resultDicomItems = new List<ResultDicomItem>();
                            }

                            resultDicomItems.Add(new ResultDicomItem(dicomItem.Tag, valueString, Enums.ResultTypeEnum.Tag));

                            //Console.WriteLine($"match tag: {dicomItem.ToString()}");
                        }
                    }

                    if (criteria.SearchDicomValue)
                    {
                        // skip binary VRs
                        if (dicomItem.ValueRepresentation == DicomVR.OB ||
                            dicomItem.ValueRepresentation == DicomVR.OF ||
                            dicomItem.ValueRepresentation == DicomVR.OW)
                        {
                            continue;
                        }
                        // compare value
                        else
                        {
                            //dicomItem
                            if ((dicomItem is DicomElement { Count: > 0 } element))
                            {
                                var valueString = element.Get<string>();
                                if (CompareString(valueString, criteria, false))
                                {
                                    //handle match
                                    if (resultDicomItems == null)
                                    {
                                        resultDicomItems = new List<ResultDicomItem>();
                                    }

                                    resultDicomItems.Add(new ResultDicomItem(element.Tag, valueString, Enums.ResultTypeEnum.ValueString));

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

            // Dicom tag: case insensitive, ignore some characters
            if (isDicomTag)
            {
                return CompareString(refString.Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", ""),
                                    criteria.SearchTextForTag,
                                    false, criteria.WholeWord);
            }
            else
            {
                return CompareString(refString, criteria.SearchText, criteria.CaseSensitive, criteria.WholeWord);
            }
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
