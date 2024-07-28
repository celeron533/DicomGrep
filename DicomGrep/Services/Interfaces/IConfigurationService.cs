using DicomGrep.Models;

namespace DicomGrep.Services.Interfaces
{
    public interface IConfigurationService
    {
        Configuration GetConfiguration();
        bool Load();
        bool Save();
        void SetConfiguration(Configuration config);
    }
}