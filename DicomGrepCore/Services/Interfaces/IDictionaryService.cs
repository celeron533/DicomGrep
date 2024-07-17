using FellowOakDicom;

namespace DicomGrepCore.Services.Interfaces
{
    public interface IDictionaryService
    {
        IEnumerable<DicomDictionaryEntry> GetAllDicomTagDefs();
        IEnumerable<DicomUID> GetAllDicomUIDDefs();
        IEnumerable<DicomDictionaryEntry> GetAllPrivateTagDefs();
        void ReadAndAppendCustomDictionaries(string dicomDictionaryFilePath = "DICOM Dictionary.xml", string privateDictionaryFilePath = "Private Dictionary.xml");
    }
}