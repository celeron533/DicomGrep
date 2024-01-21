using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrepCore.Services.EventArgs
{
    public class ListFileCompletedEventArgs : System.EventArgs
    {
        public IList<string> Filenames { get; private set; }
        public int Count => Filenames == null ? 0 : Filenames.Count;

        public ListFileCompletedEventArgs(IList<string> filenames)
        {
            this.Filenames = filenames;
        }
    }
}
