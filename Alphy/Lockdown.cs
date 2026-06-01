using System;
using System.IO;
using System.Net;

namespace Alphy
{
    public static class Lockdown
    {
        private static string githubLockdownUrl = "";

        public static bool IsInLockdown()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "AlphyModsClient");

                    string status = client.DownloadString(githubLockdownUrl).Trim().ToLower();

                    return status == "true";
                }
            }
            catch
            {
                return false;
            }
        }
    }
}