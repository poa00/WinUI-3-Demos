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

namespace DemoBuildCs
{
    public sealed partial class MainWindow : Window
    {

        private IntPtr m_hwnd;

        public MainWindow()
        {
            this.InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(CustomTitleBar);

            splitView.PaneOpened += OnPaneOpened;
            m_hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        }

        private void OnPaneOpened(SplitView sender, object args)
        {
            string currentTheme = sender.RequestedTheme == ElementTheme.Default ? App.Current.RequestedTheme.ToString() : sender.RequestedTheme.ToString();
            themePanel.Children.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme).IsChecked = true;
        }

        async void OnBrowse(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            //Make folder Picker work in Win32
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, m_hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            textBox.Text = folder != null ? folder.Path : string.Empty;
        }

        void OnRun(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(textBox.Text) && !File.Exists(textBox.Text))
            {
                return;
            }

            statusTextBlock.Text = "Status: running";

            int depth = (int)numberBox.Value;

            list.Clear();
            var sw = Stopwatch.StartNew();
            ListDirectories(textBox.Text, depth);
            sw.Stop();

            richTextBlock.Blocks.Clear();
            Paragraph paragraph = new Paragraph();
            Run run = new();
            run.Text = list.ToString();
            paragraph.Inlines.Add(run);
            richTextBlock.Blocks.Add(paragraph);
            statusTextBlock.Text = $"Status: completed in {sw.Elapsed.TotalMilliseconds} ms";
        }

        void OnKeyDownTextBox(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                OnRun(null, null);
            }
        }

        StringBuilder list = new StringBuilder();
        private void ListDirectories(string dir, int depth)
        {
            if (depth < 0)
                return;
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    list.AppendLine($"{d}");
                    ListDirectories(d, depth - 1);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                list.AppendLine($"Unauthorized Access: {dir} - {ex.Message}");
            }
            catch (PathTooLongException ex)
            {
                list.AppendLine($"Path Too Long: {dir} - {ex.Message}");
            }
            catch (System.Exception ex)
            {
                list.AppendLine($"Exception: {ex.Message}");
            }
        }

        async void OnAbout(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = false;

            var description = new StringBuilder();
            var process = Process.GetCurrentProcess();
            foreach (ProcessModule module in process.Modules)
            {
                description.AppendLine(module.FileName);
            }
            cdTextBlock.Text = description.ToString();
            contentDialog.XamlRoot = gridRoot.XamlRoot;
            await contentDialog.ShowAsync();
        }

        void OnSettings(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            if (selectedTheme != null)
            {
                ((sender as RadioButton).XamlRoot.Content as SplitView).RequestedTheme = GetEnum<ElementTheme>(selectedTheme);
            }
        }

        void OnHelp(object sender, RoutedEventArgs e)
        {
            teachingTip.IsOpen = true;
        }

        private TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        void OnSendToBottom(object sender, RoutedEventArgs e)
        {
            PInvoke.User32.SetWindowPos(m_hwnd, PInvoke.User32.SpecialWindowHandles.HWND_BOTTOM, 0, 0, 0, 0,
                PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE |
                PInvoke.User32.SetWindowPosFlags.SWP_NOSIZE |
                PInvoke.User32.SetWindowPosFlags.SWP_NOACTIVATE);
        }

        void OnWindowMinimize(object sender, RoutedEventArgs e)
        {
            PInvoke.User32.ShowWindow(m_hwnd,
                 PInvoke.User32.WindowShowStyle.SW_MINIMIZE);
        }

        void OnWindowMaximize(object sender, RoutedEventArgs e)
        {
            PInvoke.User32.ShowWindow(m_hwnd,
                PInvoke.User32.WindowShowStyle.SW_MAXIMIZE);
        }

        void OnWindowRestore(object sender, RoutedEventArgs e)
        { 
            PInvoke.User32.ShowWindow(m_hwnd,
                PInvoke.User32.WindowShowStyle.SW_RESTORE);
        }
    }
}
