using DicomGrep.Models;
using DicomGrepCore.Entities;
using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.ViewModels
{
    public class TagValueDetailViewModel : ViewModelBase
    {
        private ResultDicomItem _dicomItem;
        public ResultDicomItem DicomItem
        {
            get => _dicomItem;
            set
            {
                SetProperty(ref _dicomItem, value);
                Summary = GenerateSummaryDisplayString();
            }
        }

        private string _summary;
        public string Summary
        {
            get => _summary;
            private set => SetProperty(ref _summary, value);
        }

        string GenerateSummaryDisplayString()
        {
            if (DicomItem == null) { return "NULL"; }

            DicomDictionaryEntry dic = DicomItem.Tag.DictionaryEntry;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{DicomItem.Tag}")
                .AppendLine($" Name:    {dic.Name}")
                .AppendLine($" Group:   0x{DicomItem.Tag.Group:X4} ({DicomItem.Tag.Group})")
                .AppendLine($" Element: 0x{DicomItem.Tag.Element:X4} ({DicomItem.Tag.Element})")
                .AppendLine($" Retired: {dic.IsRetired}")
                .AppendLine($" Private: {DicomItem.Tag.IsPrivate}")
                .AppendLine($" Private Creator: {(DicomItem.Tag.IsPrivate ? DicomItem.Tag.PrivateCreator : "N/A")}")
                .AppendLine($" VR:      {string.Join(",", dic.ValueRepresentations.Select(vr => vr.ToString()))}")
                .AppendLine($" VM:      {dic.ValueMultiplicity}")
                .AppendLine($" Display Value:")
                .AppendLine($"{DicomItem.ValueString}")
                .AppendLine($" Value Buffer Bytes Length: {DicomItem.Buffer.Length}")
                .AppendLine($" Value Buffer Bytes:")
                .AppendLine($"{BitConverter.ToString(DicomItem.Buffer)}");

            return sb.ToString();
        }
    }
}
