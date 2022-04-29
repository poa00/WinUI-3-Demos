using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Composition;
using System.Numerics;

namespace PhotoViewer
{
    public sealed partial class MainWindow : Window
    {

        private string _folder;
        public ImagesRepository ImagesRepository { get; } = new ImagesRepository();

        public MainWindow()
        {
            this.InitializeComponent();
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

        private async void AboutClick(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "About Simple Photo Editor",
                Content = "Done exclusively for //Build 2022",
                CloseButtonText = "Ok"
            };
            //Explain XamlRoot concept. You can get the XamlRoot of any UIElement
            dialog.XamlRoot =  this.Content.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}
