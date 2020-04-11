using DicomGrep.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DicomGrep.Services
{
    public class NotifyIconService: INotifyIconService
    {
        private NotifyIcon notifyIcon;

        public void RegisterNotifyIcon(NotifyIcon ni)
        {
            this.notifyIcon = ni;
        }

        public void SetTooltip(string text)
        {
            if (this.notifyIcon != null)
            {
                this.notifyIcon.Text = text;
            }
        }
    }
}
