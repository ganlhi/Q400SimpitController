using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Q400SimpitController.ViewModel.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Q400SimpitController.ViewModel
{
    public class CommunicationVM
    {
        private UdpClient _client;
        private IPEndPoint _endPoint;
        private IAsyncResult _curAsyncResult;
        private string _setupFilePath;
        private JsonSerializerSettings _jsonSerializerSettings;

        private SetupMessage Setup;

        public CommunicationVM(string setupFilePath, JsonSerializerSettings jsonSerializerSettings)
        {
            _setupFilePath = setupFilePath;
            _jsonSerializerSettings = jsonSerializerSettings;
            Messenger.Default.Register<SetupMessage>(this, OnSetup);
        }

        #region Event part
        private void OnSetup(SetupMessage message)
        {
            if (Setup != null && Setup.SimPcIp == message.SimPcIp && Setup.SimPcPort == message.SimPcPort && Setup.LocalPort == message.LocalPort) 
                return;

            Setup = message;

            File.WriteAllText(_setupFilePath, JsonConvert.SerializeObject(message, _jsonSerializerSettings));

            CheckStartSimPcCommunication();
        }
        #endregion

        private void CheckStartSimPcCommunication()
        {
            StopReceivingUdpFromSimPc();
            StartReceivingUdpFromSimPc();
        }

        private void StartReceivingUdpFromSimPc()
        {
            _client = new UdpClient(Setup.LocalPort);
            _endPoint = new IPEndPoint(Setup.SimPcIp, Setup.SimPcPort);
            _client.Connect(_endPoint);

            _curAsyncResult = _client.BeginReceive(new AsyncCallback(ReceivedUdpFromSimPc), null);
            //var msg = Utils.BuildMessage((short) 0x1016, (short) 2, singleByte: true);
            //_client.Send(msg, msg.Length);
        }

        private void StopReceivingUdpFromSimPc()
        {
            _client?.Close();
        }

        private void ReceivedUdpFromSimPc(IAsyncResult ar)
        {
            if (_curAsyncResult != ar) return;

            try
            {
                var buffer = _client.EndReceive(ar, ref _endPoint);
                Debugger.Log(0, "trace", string.Join("-", buffer.ToList().Select(b => $"{b:X}")));

                _client.BeginReceive(new AsyncCallback(ReceivedUdpFromSimPc), null);
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException || ex is SocketException) return;
                MessageBox.Show("An exception just occurred: " + ex.Message, "Exception",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
