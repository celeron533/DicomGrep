using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Models
{
    public class ResultDicomFile
    {
        public string Filename { get; private set; }
        public string SOPClassName { get; private set; }
        public string SOPClassUID { get; private set; }
        public string PatientName { get; private set; }
        public IList<ResultDicomItem> ResultDicomItems { get; set; }

        public ResultDicomFile(string filename, string SOPClassName, string SOPClassUID, string PatientName, IList<ResultDicomItem> resultDicomItems)
        {
            this.Filename = filename;
            this.SOPClassName = SOPClassName;
            this.SOPClassUID = SOPClassUID;
            this.ResultDicomItems = resultDicomItems;
        }
    }
}
