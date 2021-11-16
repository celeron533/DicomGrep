using DicomGrep.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using FellowOakDicom;

namespace DicomGrep.Models
{
    public class ResultDicomItem
    {
        public DicomTag Tag { get; private set; }
        public string ValueString { get; private set; }
        public ResultTypeEnum Type { get; private set; }

        public ResultDicomItem(DicomTag tag, string valueString, ResultTypeEnum type)
        {
            this.Tag = tag;
            this.ValueString = valueString;
            this.Type = type;
        }
    }
}
