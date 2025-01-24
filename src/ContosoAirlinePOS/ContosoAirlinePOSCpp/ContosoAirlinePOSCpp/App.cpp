#include "pch.h"

#include "App.h"
#include "MainWindow.h"

using namespace winrt;
using namespace Windows::Foundation;
using namespace Microsoft::UI::Xaml;
using namespace Microsoft::UI::Xaml::Controls;
using namespace Microsoft::UI::Xaml::Navigation;
using namespace ContosoAirlinePOSCpp;
using namespace ContosoAirlinePOSCpp::implementation;

/// <summary>
/// Initializes the singleton application object.  This is the first line of authored code
/// executed, and as such is the logical equivalent of main() or WinMain().
/// </summary>
App::App()
{
    InitializeComponent();

#if defined _DEBUG && !defined DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION
    UnhandledException([this](IInspectable const&, UnhandledExceptionEventArgs const& e)
    {
        if (IsDebuggerPresent())
        {
            auto errorMessage = e.Message();
            __debugbreak();
        }
    });
#endif
}

/// <summary>
/// Invoked when the application is launched normally by the end user.
/// </summary>
/// <param name="e">Details about the launch request and process.</param>
void App::OnLaunched(LaunchActivatedEventArgs const&)
{
    //MainWindow is a WinRT object, so you should use make to instantiate it and call the public API:
    window = make<MainWindow>();
    window.Activate();    
}

