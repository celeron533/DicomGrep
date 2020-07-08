using Dicom;
using DicomGrep.Extensions;
using DicomGrep.Models;
using DicomGrep.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DicomGrep.Services
{
    /// <summary>
    /// Search in one DICOM file
    /// </summary>
    public class DicomSearchService : IDicomSearchService
    {
        SearchCriteria _criteria;

        public event SearchFileMatchDelegate SearchFileMatch;
        public event SearchFileCompletedDelegate SearchFileCompleted;

        /// <summary>
        /// Search in a Dicom file with provided search criteria
        /// </summary>
        /// <param name="path"></param>
        /// <param name="criteria"></param>
        public void SearchInDicomFile(string path, SearchCriteria criteria)
        {
            this._criteria = criteria;
            try
            {
                DicomFile dicomFile = DicomFile.Open(path);
                FileResult fileResult = null;
                //new DicomDatasetWalker(dicomFile.Dataset).Walk(new DatasetWalker());

                string patientName = string.Empty;
                string sopClassName = string.Empty;
                DicomUID sopClassUID = null;

                dicomFile.Dataset.TryGetString(DicomTag.PatientName, out patientName);
                if (dicomFile.Dataset.TryGetSingleValue<DicomUID>(DicomTag.SOPClassUID, out sopClassUID))
                {
                    sopClassName = sopClassUID?.Name;
                }

                fileResult = new FileResult { FileFullPath = path, SOPClassName = sopClassName, SOPClassUID = sopClassUID?.UID, PatientName = patientName, ResultItemCollection = null };

                CompareDicomTagAndValue(dicomFile.FileMetaInfo, fileResult, path, sopClassName, patientName);
                CompareDicomTagAndValue(dicomFile.Dataset, fileResult, path, sopClassName, patientName);

                SearchFileCompleted?.Invoke(this, new EventArgs.SearchFileCompletedEventArgs { FileResult = fileResult });
            }
            catch (Exception ex)
            {
                //event for error logging
                throw;
            }
        }

        private bool CompareDicomTagAndValue(DicomDataset dataset, FileResult fileResult, string filename, string sopClassName, string patientName)
        {
            foreach (DicomItem dicomItem in dataset)
            {
                if (_criteria.SearchDicomTag)
                {
                    if (CompareString(dicomItem.Tag.ToString(), _criteria, true))
                    {
                        //handle match
                        var valueString = ((DicomElement)dicomItem).Get<string>();

                        if (fileResult.ResultItemCollection == null)
                        {
                            fileResult.ResultItemCollection = new ResultItemCollection();
                        }

                        fileResult.ResultItemCollection.Add(new ResultItem { Tag = dicomItem.Tag, ValueString = valueString, ResultType = Enums.ResultType.Tag });
                        SearchFileMatch?.Invoke(this, new EventArgs.SearchFileMatchEventArgs { Filename = filename, DicomTag = dicomItem.Tag, ValueString = valueString });
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
                            CompareDicomTagAndValue(innerDataset, fileResult, filename, sopClassName, patientName);
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
                                if (fileResult.ResultItemCollection == null)
                                {
                                    fileResult.ResultItemCollection = new ResultItemCollection();
                                }

                                fileResult.ResultItemCollection.Add(new ResultItem { Tag = dicomItem.Tag, ValueString = valueString, ResultType = Enums.ResultType.ValueString });
                                SearchFileMatch?.Invoke(this, new EventArgs.SearchFileMatchEventArgs { Filename = filename, DicomTag = dicomItem.Tag, ValueString = valueString });
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
