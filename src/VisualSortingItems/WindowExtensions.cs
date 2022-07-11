using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace VisualSortingItems
{
    public static class WindowExtensions
    {
        public static void SetSize(this Window window, int width, int height)
        {
            // To set the size you need to wrap the XAML window in a AppWindow
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowsId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowsId);

            // Notice that AppWindow uses raw pixels because it's a low level API.
            // WinUI uses effective pixels, so the size of the elements scale within the scale set in the Windows Settings (100%, 120%, 200%, etc)
            // It's expected that your APIs use the same unit as WinUI (effective pixels), 
            // so you should convert the size from effective pixels to raw pixels to use the Microsoft.UI.Windowing.AppWindow APIs.
            var rawPixels = ConvertEffectivePixelsIntoRawPixels(hwnd, new SizeInt32(width, height));

            appWindow.Resize(rawPixels);
        }

        private static SizeInt32 ConvertEffectivePixelsIntoRawPixels(IntPtr hwnd, SizeInt32 effectivePixels)
        {
            SizeInt32 rawPixels = new();

            // We can get the scale factor from the Win32 API's GetDpiForWindow divided by 96. 
            double dpi = GetDpiForWindow(hwnd);
            float scaleFactor = (float)dpi / 96;
            rawPixels.Width = (int)(effectivePixels.Width * scaleFactor);
            rawPixels.Height = (int)(effectivePixels.Height * scaleFactor);

            return rawPixels;
        }

        [DllImport("User32.dll")]
        public static extern int GetDpiForWindow(IntPtr hwnd);
    }
}
