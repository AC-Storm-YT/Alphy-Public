using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Alphy
{
    public static class Updater
    {
        private static string githubVersionUrl = "https://github.com/AC-Storm-YT/alphy.github.io/raw/refs/heads/main/version.txt";
        private static string githubZipUrl = "https://github.com/AC-Storm-YT/alphy.github.io/raw/refs/heads/main/alphy.zip";

        public static string LocalVersion = "1.5.0";

        public static string GetServerVersion()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string tempVersion = Path.Combine(Path.GetTempPath(), "alphy_version.txt");
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "AlphyModsUpdater");
                    client.DownloadFile(githubVersionUrl, tempVersion);
                }
                return File.ReadAllText(tempVersion).Trim();
            }
            catch
            {
                return "Unknown";
            }
        }

        public static void CheckForUpdates()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string serverVersion = GetServerVersion();

                if (serverVersion != "Unknown" && serverVersion != LocalVersion)
                {
                    string tempZip = Path.Combine(Path.GetTempPath(), "update.zip");
                    DownloadFileFromWeb(githubZipUrl, tempZip);

                    if (IsAutoUpdateEnabled())
                    {
                        ExecuteUpdateBatch();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(
                            $"A new update is available.\n\n" +
                            $"Current Version: {LocalVersion}\n" +
                            $"New Version: {serverVersion}\n\n" +
                            "Would you like to update now?",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.Yes)
                        {
                            ExecuteUpdateBatch();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Nothing.
            }
        }

        private static void ExecuteUpdateBatch()
        {
            string updaterBat = Path.Combine(Application.StartupPath, "update.bat");
            string installPath = Application.StartupPath;

            if (!File.Exists(updaterBat))
            {
                MessageBox.Show("Error: update.bat not found in the application folder.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/C \"\"{updaterBat}\" \"{installPath}\"\"",
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            });

            Environment.Exit(0);
        }

        private static bool IsAutoUpdateEnabled()
        {
            string settingsFile = Path.Combine(Application.StartupPath, "settings.txt");

            if (!File.Exists(settingsFile))
                return false;

            try
            {
                var settings = File.ReadAllLines(settingsFile)
                                   .Select(line => line.Split('='))
                                   .Where(parts => parts.Length == 2)
                                   .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

                return settings.ContainsKey("auto-update") && settings["auto-update"] == "1";
            }
            catch { return false; }
        }

        private static void DownloadFileFromWeb(string url, string localPath)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "AlphyModsUpdater");
                client.DownloadFile(url, localPath);
            }
        }
    }
}