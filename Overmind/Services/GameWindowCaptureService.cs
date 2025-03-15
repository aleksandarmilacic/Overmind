using Overmind.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Overmind.Services
{
    public class GameWindowCaptureService : IGameWindowCapture
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private struct RECT { public int Left, Top, Right, Bottom; }

        private readonly string _gameWindowName;

        public GameWindowCaptureService(string gameWindowName)
        {
            _gameWindowName = gameWindowName;
        }

        public string CaptureGameWindowAsBase64()
        {
            IntPtr hwnd = FindWindow(null, _gameWindowName);
            if (hwnd == IntPtr.Zero)
            {
                Console.WriteLine($"❌ Window '{_gameWindowName}' not found.");
                return null;
            }

            if (!GetWindowRect(hwnd, out RECT rect))
            {
                Console.WriteLine("❌ Failed to get window dimensions.");
                return null;
            }

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            using Bitmap screenshot = new(width, height);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));
            }
            // Ensure minimum resolution of 512x512
            Bitmap finalScreenshot = EnsureMinimumResolution(screenshot);
            // Convert to Base64
            using MemoryStream ms = new();
            finalScreenshot.Save(ms, ImageFormat.Png);

            byte[] imageBytes = ms.ToArray();

            // Check size limit (20MB)
            if (imageBytes.Length > 20 * 1024 * 1024)
            {
                Console.WriteLine($"⚠️ Image too large ({imageBytes.Length / (1024 * 1024)}MB). Skipping.");
                return null;
            } 
            return Convert.ToBase64String(imageBytes);
        }

        private Bitmap EnsureMinimumResolution(Bitmap original)
        {
            int minWidth = 512, minHeight = 512;
            int maxShortSide = 768, maxLongSide = 2000;

            int newWidth = original.Width, newHeight = original.Height;

            // Ensure minimum resolution
            if (newWidth < minWidth || newHeight < minHeight)
            {
                newWidth = Math.Max(original.Width, minWidth);
                newHeight = Math.Max(original.Height, minHeight);
            }

            // Ensure maximum resolution constraints
            int shortSide = Math.Min(newWidth, newHeight);
            int longSide = Math.Max(newWidth, newHeight);

            if (shortSide > maxShortSide || longSide > maxLongSide)
            {
                float scaleFactor = Math.Min((float)maxShortSide / shortSide, (float)maxLongSide / longSide);
                newWidth = (int)(newWidth * scaleFactor);
                newHeight = (int)(newHeight * scaleFactor);
            }

            if (newWidth != original.Width || newHeight != original.Height)
            {
                Console.WriteLine($"🔄 Resizing image to {newWidth}x{newHeight} to fit OpenAI limits...");
                Bitmap resized = new(newWidth, newHeight);
                using (Graphics g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(original, 0, 0, newWidth, newHeight);
                }
                return resized;
            }

            return original;
        }

    }
}
