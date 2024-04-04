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

        public DicomDictionaryLookupViewModel() : base()
        {
            if (DicomDictionaryEntries == null || DicomDictionaryEntries.Count == 0)
            {
                DicomDictionaryEntries = new ObservableCollection<DicomDictionaryEntry>(
                    GetAllDicomTagDefs().OrderBy(entry => entry.Tag.Group).ThenBy(entry => entry.Tag.Element)
                    .Concat(GetAllPrivateTagDefs()).OrderBy(entry => entry.Tag.PrivateCreator).ThenBy(entry => entry.Tag.Group).ThenBy(entry => entry.Tag.Element)
                    );
            }
        }

        /// <summary>
        /// Get all public DICOM tag definitions. Private tags are not included.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DicomDictionaryEntry> GetAllDicomTagDefs()
        {
            return DicomDictionary.Default;
        }

        /// <summary>
        /// Get all private DICOM tag definitions.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DicomDictionaryEntry> GetAllPrivateTagDefs()
        {
            ConcurrentDictionary<string, DicomDictionary> _private = typeof(DicomDictionary)
                .GetField("_private", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(DicomDictionary.Default)
                as ConcurrentDictionary<string, DicomDictionary>;

            // each vendor (private tag creator) can own multiple entries (tags)
            IEnumerable<DicomDictionary> vendorsDictionary = _private.Select(item => item.Value);
            foreach (var dictionary in vendorsDictionary)
            {
                foreach (var entry in dictionary)
                {
                    yield return entry;
                }
            }
        }
    }
}
