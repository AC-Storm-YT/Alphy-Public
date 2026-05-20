using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Alphy
{
    public static class GameVersionCheck
    {
        private const string ExeRelativePath = @"..\..\Binaries\Win64\RocketLeague.exe";

        private const string SupportedVersionUrl = "https://raw.githubusercontent.com/AC-Storm-YT/alphy.github.io/refs/heads/main/gameversion.txt";

        private const string LiveBuildUrl = "https://raw.githubusercontent.com/AC-Storm-YT/alphy.github.io/refs/heads/main/rlsteambuild.txt";
        private const string SupportedBuildUrl = "https://raw.githubusercontent.com/AC-Storm-YT/alphy.github.io/refs/heads/main/supportedbuild";

        public static string GetLocalGameVersion(string cookedPath)
        {
            try
            {
                string exePath = Path.GetFullPath(Path.Combine(cookedPath, ExeRelativePath));

                if (File.Exists(exePath))
                {
                    FileVersionInfo vInfo = FileVersionInfo.GetVersionInfo(exePath);

                    return $"{vInfo.FileMajorPart}.{vInfo.FileMinorPart}.{vInfo.FileBuildPart}.{vInfo.FilePrivatePart}";
                }
                return "Not Found";
            }
            catch
            {
                return "Error";
            }
        }

        public static string GetSupportedVersion()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    return client.DownloadString(SupportedVersionUrl).Trim();
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetLiveBuildVersion()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    return client.DownloadString(LiveBuildUrl).Trim();
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetSupportedBuildVersion()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    return client.DownloadString(SupportedBuildUrl).Trim();
                }
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}