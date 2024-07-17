using DicomGrepCore.Services.Interfaces;
using FellowOakDicom;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.ViewModels
{
    public class DicomDictionaryLookupViewModel : ViewModelBase
    {
        public ObservableCollection<DicomDictionaryEntry> DicomDictionaryEntries { get; private set; }

        private DicomDictionaryEntry _selectedEntry;
        public DicomDictionaryEntry SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if (_selectedEntry == null && value != null && string.IsNullOrEmpty(DefaultFilterString)) // only triggerred when set the initial value
                {
                    DefaultFilterString = value.Name;
                }
                SetProperty(ref _selectedEntry, value);
            }
        }

        private string _defaultFilterString;
        public string DefaultFilterString
        {
            get => _defaultFilterString;
            set => SetProperty(ref _defaultFilterString, value);
        }

        public DicomDictionaryLookupViewModel(IDictionaryService dictionaryService) : base()
        {
            if (DicomDictionaryEntries == null || DicomDictionaryEntries.Count == 0)
            {
                DicomDictionaryEntries = new ObservableCollection<DicomDictionaryEntry>(
                    dictionaryService.GetAllDicomTagDefs().OrderBy(entry => entry.Tag.Group).ThenBy(entry => entry.Tag.Element)
                    .Concat(dictionaryService.GetAllPrivateTagDefs()).OrderBy(entry => entry.Tag.PrivateCreator).ThenBy(entry => entry.Tag.Group).ThenBy(entry => entry.Tag.Element)
                    );
            }
        }

    }
}
