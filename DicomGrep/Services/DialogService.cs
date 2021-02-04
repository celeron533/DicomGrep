﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DicomGrep.Services
{
    public class DialogService
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