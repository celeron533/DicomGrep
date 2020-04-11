using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DicomGrep.Services.Interfaces
{
    public interface INotifyIconService
    {
        void RegisterNotifyIcon(NotifyIcon ni);

        void SetTooltip(string text);
    }
}
