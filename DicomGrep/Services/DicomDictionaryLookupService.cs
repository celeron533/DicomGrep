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
    public class DicomDictionaryLookupService
    {
        public bool SelectDicomDictionaryEntry(ref string dicomTagString)
        {
            DicomDictionaryLookupView window = new DicomDictionaryLookupView();
            DicomDictionaryLookupViewModel vm = window.DataContext as DicomDictionaryLookupViewModel;
            if (!string.IsNullOrWhiteSpace(dicomTagString))
            {
                //build pass
                //vm.SelectedEntry = DicomTag.Parse(dicomTagString);
            }
            if (window.ShowDialog() == true)
            {
                dicomTagString = vm.SelectedEntry?.Tag.ToString("X", null);
                return true;
            }
            return false;
        }
    }
}
