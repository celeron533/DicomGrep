using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Models
{
    public class SearchCriteria : PropertyChangedBase
    {
        private string _searchPath;
        public string SearchPath
        {
            get { return _searchPath; }
            set
            {
                if (!Equals(_searchPath, value))
                {
                    _searchPath = value;
                    NotifyOfPropertyChange(() => this.SearchPath);
                }
            }
        }

        private string _fileExtension = @"*.dcm";
        public string FileExtension
        {
            get { return _fileExtension; }
            set
            {
                if (!Equals(_fileExtension, value))
                {
                    _fileExtension = value;
                    NotifyOfPropertyChange(() => this.FileExtension);
                }
            }
        }

        private string _keyword;

        public string Keyword
        {
            get { return _keyword; }
            set
            {
                if (!Equals(this._keyword, value))
                {
                    _keyword = value;
                    NotifyOfPropertyChange(() => Keyword);
                }
            }
        }




        private bool _searchDicomTag = true;

        public bool SearchDicomTag
        {
            get { return _searchDicomTag; }
            set
            {
                if (!Equals(_searchDicomTag, value))
                {
                    _searchDicomTag = value;
                    NotifyOfPropertyChange(() => this.SearchDicomTag);
                }
            }
        }


        private bool _searchDicomValue = true;

        public bool SearchDicomValue
        {
            get { return _searchDicomValue; }
            set
            {
                if (!Equals(_searchDicomValue, value))
                {
                    _searchDicomValue = value;
                    NotifyOfPropertyChange(() => this.SearchDicomValue);
                }
            }
        }


        private bool _caseSensitive = false;

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                if (!Equals(_caseSensitive, value))
                {
                    _caseSensitive = value;
                    NotifyOfPropertyChange(() => this.CaseSensitive);
                }
            }
        }


        private bool _wholeWord = false;

        public bool WholeWord
        {
            get { return _wholeWord; }
            set
            {
                if (!Equals(_wholeWord, value))
                {
                    _wholeWord = value;
                    NotifyOfPropertyChange(() => this.WholeWord);
                }
            }
        }



        private bool _includeSubfolders = true;

        public bool IncludeSubfolders
        {
            get { return _includeSubfolders; }
            set
            {
                if (!Equals(_includeSubfolders, value))
                {
                    _includeSubfolders = value;
                    NotifyOfPropertyChange(() => this.IncludeSubfolders);
                }
            }
        }
    }
}
