﻿using DicomGrepCore.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrepCore.Services.EventArgs
{
    public class OnSearchCompleteEventArgs : System.EventArgs
    {
        public CompleteReasonEnum Reason { get; set; }
    }
}
