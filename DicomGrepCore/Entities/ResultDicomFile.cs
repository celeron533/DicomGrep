using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DicomGrepCore.Entities
{
    public class ResultDicomFile
    {
        public string FullFilename { get; private set; }
        public string Filename => Path.GetFileName(FullFilename);
        public string DirectoryName => Path.GetDirectoryName(FullFilename);

        public string SOPClassName { get; private set; }
        public string SOPClassUID { get; private set; }
        public string PatientName { get; private set; }
        public IList<ResultDicomItem> ResultDicomItems { get; set; }

        public ResultDicomFile(string filename, string SOPClassName, string SOPClassUID, string PatientName, IList<ResultDicomItem> resultDicomItems)
        {
            this.FullFilename = filename;
            this.SOPClassName = SOPClassName;
            this.SOPClassUID = SOPClassUID;
            this.PatientName = PatientName;
            this.ResultDicomItems = resultDicomItems;
        }
    }
}
