namespace DicomGrep.Services.Interfaces
{
    public interface ISopClassLookupService
    {
        bool SelectSopClass(ref string sopClassUidString);
    }
}