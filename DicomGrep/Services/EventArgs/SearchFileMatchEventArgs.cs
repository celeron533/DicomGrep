using Dicom;
using DicomGrep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services.EventArgs
{
    public class SearchFileMatchEventArgs : System.EventArgs
    {
        public string Filename { get; set; }
        public DicomTag DicomTag { get; set; }
        public string ValueString { get; set; }

        public SearchResultCollection SearchResult { get; set; }
    }
}
