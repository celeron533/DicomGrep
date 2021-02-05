using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DicomGrep.ViewModels
{
    public class AboutViewModel: ViewModelBase
    {
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
