using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Alphy
{
    public static class ImageCache
    {
        private static readonly string CacheRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Alphy", "cache", "icons");
        private static readonly string GithubBaseUrl = "";

        public static async Task<Image> GetModImageAsync(string category, string modName, string replacesName)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(modName))
                    return GetErrorImage();

                string mappedCategory = GetCategoryFolder(category);
                string cleanModName = modName.Trim();

                string safeFileName = cleanModName.Replace("/", "-").Replace(":", "-").Replace("?", "").Replace("\"", "") + ".png";

                string localCategoryFolder = Path.Combine(CacheRoot, mappedCategory);
                string localFilePath = Path.Combine(localCategoryFolder, safeFileName);

                if (!Directory.Exists(localCategoryFolder))
                {
                    Directory.CreateDirectory(localCategoryFolder);
                }

                if (File.Exists(localFilePath))
                {
                    Image cachedImg = LoadImageSafely(localFilePath);
                    if (cachedImg != null) return cachedImg;

                    try { File.Delete(localFilePath); } catch { }
                }

                try
                {
                    string encodedFileName = Uri.EscapeDataString(safeFileName);

                    string webUrl = $"{GithubBaseUrl}/{mappedCategory}/{encodedFileName}?t={DateTime.Now.Ticks}";

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("User-Agent", "AlphyModsClient");
                        client.DownloadFile(webUrl, localFilePath);
                    }

                    if (File.Exists(localFilePath))
                    {
                        Image newImg = LoadImageSafely(localFilePath);
                        if (newImg != null) return newImg;

                        try { File.Delete(localFilePath); } catch { }
                    }
                }
                catch { }

                return GetErrorImage();
            });
        }

        public static async Task<Image> GetLocalImageAsync(string path)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                    return null;

                return LoadImageSafely(path);
            });
        }

        private static Image GetErrorImage()
        {
            string errorFolder = Path.Combine(CacheRoot, "error");
            string errorPath = Path.Combine(errorFolder, "error.png");

            if (!Directory.Exists(errorFolder))
            {
                Directory.CreateDirectory(errorFolder);
            }

            if (!File.Exists(errorPath))
            {
                try
                {
                    string errorUrl = $"{GithubBaseUrl}/error/error.png?t={DateTime.Now.Ticks}";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("User-Agent", "AlphyModsClient");
                        client.DownloadFile(errorUrl, errorPath);
                    }
                }
                catch { }
            }

            if (File.Exists(errorPath))
            {
                Image errImg = LoadImageSafely(errorPath);
                if (errImg != null) return errImg;

                try { File.Delete(errorPath); } catch { }
            }

            return GetFallbackBox();
        }

        private static Image LoadImageSafely(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);

                if (bytes.Length > 0 && bytes[0] == '<')
                    return null;

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        private static string GetCategoryFolder(string uiCategory)
        {
            switch (uiCategory.Trim())
            {
                case "Antenna": return "antennas";
                case "Banner": return "banners";
                case "Body": return "bodies";
                case "Boost": return "boosts";
                case "Boost Audio": return "error";
                case "Decal": return "decals";
                case "Goal Explosion": return "explosions";
                case "Hat": return "toppers";
                case "Paint": return "paints";
                case "Trail": return "trails";
                case "Wheels": return "wheels";
                default: return uiCategory.ToLower().Trim();
            }
        }

        private static Image GetFallbackBox()
        {
            Bitmap bmp = new Bitmap(120, 120);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(40, 40, 40));
                using (Font f = new Font("Consolas", 8, FontStyle.Bold))
                using (SolidBrush b = new SolidBrush(Color.DarkGray))
                {
                    g.DrawString("NO IMG", f, b, new PointF(10, 50));
                }
            }
            return bmp;
        }
    }
}
