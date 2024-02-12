using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using DicomGrepCore.Enums;

namespace DicomGrepCore.Entities
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
                try
                {
                    _dicomSearchTag = ParseDicomTag(value);
                }
                catch 
                {
                    // unable to parse the string
                }
            }
        }

        private DicomTag _dicomSearchTag;
        [JsonIgnore]
        public DicomTag DicomSearchTag => _dicomSearchTag;

        public string FileTypes { get; set; }

        public string SearchText { get; set; }

        public bool CaseSensitive { get; set; }
        public bool WholeWord { get; set; }
        public bool IncludeSubfolders { get; set; }
        public MatchPatternEnum MatchPattern { get; set; }

        [JsonIgnore]
        public bool SearchInResults { get; set; }

        public int SearchThreads { get; set; }

        public override string ToString()
        {
            return
                $"SearchPath = '{SearchPath}', " +
                $"SearchSopClassUid = '{SearchSopClassUid}', " +
                $"SearchTag = '{SearchTag}', " +
                $"FileTypes = '{FileTypes}', " +
                $"SearchText = '{SearchText}', " +
                $"MatchPattern = {MatchPattern}, " +
                $"CaseSensitive = {CaseSensitive}, " +
                $"WholeWord = {WholeWord}, " +
                $"IncludeSubfolders = {IncludeSubfolders}, " +
                $"SearchInResults = {SearchInResults}, " +
                $"SearchThreads = {SearchThreads}";
        }

        private DicomTag ParseDicomTag(string value)
        {
            // escape the "xx" in private tag string. No worries, it will apply the 0xFF mask again during the search.
            value = value.Replace(",xx", ",00").Replace(",XX", ",00").Replace(",Xx", ",00").Replace(",xX", ",00");
            return DicomTag.Parse(value);
        }
    }
}
