using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace DicomGrep.Services
{
    /// <summary>
    /// Display a folder selector UI.
    /// </summary>
    public class FolderPickupService
    {
        public bool SelectFolder(ref string folderPath)
        {
            var openFileDialog = new OpenFolderDialog
            {
                DefaultDirectory = folderPath,
            };

            if (true == openFileDialog.ShowDialog())
            {
                folderPath = openFileDialog.FolderName;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
