using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrepCore.Services.EventArgs
{
    public class OnLoadDicomFileEventArgs : System.EventArgs
    {
        public string Filename { get; private set; }
        public OnLoadDicomFileEventArgs(string filename)
        {
            this.Filename = filename;
        }
    }
}
