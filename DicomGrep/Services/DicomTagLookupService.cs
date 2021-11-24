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
    public class DicomTagLookupService
    {
        public bool SelectDicomTag(ref string dicomTagString)
        {
            DicomTagLookupView window = new DicomTagLookupView();
            DicomTagLookupViewModel vm = window.DataContext as DicomTagLookupViewModel;
            if (!string.IsNullOrWhiteSpace(dicomTagString))
            {
                vm.SelectedTag = DicomTag.Parse(dicomTagString);
            }
            if (window.ShowDialog() == true)
            {
                dicomTagString = vm.SelectedTag.ToString();
                return true;
            }
            return false;
        }
    }
}
