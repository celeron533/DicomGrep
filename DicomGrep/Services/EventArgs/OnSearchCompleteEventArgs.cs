using DicomGrep.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Services.EventArgs
{
    public class OnSearchCompleteEventArgs : System.EventArgs
    {
        public ReasonEnum Reason { get; set; }
    }
}
