﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace DicomGrep.Services
{
    public class FileOperationService
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
