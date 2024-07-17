using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using DicomGrep.Services.Interfaces;

namespace DicomGrep.Services
{
    /// <summary>
    /// Dialog Service. For the WPF MVVM pattern.
    /// </summary>
    public class DialogService : IDialogService
    {
        public MessageBoxResult ShowMessageBox(string messageBoxText,
                                        string caption = "Message",
                                        MessageBoxButton button = MessageBoxButton.OK,
                                        MessageBoxImage icon = MessageBoxImage.None,
                                        MessageBoxResult defaultResult = MessageBoxResult.OK,
                                        MessageBoxOptions options = MessageBoxOptions.None)
        {
            return MessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
        }

        public void ShowView(Window view)
        {
            view.Show();
        }

        public bool? ShowViewDialog(Window view)
        {
            return view.ShowDialog();
        }
    }
}
