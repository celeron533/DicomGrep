using DicomGrep.Models;
using DicomGrep.Services.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services.Interfaces
{
    public delegate void SearchFileMatchDelegate(object sender, SearchFileMatchEventArgs e);

    public delegate void SearchFileCompletedDelegate(object sender, SearchFileCompletedEventArgs e);

    /// <summary>
    /// Search in one DICOM file
    /// </summary>
    public interface IDicomSearchService
    {
        event SearchFileMatchDelegate SearchFileMatch;

        event SearchFileCompletedDelegate SearchFileCompleted;

        void SearchInDicomFile(string path, SearchCriteria searchCriteria);
    }
}
