using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace Alphy
{
    public static class Updater
    {
        private static string githubVersionUrl = "";
        private static string githubZipUrl = "";

        public static string LocalVersion = "1.7.1";

        public static void EnsureBundledBackendInstalled()
        {
            InstallEmbeddedBackendFile(
                "alphy_custom_texture_injector.py",
                "Alphy.Backend.alphy_custom_texture_injector.py");
        }

        private static void InstallEmbeddedBackendFile(string fileName, string resourceName)
        {
            try
            {
                byte[] embeddedBytes = ReadEmbeddedResource(resourceName, fileName);
                if (embeddedBytes == null || embeddedBytes.Length == 0)
                {
                    CopyLooseBackendFileIfNewer(fileName);
                    return;
                }

                string destinationDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "AlphySwapper",
                    "Backend");
                Directory.CreateDirectory(destinationDir);

                string destinationPath = Path.Combine(destinationDir, fileName);
                if (!File.Exists(destinationPath) || !FileBytesEqual(destinationPath, embeddedBytes))
                {
                    File.WriteAllBytes(destinationPath, embeddedBytes);
                    File.SetLastWriteTimeUtc(destinationPath,
                        File.GetLastWriteTimeUtc(Assembly.GetExecutingAssembly().Location));
                }
            }
            catch
            {
                // The local bundled backend remains available as a fallback.
            }
        }

        private static byte[] ReadEmbeddedResource(string resourceName, string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resolvedName = resourceName;
            if (assembly.GetManifestResourceInfo(resolvedName) == null)
            {
                resolvedName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(name => name.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));
            }

            if (string.IsNullOrWhiteSpace(resolvedName))
                return null;

            using (Stream stream = assembly.GetManifestResourceStream(resolvedName))
            {
                if (stream == null)
                    return null;

                using (MemoryStream memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
        }

        private static bool FileBytesEqual(string path, byte[] expectedBytes)
        {
            byte[] currentBytes = File.ReadAllBytes(path);
            if (currentBytes.Length != expectedBytes.Length)
                return false;

            for (int i = 0; i < currentBytes.Length; i++)
            {
                if (currentBytes[i] != expectedBytes[i])
                    return false;
            }

            return true;
        }

        private static void CopyLooseBackendFileIfNewer(string fileName)
        {
            string sourcePath = Path.Combine(Application.StartupPath, "Backend", fileName);
            if (!File.Exists(sourcePath))
                return;

            string destinationDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AlphySwapper",
                "Backend");
            Directory.CreateDirectory(destinationDir);

            string destinationPath = Path.Combine(destinationDir, fileName);
            if (!File.Exists(destinationPath) || IsSourceNewer(sourcePath, destinationPath))
            {
                File.Copy(sourcePath, destinationPath, true);
                File.SetLastWriteTimeUtc(destinationPath, File.GetLastWriteTimeUtc(sourcePath));
            }
        }

        private static bool IsSourceNewer(string sourcePath, string destinationPath)
        {
            DateTime sourceTime = File.GetLastWriteTimeUtc(sourcePath);
            DateTime destinationTime = File.GetLastWriteTimeUtc(destinationPath);
            return sourceTime > destinationTime.AddMilliseconds(1);
        }

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
