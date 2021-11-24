using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.ViewModels
{
    public class DicomTagLookupViewModel : ViewModelBase
    {
        public ObservableCollection<DicomTag> DicomTags { get; private set; }

        private DicomTag _selectedTag;
        public DicomTag SelectedTag
        {
            get { return _selectedTag; }
            set { SetProperty(ref _selectedTag, value); }
        }

        public DicomTagLookupViewModel() : base()
        {
            if (DicomTags == null || DicomTags.Count == 0)
            {
                DicomTags = new ObservableCollection<DicomTag>(GetAllDicomTagDefs());
            }
        }

        private IEnumerable<DicomTag> GetAllDicomTagDefs()
        {
            return typeof(DicomTag).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Select(f => f.GetValue(null)).Where(v => v is DicomTag).Cast<DicomTag>();
        }
    }
}
