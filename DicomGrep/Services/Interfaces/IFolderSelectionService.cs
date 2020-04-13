using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services.Interfaces
{
    public interface IFolderSelectionService
    {
        bool SelectFolder(ref string folderPath);
    }
}
