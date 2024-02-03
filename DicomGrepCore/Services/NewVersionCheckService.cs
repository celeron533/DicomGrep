using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DicomGrepCore.Services
{
    public class NewVersionCheckService
    {
        public static async Task<Version> GetLatestReleaseAsync()
        {
            Version latestVersion = new Version();
            var client = new GitHubClient(new ProductHeaderValue("DicomGrep"));
            var latest = await client.Repository.Release.GetLatest("celeron533", "DicomGrep");
            if (latest != null)
            {
                latestVersion = ParseVersion(latest.TagName);
            }

            return latestVersion;
        }

        static Version ParseVersion(string versionStr)
        {
            Version version = new Version();
            Regex regex = new Regex(@"v(\d+)\.(\d+)\.(\d+)");
            Match match = regex.Match(versionStr);

            if (match.Success)
            {
                try
                {
                    int major = int.Parse(match.Groups[1].Value);
                    int minor = int.Parse(match.Groups[2].Value);
                    int patch = int.Parse(match.Groups[3].Value);
                    version = new Version(major, minor, patch);
                }
                catch { }
            }

            return version;
        }
    }
}
