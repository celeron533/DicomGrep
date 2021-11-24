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

        [JsonIgnore]
        public DicomTag ParsedDicomTag { get; private set; } = null;

        private string _searchTag;
        public string SearchTag 
        {
            get => _searchTag;
            set 
            {
                _searchTag = value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    ParsedDicomTag = null;
                }
                else
                {
                    try
                    {
                        ParsedDicomTag = DicomTag.Parse(value);
                    }
                    catch
                    {
                        ParsedDicomTag = null;
                    }
                }
            }
        }

        public string FileTypes { get; set; }

        public string SearchText { get; set; }

        public bool CaseSensitive { get; set; }
        public bool WholeWord { get; set; }
        public bool IncludeSubfolders { get; set; }
        public bool IncludePrivateTag { get; set; }

        public int SearchThreads { get; set; }
    }
}
