using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.ViewModels
{
    public class SopClassLookupViewModel : ViewModelBase
    {
        public ObservableCollection<DicomUID> SOPClassUIDs { get; private set; }

        private DicomUID _selectedUID;
        public DicomUID SelectedUID
        {
            get { return _selectedUID; }
            set { SetProperty(ref _selectedUID, value); } 
        }

        public SopClassLookupViewModel() : base()
        {
            if (SOPClassUIDs == null || SOPClassUIDs.Count == 0)
            {
                SOPClassUIDs = new ObservableCollection<DicomUID>(GetAllDicomUIDDefs().Where(u => u.Type == DicomUidType.SOPClass || u.Type == DicomUidType.MetaSOPClass));
            }
        }

        private IEnumerable<DicomUID> GetAllDicomUIDDefs()
        {
            return typeof(DicomUID).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Select(f => f.GetValue(null)).Where(v => v is DicomUID).Cast<DicomUID>();
        }
    }
}
