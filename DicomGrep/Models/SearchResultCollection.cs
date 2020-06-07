using Caliburn.Micro;
using Dicom;
using DicomGrep.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Models
{
    /// <summary>
    /// Filename + Matches in the file
    /// </summary>
    public class SearchResultCollection : ObservableCollection<FileResult>
    {
        
    }

    public class FileResult
    {
        public string FileFullPath { get; set; }

        public string FileName => System.IO.Path.GetFileName(FileFullPath);

        public string Path => System.IO.Path.GetDirectoryName(FileFullPath);
        public string SOPClassUID { get; set; }

        public string SOPClassName { get; set; }
        public string PatientName { get; set; }

        public ResultItemCollection ResultItemCollection { get; set; }
    }

    public class ResultItemCollection : ObservableCollection<ResultItem>
    {

    }

    public class ResultItem
    {
        public DicomTag Tag { get; set; }

        public string TagString => Tag.ToString();

        public string TagName => Tag.DictionaryEntry.Name;

        public string ValueString { get; set; }

        public ResultType ResultType { get; set; }
    }
}
