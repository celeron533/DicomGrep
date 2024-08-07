﻿using DicomGrep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using DicomGrepCore.Entities;
using DicomGrepCore.Enums;
using DicomGrep.Services.Interfaces;

namespace DicomGrep.Services
{
    /// <summary>
    /// Configuration Service
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
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
            logger.Info("Saving the configuration to file");

            JsonSerializerOptions option = new JsonSerializerOptions { WriteIndented = true };
            try
            {
                File.WriteAllText(CONFIG_FILE, JsonSerializer.Serialize(CurrentConfiguration, option));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to save the configuration to file.");
                return false;
            }
            return true;
        }

        public bool Load()
        {
            logger.Info("Loading the configuration from file");
            try
            {
                if (File.Exists(CONFIG_FILE))
                {
                    CurrentConfiguration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(CONFIG_FILE));
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to load the configuration from file.");
            }
            CurrentConfiguration = DefaultConfiguration();
            return false;
        }





        /// <summary>
        /// The default configuration
        /// </summary>
        /// <returns></returns>
        private static Configuration DefaultConfiguration()
        {
            logger.Info("Load default configuration");

            SearchCriteria criteria = new SearchCriteria
            {
                SearchPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                FileTypes = "*.dcm",
                SearchSopClassUid = "",
                SearchTag = "",
                SearchText = "TREATMENT",
                IncludeSubfolders = true,
                MatchPattern = MatchPatternEnum.Normal,
                SearchInResults = false,
                SearchThreads = 0
            };

            return new Configuration
            {
                SopClassUidHistory = new List<string> { criteria.SearchSopClassUid },
                DicomTagHistory = new List<string> { criteria.SearchTag },
                FileTypesHistory = new List<string> { criteria.FileTypes },
                SearchPathHistory = new List<string> { criteria.SearchPath },
                SearchTextHistory = new List<string> { criteria.SearchText },
                HistoryCapacity = 10,
                SearchCriteria = criteria
            };
        }

    }
}
