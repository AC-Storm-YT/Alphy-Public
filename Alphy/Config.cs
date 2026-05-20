using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alphy
{
    public static class Config
    {
        private static readonly string BaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Alphy");
        public static readonly string BackupFolder = Path.Combine(BaseFolder, "Backups");
        private static readonly string ConfigFolder = Path.Combine(BaseFolder, "Config");
        private static readonly string SettingsFile = Path.Combine(ConfigFolder, "settings.txt");

        public static void InitializeDirectories()
        {
            Directory.CreateDirectory(BaseFolder);
            Directory.CreateDirectory(BackupFolder);
            Directory.CreateDirectory(ConfigFolder);
        }

        /// <summary>
        /// Saves mod names paired with their current file hashes.
        /// </summary>
        public static void SaveSettings(Dictionary<string, string> modHashes)
        {
            try
            {
                var lines = modHashes.Select(kvp => $"{kvp.Key}={kvp.Value}");
                File.WriteAllLines(SettingsFile, lines);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Loads the saved mods and their hashes into a Dictionary.
        /// </summary>
        public static Dictionary<string, string> LoadSettings()
        {
            var results = new Dictionary<string, string>();
            if (!File.Exists(SettingsFile)) return results;

            try
            {
                var lines = File.ReadAllLines(SettingsFile);
                foreach (var line in lines)
                {
                    int index = line.IndexOf('=');
                    if (index > 0)
                    {
                        string key = line.Substring(0, index);
                        string val = line.Substring(index + 1);
                        results[key] = val;
                    }
                }
            }
            catch { }
            return results;
        }
    }
}