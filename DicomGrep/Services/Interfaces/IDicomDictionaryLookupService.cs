namespace DicomGrep.Services.Interfaces
{
    public interface IDicomDictionaryLookupService
    {
        bool SelectDicomDictionaryEntry(ref string dicomTagString);
    }
}