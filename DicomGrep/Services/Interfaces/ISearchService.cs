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

    public delegate void SearchFileMatchDelegate(object sender, SearchFileMatchEventArgs e);


    public interface ISearchService
    {

        event EventHandler SearchStarted;

        event EventHandler SearchCompleted;

        event FileListCompletedDelegate FileListCompleted;

        event SearchFileStatusDelegate SearchFileStatus;

        event SearchFileMatchDelegate SearchFileMatch;

        //event EventHandler SearchMatchChanged;

        void Search(SearchCriteria searchCriteria);

        void Cancel();
    }
}
