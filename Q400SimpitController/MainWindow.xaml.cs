using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using Newtonsoft.Json;

namespace Q400SimpitController
{
    class IpAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }
    }
    
    public class Setup
    {
        public IPAddress SimPcIp {get; set;}
        public int? SimPcPort {get; set;}
        public int? LocalPort {get; set;}
    } 
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Setup data

        public Setup Setup { get; private set; }
        private readonly string _setupFilePath;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Converters = {new IpAddressConverter()}
        };

        #endregion

        #region UI stuff 

        public MainWindow()
        {
            Directory.CreateDirectory(Environment.SpecialFolder.LocalApplicationData.ToString());
            _setupFilePath = Environment.SpecialFolder.LocalApplicationData + "\\setup.json";

            if (File.Exists(_setupFilePath)) {
                var jsonString = File.ReadAllText(_setupFilePath);
                Setup = JsonConvert.DeserializeObject<Setup>(jsonString, _jsonSerializerSettings);
            }
            
            InitializeComponent();
//            this.DataContext = this;
            
            Console.WriteLine("MainWindow!");
            Debugger.Log(0, "trace", "MainWindow");
        }

        private void SetupFormOnSaveSetup(object sender, Tuple<IPAddress, int, int> e)
        {
            Debugger.Log(0, "trace", e.ToString());
            var (simPcIp, simPcPort, localPort) = e;
            if (simPcIp.Equals(Setup.SimPcIp) && simPcPort == Setup.SimPcPort && localPort == Setup.LocalPort) return;
            
            Setup = new Setup()
            {
                SimPcIp = simPcIp,
                SimPcPort = simPcPort,
                LocalPort = localPort,
            };
            
            
            File.WriteAllText(_setupFilePath, JsonConvert.SerializeObject(Setup, _jsonSerializerSettings));

            CheckStartSimPcCommunication();
        }

        #endregion

        #region Sim PC communications

        private UdpClient _client;
        private IPEndPoint _endPoint;
        private IAsyncResult _curAsyncResult;

        private void CheckStartSimPcCommunication()
        {
            StopReceivingUdpFromSimPc();
            if (Setup.SimPcIp == null || Setup.SimPcPort == null || Setup.LocalPort == null) return;
            StartReceivingUdpFromSimPc();
        }

        private void StartReceivingUdpFromSimPc()
        {
            if (Setup.SimPcIp == null || Setup.SimPcPort == null || Setup.LocalPort == null) return;
            _client = new UdpClient(Setup.LocalPort.Value);
            _endPoint = new IPEndPoint(Setup.SimPcIp, Setup.SimPcPort.Value);
            _client.Connect(_endPoint);

            _curAsyncResult = _client.BeginReceive(new AsyncCallback(ReceivedUdpFromSimPc), null);

            // test send
//            var msg = Utils.BuildMessage((short) 0x1016, (short) 2, singleByte: true);
//            _client.Send(msg, msg.Length);
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

        #endregion
    }
}