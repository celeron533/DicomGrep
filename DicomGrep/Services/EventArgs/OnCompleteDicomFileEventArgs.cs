using DicomGrep.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Services.EventArgs
{
    public class OnCompleteDicomFileEventArgs : System.EventArgs
    {
        public string Filename { get; private set; }
        public ResultDicomFile ResultDicomFile { get; private set; }
        public bool IsMatch { get; private set; }
        public int SearchedFileCount { get; private set; }
        public int MatchFileCount { get; private set; }

        public OnCompleteDicomFileEventArgs(string filename, ResultDicomFile resultDicomFile, bool isMatch, int searchedFileCount, int matchFileCount)
        {
            this.Filename = filename;
            this.ResultDicomFile = resultDicomFile;
            this.IsMatch = isMatch;
            this.SearchedFileCount = searchedFileCount;
            this.MatchFileCount = matchFileCount;
        }
    }
}
