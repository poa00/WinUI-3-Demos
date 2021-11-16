using System;
using Microsoft.UI.Xaml;

namespace DemoBuildCs
{
    public partial class App : Application
    {

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
            m_window.Title = "Folder Inspector (.NET 6 WinUI 3)";

            // The Window object doesn't have Width and Height properties in WInUI 3.
            // You can use the Win32 API SetWindowPos to set the Width and Height.
            SetWindowSize(hwnd, 800, 600);

            m_window.Activate();
        }

        private void SetWindowSize(IntPtr hwnd, int width, int height)
        {
            // Win32 uses pixels and WinUI 3 uses effective pixels, so you should apply the DPI scale factor
            var dpi = PInvoke.User32.GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            PInvoke.User32.SetWindowPos(hwnd, PInvoke.User32.SpecialWindowHandles.HWND_TOP,
                                        0, 0, width, height,
                                        PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE);
        }

        private Window m_window;
    }
}
