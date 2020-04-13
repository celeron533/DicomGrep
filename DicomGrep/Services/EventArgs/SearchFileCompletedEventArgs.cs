using Dicom;
using DicomGrep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services.EventArgs
{
    public class SearchFileCompletedEventArgs : System.EventArgs
    {
        public FileResult FileResult { get; set; }
    }
}
