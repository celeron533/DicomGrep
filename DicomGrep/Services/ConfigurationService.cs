using DicomGrep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DicomGrep.Services
{
    public class ConfigurationService
    {
        private static Configuration CurrentConfiguration { get; set; }
        public const string CONFIG_FILE = "config.json";

        public Configuration GetConfiguration() => CurrentConfiguration;

        public void SetConfiguration(Configuration config)
        {
            CurrentConfiguration = config;
            //todo: raise configuration changed event
        }



        public bool Save()
        {
            JsonSerializerOptions option = new JsonSerializerOptions { WriteIndented = true };
            try
            {
                File.WriteAllText(CONFIG_FILE, JsonSerializer.Serialize(CurrentConfiguration, option));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Load()
        {
            try
            {
                if (File.Exists(CONFIG_FILE))
                {
                    CurrentConfiguration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(CONFIG_FILE));
                    return true;
                }
            }
            catch { }
            CurrentConfiguration = DefaultConfiguration();
            return false;
        }






        private static Configuration DefaultConfiguration()
        {
            SearchCriteria criteria = new SearchCriteria
            {
                SearchPath = "C:\\Users\\Public",
                FileTypes = "*.dcm",
                SearchText = "(0010,0020)",
                SearchDicomTag = true,
                SearchDicomValue = true,
                IncludeSubfolders = true,
                SearchThreads = 1
            };

            return new Configuration
            {
                FileTypesHistory = new List<string> { criteria.FileTypes },
                SearchPathHistory = new List<string> { criteria.SearchPath },
                SearchTextHistory = new List<string> { criteria.SearchText },
                HistoryCapacity = 10,
                SearchCriteria = criteria
            };
        }

    }
}
