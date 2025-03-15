using Overmind.Interfaces;
using System;
using System.Drawing;
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

        public Bitmap CaptureGameWindow()
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

            Bitmap screenshot = new(width, height);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));
            }
            return screenshot;
        }
    }
}
