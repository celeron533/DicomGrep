using DicomGrep.Models;
using DicomGrep.Services.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services.Interfaces
{
    public delegate void FileListCompletedDelegate(object sender, FileListCompletedEventArgs e);

    public delegate void SearchFileStatusDelegate(object sender, SearchFileStatusEventArgs e);

    public interface ISearchService
    {
        event EventHandler SearchStarted;

        event EventHandler SearchCompleted;

        event FileListCompletedDelegate FileListCompleted;

        event SearchFileStatusDelegate SearchFileStatus;

        void Search(SearchCriteria searchCriteria, IDicomSearchService dicomSearchService);

        void Cancel();
    }
}
