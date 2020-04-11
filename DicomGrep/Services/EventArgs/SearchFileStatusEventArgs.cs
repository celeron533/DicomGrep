using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services.EventArgs
{
    public class SearchFileStatusEventArgs : System.EventArgs
    {
        public string Filename { get; set; }
        public int Index { get; set; }
    }
}
