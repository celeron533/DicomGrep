using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Models
{
    public class Configuration
    {
        public List<string> SearchPathHistory { get; set; }
        public List<string> FileTypesHistory { get; set; }
        public List<string> SopClassUidHistory { get; set; }
        public List<string> DicomTagHistory { get; set; }
        public List<string> SearchTextHistory { get; set; }

        // todo: other configuration item
        public int HistoryCapacity { get; set; }
        public SearchCriteria SearchCriteria { get; set; }
    }
}
