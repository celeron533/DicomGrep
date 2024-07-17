using DicomGrepCore.Entities;
using System.Collections.Generic;

namespace DicomGrep.Services.Interfaces
{
    public interface IExportService
    {
        void Export(List<ResultDicomFile> resultFiles, string exportTo = "");
    }
}