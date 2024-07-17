namespace DicomGrep.Services.Interfaces
{
    public interface IFileOperationService
    {
        bool CopyFullNamePath(string filePath);
        bool CopyName(string filePath);
        bool OpenDirectory(string filePath);
        bool OpenFile(string filePath);
    }
}