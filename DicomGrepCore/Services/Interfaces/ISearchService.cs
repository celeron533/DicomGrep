using DicomGrepCore.Entities;

namespace DicomGrepCore.Services.Interfaces
{
    public interface ISearchService
    {
        /// <summary>
        /// Generate the file list to be searched based on the search criteria.
        /// </summary>
        event SearchService.ListFileCompletedDelegate FileListCompleted;

        /// <summary>
        /// Load a DICOM file and search for the search criteria.
        /// </summary>
        event SearchService.OnLoadDicomFileDelegate OnLoadDicomFile;

        /// <summary>
        /// Finish searching a DICOM file.
        /// </summary>
        event SearchService.OnCompletDicomFileDelegate OnCompletDicomFile;

        /// <summary>
        /// Finish searching all DICOM files.
        /// </summary>
        event SearchService.OnAllSearchCompleteDelegate OnAllSearchComplete;

        void Search(SearchCriteria criteria, CancellationTokenSource tokenSource);
    }
}