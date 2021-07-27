using ContosoAirlinePOSCs.IoC;
using ContosoAirlinePOSCs.Services.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ContosoAirlinePOSCs
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            var rootFrame = Content as Frame;

            var navigationService = DIHelper.Resolve<INavigationService>();
            navigationService.InitializeFrame(rootFrame);
        }
    }
}
