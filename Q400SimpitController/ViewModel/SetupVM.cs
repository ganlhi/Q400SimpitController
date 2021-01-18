using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Q400SimpitController.ViewModel.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Q400SimpitController.ViewModel
{
    public class SetupVM : ViewModelBase
    {
        public string SimPcIpString { get; set; }
        public int SimPcPort { get; set; }
        public int LocalPort { get; set; }

        public ICommand SaveCommand { get; }

        public SetupVM(SetupMessage message = null)
        {
            SaveCommand = new RelayCommand(Save);
            if(message != null)
            {
                SimPcIpString = message.SimPcIp.ToString();
                SimPcPort = message.SimPcPort;
                LocalPort = message.LocalPort;
            }
        }

        public void Save()
        {
            if (IPAddress.TryParse(SimPcIpString, out var simPcIp)
                && SimPcPort > 0
                && LocalPort > 0
            )
            {
                Messenger.Default.Send(new SetupMessage(simPcIp, LocalPort, SimPcPort));
            }

            Console.WriteLine("Saved !");
        }
    }
}
