#include "pch.h"
#include "MainWindow.h"
#include "INavigationService.h"
#include "MainWindow.g.cpp"
#include <winrt/Windows.UI.Xaml.Interop.h>

using namespace winrt;
using namespace Microsoft::UI::Xaml;
using namespace Microsoft::UI::Xaml::Controls;
using namespace Microsoft::UI::Xaml::Navigation;
using namespace winrt::ContosoAirlinePOSCpp::implementation;

namespace winrt::ContosoAirlinePOSCpp::implementation
{
    MainWindow::MainWindow()
    {
        INavigationService _navigationService;
        InitializeComponent();
        // The XAML markup defined the Frame and named as MainFrame (x:Name="MainFrame") 
        // MainFrame is a property of the class MainWindow and you can get the object via MainWindow::MainFrame()

        _navigationService.InitializeFrame(MainWindow::MainFrame());
        _navigationService.NavigateTo(xaml_typename<LoginPage>());
    }

    void MainWindow::MainFrame_Navigated(winrt::Windows::Foundation::IInspectable const& sender, winrt::Microsoft::UI::Xaml::Navigation::NavigationEventArgs const& e)
    {
        if (!MainFrame().CanGoBack()) 
        {
            INavigationService _navigationService;
            _navigationService.InitializeFrame(MainWindow::MainFrame());
        }
    }
}
