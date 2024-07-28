using DicomGrepCore.Entities;

namespace DicomGrepCore.Services.Interfaces
{
    public interface ISearchService
    {
        event SearchService.ListFileCompletedDelegate FileListCompleted;
        event SearchService.OnCompletDicomFileDelegate OnCompletDicomFile;
        event SearchService.OnLoadDicomFileDelegate OnLoadDicomFile;
        event SearchService.OnSearchCompleteDelegate OnSearchComplete;

        void Search(SearchCriteria criteria, CancellationTokenSource tokenSource);
    }
}