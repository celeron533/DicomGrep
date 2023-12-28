using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellowOakDicom;

namespace DicomGrep.Services.Tests
{
    [TestClass()]
    public class DictionaryServiceTests
    {
        [TestMethod()]
        public void ReadAndAppendCustomDictionariesTest()
        {
            DictionaryService dictionaryService = new DictionaryService();
            int before = DicomDictionary.Default.Count();
            dictionaryService.ReadAndAppendCustomDictionaries();
            int after = DicomDictionary.Default.Count();
            Console.WriteLine($"dicom tag count - before: {before} ; after: {after}");

            Assert.IsTrue(before < after);
        }
    }
}