using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Models
{
    public class SearchCriteria
    {
        public string SearchPath { get; set; }
        public string FileTypes { get; set; }

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                SearchTextForTag = _searchText.Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", "");
            }
        }

        public string SearchTextForTag { get; private set; }

        public bool SearchDicomTag { get; set; }
        public bool SearchDicomValue { get; set; }
        public bool CaseSensitive { get; set; }
        public bool WholeWord { get; set; }
        public bool IncludeSubfolders { get; set; }
        public bool IncludePrivateTag { get; set; }

        public int SearchThreads { get; set; }
    }
}
