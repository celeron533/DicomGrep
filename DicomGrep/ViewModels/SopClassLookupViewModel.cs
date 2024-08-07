﻿using DicomGrepCore.Services.Interfaces;
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
            get => _selectedUID;
            set
            {
                if (_selectedUID == null && value != null && string.IsNullOrEmpty(DefaultFilterString)) // only triggerred when set the initial value
                {
                    DefaultFilterString = value.UID.ToString();
                }
                SetProperty(ref _selectedUID, value);
            }
        }

        private string _defaultFilterString;
        public string DefaultFilterString
        {
            get => _defaultFilterString;
            set => SetProperty(ref _defaultFilterString, value);
        }

        public SopClassLookupViewModel() : base()
        {
            var dictionaryService = GetService<IDictionaryService>();
            if (SOPClassUIDs == null || SOPClassUIDs.Count == 0)
            {
                SOPClassUIDs = new ObservableCollection<DicomUID>(dictionaryService.GetAllDicomUIDDefs().Where(u => u.Type == DicomUidType.SOPClass || u.Type == DicomUidType.MetaSOPClass));
            }
        }
    }
}
