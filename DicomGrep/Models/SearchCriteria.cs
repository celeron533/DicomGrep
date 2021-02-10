using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Models
{
    public class SearchCriteria
    {
        public string SearchPath { get; set; }
        public string FileTypes { get; set; }
        public string SearchText { get; set; }

        public bool SearchDicomTag { get; set; }
        public bool SearchDicomValue { get; set; }
        public bool CaseSensitive { get; set; }
        public bool WholeWord { get; set; }
        public bool IncludeSubfolders { get; set; }
        public bool IncludePrivateTag { get; set; }

        public int SearchThreads { get; set; }
    }
}
