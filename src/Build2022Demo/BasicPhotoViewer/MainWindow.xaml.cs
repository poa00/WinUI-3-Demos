using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Numerics;
using Windows.Storage.Pickers;

//Mica
using WinRT; // required to support Window.As<ICompositionSupportsSystemBackdrop>()

namespace PhotoViewer
{
    public sealed partial class MainWindow : Window
    {
        private string _folder;
        public ImagesRepository ImagesRepository { get; } = new ImagesRepository();

        public MainWindow()
        {
            this.InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(CustomTitleBar);

            TrySetMicaBackdrop();
            Title = "Simple Photo Viewer";

            string _folder = "C:\\Users\\migue\\source\\repos\\WinUI-3-Demos\\src\\Build2022Demo\\Photos";

            LoadImages(_folder);
        }
        private async void SelectFolderClick(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new();
            folderPicker.FileTypeFilter.Add("*");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder is not null)
            {
                _folder = folder.Path;
                LoadImages(_folder);
            }
        }
        void LoadImages(string _folder)
        {
            ImagesRepository.GetImages(_folder);
            ImageCollectionInfoBar.IsOpen = true;
            ImageCollectionInfoBar.Title = "Load Images";
            ImageCollectionInfoBar.Message = ImagesRepository.Images.Count.ToString() + " images loaded";
        }

        private void ImageClick(object sender, RoutedEventArgs e)
        {
            //MultiWindows, Also you can write C# code, you don't need markup

            var window = new Window();
            ImageInfo imageInfo = (sender as Button).DataContext as ImageInfo;
            if (imageInfo is not null)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(imageInfo.FullName, UriKind.Absolute));
                window.Content = img;
                window.Title = imageInfo.Name;
            }

            //Another componente of the WinAppSDK: AppWindow Interop 
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var winID = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(winID);
            appWindow.Resize(new Windows.Graphics.SizeInt32(800, 600));

            window.Activate();
        }
        private async void AboutClick(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "About Simple Photo Editor",
                Content = "Done exclusively for //Build 2022",
                CloseButtonText = "Ok"
            };
            //Explain XamlRoot concept. You can get the XamlRoot of any UIElement
            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }

        #region Animations
        //Advanced - Copy from XAML Controls Gallery
        private SpringVector3NaturalMotionAnimation _springAnimation;

        private void OnElementPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.05f);
            (sender as UIElement).StartAnimation(_springAnimation);
        }
        private void OnElementPointerExited(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.0f);
            (sender as UIElement).StartAnimation(_springAnimation);
        }
        private void CreateOrUpdateSpringAnimation(float finalValue)
        {
            if (_springAnimation == null)
            {
                Compositor compositor = this.Compositor;
                if (compositor is not null)
                {
                    _springAnimation = compositor.CreateSpringVector3Animation();
                    _springAnimation.Target = "Scale";
                }
            }
            _springAnimation.FinalValue = new Vector3(finalValue);
        }
        #endregion Animations

        #region Mica
        WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See separate sample below for implementation
        Microsoft.UI.Composition.SystemBackdrops.MicaController m_micaController;
        Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource;
        bool TrySetMicaBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                this.Activated += Window_Activated;
                this.Closed += Window_Closed;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                switch (((FrameworkElement)this.Content).ActualTheme)
                {
                    case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                    case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                    case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
                }

                m_micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }
        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use this closed window.
            if (m_micaController != null)
            {
                m_micaController.Dispose();
                m_micaController = null;
            }
            this.Activated -= Window_Activated;
            m_configurationSource = null;
        }
        #endregion
    }
}
