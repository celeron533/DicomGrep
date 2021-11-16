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
        public bool IsMatched { get; private set; }
        public int SearchedFileCount { get; private set; }
        public int MatchedFileCount { get; private set; }

        public OnCompleteDicomFileEventArgs(string filename, ResultDicomFile resultDicomFile, bool isMatched, int searchedFileCount, int matchedFileCount)
        {
            this.Filename = filename;
            this.ResultDicomFile = resultDicomFile;
            this.IsMatched = isMatched;
            this.SearchedFileCount = searchedFileCount;
            this.MatchedFileCount = matchedFileCount;
        }
    }
}
