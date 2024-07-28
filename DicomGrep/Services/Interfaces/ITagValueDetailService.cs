using DicomGrepCore.Entities;

namespace DicomGrep.Services.Interfaces
{
    public interface ITagValueDetailService
    {
        void InspectTagValue(ResultDicomItem item);
    }
}