using System;
using System.IO;
using System.Security.Cryptography;

namespace Alphy
{
    public static class FileChecker
    {
        /// <summary>
        /// Calculates the MD5 hash of a file to create a unique fingerprint.
        /// </summary>
        public static string GetFileHash(string filename)
        {
            if (!File.Exists(filename)) return "null";

            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch
            {
                return "error";
            }
        }

        /// <summary>
        /// Compares the current game file against the mod and backup to see if an update changed it.
        /// </summary>
        public static bool IsNewVanillaFile(string gamePath, string backupPath, string modPath)
        {
            string gameHash = GetFileHash(gamePath);
            string modHash = GetFileHash(modPath);
            string backupHash = GetFileHash(backupPath);

            if (gameHash == modHash) return false;

            if (gameHash != backupHash) return true;

            return false;
        }
    }
}