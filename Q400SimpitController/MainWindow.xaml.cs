using Newtonsoft.Json;
using Q400SimpitController.Converter.Json;
using Q400SimpitController.ViewModel;
using Q400SimpitController.ViewModel.Messaging;
using System;
using System.Diagnostics;
using System.IO;

namespace Q400SimpitController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Setup data

        public SetupVM Setup { get; private set; }
        public CommunicationVM Communication { get; private set; }
        private readonly string _setupFilePath;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Converters = { new IpAddressConverter() }
        };

        #endregion

        public MainWindow()
        {
            Directory.CreateDirectory(Environment.SpecialFolder.LocalApplicationData.ToString());
            _setupFilePath = Environment.SpecialFolder.LocalApplicationData + "\\setup.json";
            Communication = new CommunicationVM(_setupFilePath, _jsonSerializerSettings);

            if (File.Exists(_setupFilePath))
            {
                var jsonString = File.ReadAllText(_setupFilePath);
                Setup = new SetupVM(JsonConvert.DeserializeObject<SetupMessage>(jsonString, _jsonSerializerSettings));
            }
            else
                Setup = new SetupVM();


            InitializeComponent();
            SetupForm.DataContext = Setup;

            Console.WriteLine("MainWindow!");
            Debugger.Log(0, "trace", "MainWindow");

        }
    }
}