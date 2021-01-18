using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Q400SimpitController.ViewModel.Messaging
{
    public class SetupMessage
    {
        public IPAddress SimPcIp { get; }
        public int LocalPort { get; }
        public int SimPcPort { get; }

        public SetupMessage(IPAddress simPcIp, int localPort, int simPcPort)
        {
            SimPcIp = simPcIp;
            LocalPort = localPort;
            SimPcPort = simPcPort;
        }
    }
}
