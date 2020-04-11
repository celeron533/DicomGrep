using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services.EventArgs
{
    public class FileListCompletedEventArgs : System.EventArgs
    {
        public int TotalFileCount { get; set; }
    }
}
