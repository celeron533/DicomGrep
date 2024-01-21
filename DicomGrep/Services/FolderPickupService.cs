using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Services
{
    /// <summary>
    /// Display a folder selector UI.
    /// </summary>
    public class FolderPickupService
    {
        public bool SelectFolder(ref string folderPath)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                DefaultDirectory = folderPath
            };

            // note: may slow to stop debugging when running from Visual Studio
            if (CommonFileDialogResult.Ok == openFileDialog.ShowDialog())
            {
                folderPath = openFileDialog.FileName;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
