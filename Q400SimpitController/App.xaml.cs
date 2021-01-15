using System.Windows;
using System.Windows.Threading;

namespace Q400SimpitController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}