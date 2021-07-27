using System;
using System.Diagnostics; //Proccess
using System.IO; //File access
using System.Linq;
using System.Reflection;
using System.Text;

using Windows.Storage.Pickers;

using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;

namespace DemoBuildCs
{
    public  class FolderPickerEx 
    {
        FolderPicker mfolderPicker;

        public FolderPickerEx()
        {
            mfolderPicker = new FolderPicker();
            mfolderPicker.FileTypeFilter.Add("*");
            IntPtr hwnd = GetActiveWindow();
            WinRT.Interop.InitializeWithWindow.Initialize(mfolderPicker, hwnd);
        }

        public Windows.Storage.StorageFolder PickSingleFolder()
        {
           return  mfolderPicker.PickSingleFolderAsync().GetAwaiter().GetResult();
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();
    }
}