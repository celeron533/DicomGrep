namespace DicomGrep.Services.Interfaces
{
    public interface IFolderPickupService
    {
        bool SelectFolder(ref string folderPath);
    }
}