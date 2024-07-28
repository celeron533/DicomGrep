using DicomGrep.Models;
using DicomGrep.Services.Interfaces;
using DicomGrep.ViewModels;
using DicomGrep.Views;
using DicomGrepCore.Entities;
using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services
{
    /// <summary>
    /// DICOM tag value details lookup Service. In WPF to call View and VievModel.
    /// </summary>
    public class TagValueDetailService : ITagValueDetailService
    {
        public void InspectTagValue(ResultDicomItem item)
        {
            TagValueDetailView window = new TagValueDetailView();
            TagValueDetailViewModel vm = window.DataContext as TagValueDetailViewModel;
            vm.DicomItem = item;
            window.ShowDialog();
        }
    }
}
