using DicomGrep.ViewModels;
using DicomGrep.Views;
using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services
{
    public class SopClassLookupService
    {
        public bool SelectSopClass(ref string sopClassUidString)
        {
            SopClassLookupView window = new SopClassLookupView();
            SopClassLookupViewModel vm = window.DataContext as SopClassLookupViewModel;
            if (!string.IsNullOrWhiteSpace(sopClassUidString))
            {
                vm.SelectedUID = new DicomUID(sopClassUidString, "empty", DicomUidType.SOPClass);
            }
            if (window.ShowDialog() == true)
            {
                sopClassUidString = vm.SelectedUID?.UID;
                return true;
            }
            return false;
        }
    }
}
