using DicomGrep.Models;
using DicomGrep.Services.Interfaces;
using DicomGrepCore.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services
{
    /// <summary>
    /// Export the search result to file.
    /// </summary>
    public class ExportService : IExportService
    {
        public void Export(List<ResultDicomFile> resultFiles, string exportTo = "")
        {
            if (string.IsNullOrEmpty(exportTo))
            {
                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
                };
                if (dialog.ShowDialog() == true)
                {
                    exportTo = dialog.FileName;
                }
                else
                {
                    return;
                }
            }

            using (StreamWriter sw = new StreamWriter(exportTo, false))
            {
                sw.WriteLine($"{resultFiles.Count} match files on {DateTime.Now.ToString("s")}");
                foreach (ResultDicomFile dicomFile in resultFiles)
                {
                    sw.WriteLine($"{dicomFile.FullFilename} | {dicomFile.SOPClassUID} | {dicomFile.SOPClassName} | {dicomFile.PatientName} | matches:{dicomFile.ResultDicomItems.Count}");
                    foreach (ResultDicomItem result in dicomFile.ResultDicomItems)
                    {
                        sw.WriteLine($"    {result.Tag.ToString()} | {result.ValueString}");
                    }
                }

                DialogService ds = new DialogService();
                ds.ShowMessageBox($"Search result has been exported to:\n{exportTo}",
                    "Export", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}
