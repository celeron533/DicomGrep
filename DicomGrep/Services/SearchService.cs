using Dicom;
using DicomGrep.Extensions;
using DicomGrep.Models;
using DicomGrep.Services.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private SearchCriteria criteria;
        private IList<string> result = new List<string>();


        public void Search(SearchCriteria criteria)
        {
            this.criteria = criteria;

            ListFilename();

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 6 };
            Parallel.ForEach(result, options, filename =>
             {
                 SearchInDicomFile(filename);
             });
        }



        private void ListFilename()
        {
            result.Clear();

            if (Directory.Exists(criteria.SearchPath))
            {
                LoopupDirectory(criteria.SearchPath);
            }

            FileListCompleted?.Invoke(this, new ListFileCompletedEventArgs(result));
        }

        private void LoopupDirectory(string directoryPath)
        {
            try
            {
                Array.ForEach(Directory.GetFiles(directoryPath, criteria.FileTypes), fn => result.Add(fn));
            }
            catch (Exception e)
            { }

            if (criteria.IncludeSubfolders)
            {
                try
                {
                    foreach (string subdirectory in Directory.GetDirectories(directoryPath))
                    {
                        LoopupDirectory(subdirectory);
                    }
                }
                catch (Exception e)
                { }
            }
        }




        public void SearchInDicomFile(string filePath)
        {
            try
            {
                OnLoadDicomFile?.Invoke(this, new OnLoadDicomFileEventArgs(filePath));

                DicomFile dicomFile = DicomFile.Open(filePath);
                ResultDicomFile resultDicomFile = null;
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

                resultDicomFile = new ResultDicomFile(filePath, sopClassName, sopClassUID?.UID, patientName, resultDicomItems);
                bool isMatched = resultDicomItems?.Count > 0;

                OnCompletDicomFile?.Invoke(this, new OnCompleteDicomFileEventArgs(filePath, resultDicomFile, isMatched));
            }
            catch (Exception ex)
            {
                //event for error logging
                //throw;
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

                            resultDicomItems.Add(new ResultDicomItem(dicomItem.Tag, valueString, Enums.ResultType.Tag));

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
                            if ((dicomItem is DicomElement) && (((DicomElement)dicomItem).Count > 0))
                            {
                                var valueString = ((DicomElement)dicomItem).Get<string>();
                                if (CompareString(valueString, criteria, false))
                                {
                                    //handle match
                                    if (resultDicomItems == null)
                                    {
                                        resultDicomItems = new List<ResultDicomItem>();
                                    }

                                    resultDicomItems.Add(new ResultDicomItem(dicomItem.Tag, valueString, Enums.ResultType.ValueString));

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
            // Dicom tag: case insensitive, ignore some characters
            if (isDicomTag)
            {
                return CompareString(refString.Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", ""),
                                    criteria.SearchText.Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", ""),
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
