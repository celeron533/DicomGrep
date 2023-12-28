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
                    .Replace("x", "0") //private tag
                    .Replace("X", "0") //private tag
                    .ToUpper();
            }
        }

        /// <summary>
        /// Only Group & Element, not considering the private creator
        /// </summary>
        private string _searchTagFlattened;
        [JsonIgnore]
        public string SearchTagFlattened => _searchTagFlattened;

        public string FileTypes { get; set; }

        public string SearchText { get; set; }

        public bool CaseSensitive { get; set; }
        public bool WholeWord { get; set; }
        public bool IncludeSubfolders { get; set; }

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
                $"CaseSensitive = {CaseSensitive}, " +
                $"WholeWord= {WholeWord}, " +
                $"IncludeSubfolders = {IncludeSubfolders}, " +
                $"SearchInResults = {SearchInResults}, " +
                $"SearchThreads = {SearchThreads}";
        }
    }
}
