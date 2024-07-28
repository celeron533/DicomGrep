using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using DicomGrep.Services.Interfaces;

namespace DicomGrep.Services
{
    /// <summary>
    /// File system and Windows explorer related
    /// </summary>
    public class FileOperationService : IFileOperationService
    {
        public bool OpenDirectory(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            filePath = Path.GetFullPath(filePath);
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            return true;
        }

        public bool OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            filePath = Path.GetFullPath(filePath);
            Process.Start("explorer.exe", $"\"{filePath}\"");
            return true;
        }

        public bool CopyName(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            Clipboard.SetText(Path.GetFileName(filePath));
            return true;
        }

        public bool CopyFullNamePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            Clipboard.SetText(Path.GetFullPath(filePath));
            return true;
        }
    }
}
