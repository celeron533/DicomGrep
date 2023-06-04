using DicomGrep.Models;
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
    public class TagValueDetailService
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
