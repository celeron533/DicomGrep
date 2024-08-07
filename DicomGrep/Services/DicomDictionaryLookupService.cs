﻿using DicomGrep.Services.Interfaces;
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
    /// <summary>
    /// DICOM Dictionary lookup Service. In WPF to call View and VievModel.
    /// </summary>
    public class DicomDictionaryLookupService : IDicomDictionaryLookupService
    {
        public bool SelectDicomDictionaryEntry(ref string dicomTagString)
        {
            DicomDictionaryLookupView window = new DicomDictionaryLookupView();
            DicomDictionaryLookupViewModel vm = window.DataContext as DicomDictionaryLookupViewModel;
            if (!string.IsNullOrWhiteSpace(dicomTagString))
            {
                DicomTag tag = null;
                try
                {
                    tag = DicomTag.Parse(dicomTagString);
                }
                catch { }
                vm.SelectedEntry = DicomDictionary.Default.FirstOrDefault(x => x.Tag == tag);
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
