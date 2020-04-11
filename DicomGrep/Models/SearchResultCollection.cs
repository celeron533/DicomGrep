using Caliburn.Micro;
using Dicom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Models
{
    /// <summary>
    /// Filename + Matches in the file
    /// </summary>
    public class SearchResultCollection : ObservableCollection<FileResult>
    {
        public void AddResult(string filename, DicomTag dicomTag, string valueString)
        {
            ResultItem resultItem = new ResultItem() { Tag = dicomTag, ValueString = valueString };
            var fileResult = this.FirstOrDefault(s => s.Filename == filename);
            if (fileResult != null)
            {
                fileResult.resultItemCollection.Add(resultItem);
            }
            else
            {
                var resultItemColl = new ObservableCollection<ResultItem>();
                resultItemColl.Add(resultItem);

                this.Add(new FileResult { Filename = filename, resultItemCollection = resultItemColl });
            }

        }
    }

    public class FileResult : PropertyChangedBase
    {
        public string Filename { get; set; }
        public ObservableCollection<ResultItem> resultItemCollection { get; set; }
    }

    public class ResultItem : PropertyChangedBase
    {
        private DicomTag _tag;

        public DicomTag Tag
        {
            get { return _tag; }
            set
            {
                if (!Equals(_tag, value))
                {
                    _tag = value;
                    NotifyOfPropertyChange(() => this.Tag);
                }
            }
        }

        private string _valueString;

        public string ValueString
        {
            get { return _valueString; }
            set
            {
                if (!Equals(_valueString, value))
                {
                    _valueString = value;
                    NotifyOfPropertyChange(() => this.ValueString);
                }
            }
        }

    }
}
