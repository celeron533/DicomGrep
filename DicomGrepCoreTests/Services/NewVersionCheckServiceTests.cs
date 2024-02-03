using Microsoft.VisualStudio.TestTools.UnitTesting;
using DicomGrepCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrepCore.Services.Tests
{
    [TestClass()]
    public class NewVersionCheckServiceTests
    {
        [TestMethod()]
        public async Task GetLatestReleaseAsync()
        {
            await NewVersionCheckService.GetLatestReleaseAsync();
        }
    }
}