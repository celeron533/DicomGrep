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
        public static Configuration CurrentConfiguration { get; private set; }
        public const string CONFIG_FILE = "config.json";

        public Configuration GetConfiguration() => CurrentConfiguration;

        public void SetConfiguration(Configuration config)
        {
            CurrentConfiguration = config;
            //todo: raise configuration changed event
        }



        public bool SaveConfiguration()
        {
            try
            {
                File.WriteAllText(CONFIG_FILE, JsonSerializer.Serialize(CurrentConfiguration));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool LoadConfiguration()
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
            return new Configuration
            {
                FileTypesHistory = new List<string> { "*.dcm" },
                SearchPathHistory = new List<string> { "C:\\Users\\Public" },
                SearchTextHistory = new List<string> { "(0010,0020)" },
                SearchThreads = 1,
                HistoryCapacity = 10
            };
        }


        private void PushToList(string newItem, ref IList<string> list, int capacity)
        {
            int index = list.IndexOf(newItem);
            if (index >= 0)
            {
                list.RemoveAt(index);
            }

            if (list.Count > capacity)
            {
                list.RemoveAt(list.Count - 1);
            }

            if (list.Count == 0)
            {
                list.Add(newItem);
            }
            else
            {
                list.Insert(0, newItem);
            }
        }
    }
}
