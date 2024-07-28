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
            return ToString("c");
        }

        /// <summary>
        /// Format the search criteria.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            switch (format)
            {
                case "c":
                default:
                    return string.Join(", ", CriteriaSummary().Select(x => $"{x.Key}={x.Value}"));
                case "n":
                    return string.Join(Environment.NewLine, CriteriaSummary().Select(x => $"{x.Key}={x.Value}"));
                case "n1":
                    return string.Join(Environment.NewLine, CriteriaSummary().Select(x => $"> {x.Key}={x.Value}"));

            }
        }

        private List<KeyValuePair<string,string>> CriteriaSummary()
        {
            List<KeyValuePair<string, string>> summary= new List<KeyValuePair<string, string>>();
            summary.Add(new KeyValuePair<string, string>("SearchPath", Quote(SearchPath)));
            summary.Add(new KeyValuePair<string, string>("FileTypes", Quote(FileTypes)));
            summary.Add(new KeyValuePair<string, string>("IncludeSubfolders", IncludeSubfolders.ToString()));
            summary.Add(new KeyValuePair<string, string>("SearchSopClassUid", Quote(SearchSopClassUid)));
            summary.Add(new KeyValuePair<string, string>("SearchTag", Quote(SearchTag)));
            summary.Add(new KeyValuePair<string, string>("SearchText", Quote(SearchText)));
            summary.Add(new KeyValuePair<string, string>("MatchPattern", MatchPattern.ToString()));
            summary.Add(new KeyValuePair<string, string>("CaseSensitive", CaseSensitive.ToString()));
            summary.Add(new KeyValuePair<string, string>("WholeWord", WholeWord.ToString()));
            summary.Add(new KeyValuePair<string, string>("SearchInResults", SearchInResults.ToString()));
            summary.Add(new KeyValuePair<string, string>("SearchThreads", SearchThreads.ToString()));
            return summary;
        }

        private string Quote(string value) => $"\"{value}\"";

        private DicomTag ParseDicomTag(string value)
        {
            // escape the "xx" in private tag string. No worries, it will apply the 0xFF mask again during the search.
            value = value.Replace(",xx", ",00").Replace(",XX", ",00").Replace(",Xx", ",00").Replace(",xX", ",00");
            return DicomTag.Parse(value);
        }
    }
}
