using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrepCore.Services
{
    /// <summary>
    /// DICOM Dictionary related function. Mostly are about append custome dictionary items.
    /// </summary>
    public class DictionaryService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public const string DICOM_DIC_FILE = "DICOM Dictionary.xml";
        public const string PRIVA_DIC_FILE = "Private Dictionary.xml";

        public DictionaryService()
        {
            DicomDictionary.EnsureDefaultDictionariesLoaded(true);
        }

        public void ReadAndAppendCustomDictionaries(string dicomDictionaryFilePath = DICOM_DIC_FILE, string privateDictionaryFilePath = PRIVA_DIC_FILE)
        {
            // append private tags
            if (File.Exists(privateDictionaryFilePath))
            {
                DicomDictionary.Default.Load(privateDictionaryFilePath, DicomDictionaryFormat.XML);
                logger.Info($"Loaded user defined private tags from: {privateDictionaryFilePath}");
            }

            // append common tags
            if (File.Exists(dicomDictionaryFilePath))
            {
                ReadDictionary(out DicomDictionary dicomDictionary, dicomDictionaryFilePath);
                foreach (var entry in dicomDictionary)
                {
                    DicomDictionary.Default.Add(entry);
                }
                logger.Info($"Loaded user defined DICOM tags from: {dicomDictionaryFilePath}");
            }
        }

        private bool ReadDictionary(out DicomDictionary dictionary, string filePath)
        {
            dictionary = new DicomDictionary();

            if (File.Exists(filePath))
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    DicomDictionaryReader reader = new DicomDictionaryReader(dictionary, DicomDictionaryFormat.XML, stream);
                    reader.Process();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
