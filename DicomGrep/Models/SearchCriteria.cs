using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DicomGrep.Models
{
    public class SearchCriteria
    {
        public string SearchPath { get; set; }
        public string SearchSopClassUid { get; set; }

        private string _searchTag;

        public string SearchTag
        {
            get => _searchTag;
            set
            {
                _searchTag = value;
                _searchTagFlattened = value?.Substring(0, value.IndexOf(':') > 0 ? value.IndexOf(':') : value.Length)
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(",", "")
                    .Replace(".", "")
                    .Replace(" ", "")
                    .ToUpper();
            }
        }

        private string _searchTagFlattened;
        [JsonIgnore]
        public string SearchTagFlattened => _searchTagFlattened;

        public string FileTypes { get; set; }

        public string SearchText { get; set; }

        public bool CaseSensitive { get; set; }
        public bool WholeWord { get; set; }
        public bool IncludeSubfolders { get; set; }

        public int SearchThreads { get; set; }
    }
}
