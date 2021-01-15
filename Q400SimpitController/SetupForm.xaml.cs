using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Q400SimpitController
{
    public partial class SetupForm : UserControl
    {
        public event EventHandler<Tuple<IPAddress, int, int>> SaveSetupEvent;
        public Setup Setup { get; set; }

        public SetupForm()
        {
            InitializeComponent();
//            this.DataContext = this;
//            if (Setup.SimPcIp != null) TxtSimPcIp.Text = Setup.SimPcIp.ToString();
//            if (Setup.SimPcPort.HasValue) TxtSimPcPort.Text = Setup.SimPcPort.Value.ToString();
//            if (Setup.LocalPort.HasValue) TxtLocalPort.Text = Setup.LocalPort.Value.ToString();

        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (IPAddress.TryParse(TxtSimPcIp.Text, out var simPcIp)
                && int.TryParse(TxtSimPcPort.Text, out var simPcPort) && simPcPort > 0
                && int.TryParse(TxtLocalPort.Text, out var localPort) && localPort > 0
            )
            {
                SaveSetupEvent?.Invoke(this, new Tuple<IPAddress, int, int>(simPcIp, simPcPort, localPort));
            }
        }
    }
}