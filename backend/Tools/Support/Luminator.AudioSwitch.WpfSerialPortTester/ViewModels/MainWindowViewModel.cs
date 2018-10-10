// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="MainWindowViewModel.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Natraj Bontha</author>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.AudioSwitch.WpfSerialPortTester.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Text;
    using System.Timers;

    using Luminator.AudioSwitch.WpfSerialPortTester.Properties;
    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;
    using Luminator.UIFramework.ResourceLibrary.Utils;

    using NLog;

    using Timer = System.Timers.Timer;
    using System.Xml.Serialization;
    using Microsoft.Win32;

    public class MainWindowViewModel : BaseViewModel, IDisposable
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        private AudioSwitchSerialClient audioSwitchSerialClient;

        private AudioSwitchSerialClient audioSwitchSerialClientToUse;

        private DelegateCommand connectCommand;

        private DelegateCommand connectUsingAudioSwitchCommand;

        private bool isConnected;

        private string messageReceived;

        private string selectedMessage;

        //  private DelegateCommand sendCommand;

        private DelegateCommand closingCommand;

        private DelegateCommand sendUsingAudioSwitchCommand;

        private bool showLoopBack;

        private bool useLoopBackSerial;

        private SerialPortInfo selectedComPort;

        private SerialPortInfo selectedLoopBackComPort;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.AudioConfigViewModel = new AudioConfigViewModel();
            this.SetVolumeViewModel = new SetVolumeViewModel
            {
                InteriorVolume = Settings.Default.InteriorVolume,
                ExteriorVolume = Settings.Default.ExteriorVolume
            };
            this.AudioSpeakersViewModel = new AudioSpeakersViewModel
            {
                InteriorSpeakersEnabled = Settings.Default.InteriorActive,
                ExteriorSpeakersEnabled = Settings.Default.ExteriorActive
            };
            this.GpioStatusViewModel = new GpioStatusViewModel();
            this.ScanAndInitSerialPorts();

            if (SerialPortList.Any(x => x.Name == Settings.Default.AudioBoxComPort))
            {
                this.SelectedComPort = SerialPortList.FirstOrDefault(x => x.Name == Settings.Default.AudioBoxComPort);
            }
            else
            {
                this.SelectedComPort = SerialPortList.FirstOrDefault();
            }

            if (SerialPortList.Any(x => x.Name == "COM4"))
            {
                this.SelectedLoopBackComPort = SerialPortList.FirstOrDefault(x => x.Name == "COM4");
            }
            else
            {
                this.SelectedLoopBackComPort = SerialPortList.FirstOrDefault();
            }

            this.ShowLoopBack = Settings.Default.UseLoopBack;
            this.AudioDelayOptions = new ObservableCollection<string>(Settings.Default.AudioDelayOptions.Split(','));
            this.UseLoopBackSerial = false;
            this.InitializeMessagesChoices();
            this.SelectedMessage = "Poll";
            this.NotificationMessages = new ObservableCollection<string>();
            if (Settings.Default.FakeGpioData)
                this.GenerateFakeGpioData();
            this.InvokeOnUI(() => { this.MessageReceived = "Ready"; });
        }

        private void GenerateFakeGpioData()
        {
            GpioFakeTimer = new Timer { Interval = 5000 };
            GpioFakeTimer.Elapsed += this.GpioFakeTimerOnElapsed;
            GpioFakeTimer.AutoReset = true;
            GpioFakeTimer.Enabled = true;
            GpioFakeTimer.Start();
        }

        private void GpioFakeTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Random rand = new Random(DateTime.Now.Second);
            this.InvokeOnUI(
                () =>
                    {
                        this.GpioStatusViewModel.Door1Active = rand.Next(2) == 0;
                        this.GpioStatusViewModel.Door2Active = rand.Next(2) == 0;
                        this.GpioStatusViewModel.ExteriorSpeakersActive = rand.Next(2) == 0;
                        this.GpioStatusViewModel.InteriorSpeakersActive = rand.Next(2) == 0;
                        this.MessageReceived = DateTime.Now + " Fake Gpio Data => " + "Door 1 = " + this.GpioStatusViewModel.Door1Active +
                                                ", Door 2 = " + this.GpioStatusViewModel.Door2Active +
                                                ", Interior Speakers = " + this.GpioStatusViewModel.InteriorSpeakersActive +
                                                ", Exterior Speakers = " + this.GpioStatusViewModel.ExteriorSpeakersActive;
                    });

        }

        #endregion

        #region Public Properties (Alphabetically Sorted)

        public AudioConfigViewModel AudioConfigViewModel { get; set; }

        public ObservableCollection<string> AudioDelayOptions { get; set; }

        public AudioSpeakersViewModel AudioSpeakersViewModel { get; set; }

        public static System.Timers.Timer GpioFakeTimer;

        public string ConnectButtonLabel
        {
            get
            {
                if (this.IsInDesignMode)
                {
                    return "Connect to Audio Board: (In Design)";
                }
                return this.audioSwitchSerialClient != null && this.audioSwitchSerialClient.IsComPortOpen ?
                        "Disconnect from Audio Board" : "Connect to Audio Board";
            }
        }

        public DelegateCommand ConnectCommand
        {
            get
            {
                if (this.connectCommand == null)
                {
                    this.connectCommand = new DelegateCommand(p => this.Connect(), this.CanConnect);
                }

                return this.connectCommand;
            }
        }

        public DelegateCommand ConnectUsingAudioSwitchCommand
        {
            get
            {
                if (this.connectUsingAudioSwitchCommand == null)
                {
                    this.connectUsingAudioSwitchCommand = new DelegateCommand(p => this.ConnectToAudioSwitch(), this.CanConnect);
                }

                return this.connectUsingAudioSwitchCommand;
            }
        }

        public GpioStatusViewModel GpioStatusViewModel { get; set; }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
            set
            {
                this.isConnected = value;
                this.RaisePropertyChanged(() => this.IsConnected);
            }
        }

        public string MessageReceived
        {
            get
            {
                return this.messageReceived;
            }
            set
            {
                this.messageReceived = value;
                this.NotificationMessages.Insert(0, value);
                this.RaisePropertyChanged(() => this.MessageReceived);
            }
        }

        public ObservableCollection<string> NotificationMessages { get; set; }

        public ObservableCollection<string> PeripheralMessageTypesCollection { get; set; }

        public ObservableCollection<string> PinSenceTypesCollection { get; set; }

        public SerialPortInfo SelectedComPort
        {
            get { return selectedComPort; }
            set
            {
                if (selectedComPort == value) return;
                selectedComPort = value;
                this.RaisePropertyChanged(() => this.SelectedComPort);
            }
        }

        public SerialPortInfo SelectedLoopBackComPort
        {
            get { return selectedLoopBackComPort; }
            set
            {
                if (selectedLoopBackComPort == value) return;
                selectedLoopBackComPort = value;
                this.RaisePropertyChanged(() => this.SelectedLoopBackComPort);
            }
        }

        public string SelectedMessage
        {
            get
            {
                return this.selectedMessage;
            }
            set
            {
                this.selectedMessage = value;
                // Call OnPropertyChanged whenever the property is updated
                this.RaisePropertyChanged(() => this.SelectedMessage);
            }
        }



        public DelegateCommand SendUsingAudioSwitchCommand
        {
            get
            {
                if (this.sendUsingAudioSwitchCommand == null)
                {
                    this.sendUsingAudioSwitchCommand = new DelegateCommand(p => this.SendToAudioSwitch(), this.CanConnect);
                }
                return this.sendUsingAudioSwitchCommand;
            }
        }

        public DelegateCommand ClosingCommand
        {
            get
            {
                if (this.closingCommand == null)
                {
                    this.closingCommand = new DelegateCommand(p => this.ClosingWindow(), this.CanClose);
                }
                return this.closingCommand;
            }
        }

        private bool CanClose(object obj)
        {
            return true;
        }

        private void ClosingWindow()
        {
            //  GpioFakeTimer.Stop();
            Settings.Default.AudioBoxComPort = this.SelectedComPort.Name;
            Settings.Default.InteriorVolume = this.SetVolumeViewModel.InteriorVolume;
            Settings.Default.ExteriorVolume = this.SetVolumeViewModel.ExteriorVolume;
            Settings.Default.InteriorActive = this.AudioSpeakersViewModel.InteriorSpeakersEnabled;
            Settings.Default.ExteriorActive = this.AudioSpeakersViewModel.ExteriorSpeakersEnabled;
            Settings.Default.Save();
        }

        public ObservableCollection<SerialMessage> SerialMessageList { get; set; } = new ObservableCollection<SerialMessage>();

        public ObservableCollection<SerialPortInfo> SerialPortList { get; set; } = new ObservableCollection<SerialPortInfo>();

        public SetVolumeViewModel SetVolumeViewModel { get; set; }

        public bool ShowLoopBack
        {
            get
            {
                return this.showLoopBack;
            }
            set
            {
                this.showLoopBack = value;
                this.RaisePropertyChanged(() => this.ShowLoopBack);
            }
        }

        public bool UseLoopBackSerial
        {
            get
            {
                return this.useLoopBackSerial;
            }
            set
            {
                this.useLoopBackSerial = value;
                this.RaisePropertyChanged(() => this.UseLoopBackSerial);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The open serial port.</summary>
        /// <param name="comPort">The com Port.</param>
        /// <param name="baudRate">The baud Rate.</param>
        /// <param name="dataBits">The data Bits.</param>
        /// <param name="parity">The parity.</param>
        /// <param name="stopBits">The stop Bits.</param>
        /// <param name="bufferSize">The buffer Size.</param>
        /// <returns>The <see cref="SerialPort" />.</returns>
        public static SerialPort OpenSerialPort(
            string comPort,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int bufferSize = 4096)
        {
            SerialPort port = null;
            try
            {
                port = new SerialPort(comPort, baudRate, parity, dataBits, stopBits) { Encoding = Encoding.UTF8, ReadBufferSize = bufferSize, WriteBufferSize = bufferSize };
                port.Open();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return port;
        }

        public void Dispose()
        {
            GpioFakeTimer.Stop();
        }

        #endregion

        #region Methods

        private void AudioSwitchSerialClientPeripheralDataReceived(object sender, PeripheralDataReceivedEventArgs args)
        {
            try
            {
                var baseMessage = args.Message;
                if (baseMessage != null)
                {
                    this.InvokeOnUI(
                        () =>
                            {
                                // Test refactor to the base AudioSwitchBase
                                var messageType = baseMessage.Header.MessageType;

                                switch (messageType)
                                {
                                    case PeripheralMessageType.AudioVersionResponse:
                                        this.MessageReceived = $"{DateTime.Now} Data Received => { baseMessage.ToString() }";
                                        break;
                                    default:
                                        this.MessageReceived = $"{DateTime.Now} Data Received => { baseMessage.ToString().Split('.').Last() }";
                                        break;
                                }
                            });
                }
            }
            catch (InvalidDataException exception)
            {
                this.InvokeOnUI(
                    () =>
                        {
                            this.MessageReceived = exception.Message;
                        });
            }
        }

        private bool CanConnect(object obj)
        {
            return this.SerialPortList.Count > 0;
        }


        private void Connect()
        {
            SerialPort serialPort = null;
            try
            {
                if (this.SelectedComPort != null)
                {
                    Logger.Debug("Opening Serial " + this.SelectedComPort.Name);
                    serialPort = OpenSerialPort(this.SelectedComPort.Name, this.SelectedComPort.BaudRate);
                    if (serialPort.IsOpen)
                    {
                        serialPort.RtsEnable = true;
                        this.InvokeOnUI(() => { this.MessageReceived = "Ready to receive on Serial " + this.SelectedComPort.Name; });
                        serialPort.DataReceived += (sender, args) =>
                            {
                                if (args.EventType == SerialData.Chars)
                                {
                                    this.InvokeOnUI(() => { this.MessageReceived = serialPort.ReadLine(); });
                                    serialPort.Close();
                                }
                                else if (args.EventType == SerialData.Eof)
                                {
                                    this.InvokeOnUI(() => { this.MessageReceived = "SerialData.Eof Occurred."; });
                                }
                                else
                                {
                                    this.InvokeOnUI(() => { this.MessageReceived = "In DataReceived but No Data!"; });
                                }
                            };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        bool DisconnectAudioSwitchSerialClient()
        {
            // Disconnect from AudioSwitch 
            if (this.audioSwitchSerialClient != null && this.audioSwitchSerialClient.IsComPortOpen)
            {
                this.audioSwitchSerialClient.PeripheralDataReceived -= this.AudioSwitchSerialClientPeripheralDataReceived;
                this.audioSwitchSerialClient.GpioChanged -= this.AudioSwitchSerialClientGpioChanged;
                this.audioSwitchSerialClient.Close();
                this.InvokeOnUI(() => { this.MessageReceived = "Disconnected from Audio Switch "; });
                this.RaisePropertyChanged(() => this.ConnectButtonLabel);
                this.IsConnected = false;
                return true;
            }

            return false;
        }

        private void ConnectToAudioSwitch()
        {
            if (this.DisconnectAudioSwitchSerialClient())
            {
                return;
            }

            try
            {
                //Connect to AudioSwitch
                if (this.SelectedComPort != null)
                {
                    var settings = new SerialPortSettings(this.SelectedComPort.Name);
                    this.audioSwitchSerialClient = new AudioSwitchSerialClient(settings);

                    var peripheralVersionsInfo = this.audioSwitchSerialClient.WriteVersionRequest();

                    if (peripheralVersionsInfo != null)
                    {
                        this.AudioConfigViewModel.SetFromPeripheralAudioConfig(this.audioSwitchSerialClient.ReadPeripheralAudioConfigFile("PeripheralAudioConfig.xml"));

                        if (this.audioSwitchSerialClient.IsComPortOpen)
                        {
                            this.audioSwitchSerialClient.PeripheralDataReceived += this.AudioSwitchSerialClientPeripheralDataReceived;
                            this.audioSwitchSerialClient.GpioChanged += this.AudioSwitchSerialClientGpioChanged;
                            this.InvokeOnUI(() => { this.MessageReceived = "Connected to Audio Switch at  " + this.SelectedComPort.Name; });
                            this.RaisePropertyChanged(() => this.ConnectButtonLabel);
                            this.IsConnected = true;
                        }
                    }
                    else
                    {
                        // Not a Audio Mux we connected to on this serial port.
                        this.DisconnectAudioSwitchSerialClient();
                        this.InvokeOnUI(() => { this.MessageReceived = "Audio Switch not found on selected Serial Port!"; });
                    }
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                this.MessageReceived = exception.Message;
            }
        }

        private void AudioSwitchSerialClientGpioChanged(object sender, PeripheralGpioEventArg e)
        {
            try
            {
                this.InvokeOnUI(() =>
               {
                   foreach (var gpioInfo in e.GpioInfo)
                   {
                       if (gpioInfo.Gpio == PeripheralGpioType.Door1) this.GpioStatusViewModel.Door1Active = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.Door2) this.GpioStatusViewModel.Door2Active = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.InteriorActive) this.GpioStatusViewModel.InteriorSpeakersActive = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.ExteriorActive) this.GpioStatusViewModel.ExteriorSpeakersActive = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.StopRequest) this.GpioStatusViewModel.StopRequest = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.PushToTalk) this.GpioStatusViewModel.PushToTalk = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.AdaStopRequest) this.GpioStatusViewModel.AdaStopRequest = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.Reverse) this.GpioStatusViewModel.Reverse = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.RadioSpeakerMuted) this.GpioStatusViewModel.RadioSpeakerMuted = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.RadioSpeakerNonMuting) this.GpioStatusViewModel.RadioSpeakerNonMuting = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.InteriorSpeakerMuted) this.GpioStatusViewModel.InteriorSpeakerMuted = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.ExteriorSpeakerMuted) this.GpioStatusViewModel.ExteriorSpeakerMuted = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.InteriorSpeakerNonMuting) this.GpioStatusViewModel.InterorSpeakerNonMuting = gpioInfo.Active;
                       if (gpioInfo.Gpio == PeripheralGpioType.ExteriorSpeakerNonMuting) this.GpioStatusViewModel.ExteriorSpeakerNonMuting = gpioInfo.Active;

                   }

                   if (e.RawPinStatus != 0)
                   {
                       this.MessageReceived = "Gpio Raw Status Hex (int) => " + e.RawPinStatus.ToString("X4") + "  ( " + e.RawPinStatus + " )";
                       this.MessageReceived = "Gpio Raw Status Bin => " + string.Join(" ", ((int)e.RawPinStatus).IntToBinaryString().Splice(2));
                   }
                   //this.MessageReceived = DateTime.Now + " Gpio Changed Received => " + 
                   //                         "Door 1 = " + this.GpioStatusViewModel.Door1Active +
                   //                         ", Door 2 = " + this.GpioStatusViewModel.Door2Active +
                   //                         ", Int Speakers = " + this.GpioStatusViewModel.InteriorSpeakersActive +
                   //                         ", Ext Speakers = " + this.GpioStatusViewModel.ExteriorSpeakersActive +
                   //                         ", Stop Req. = " + this.GpioStatusViewModel.StopRequest +
                   //                         ", PTT = " + this.GpioStatusViewModel.PushToTalk +
                   //                         ", ADA Stop Req. = " + this.GpioStatusViewModel.AdaStopRequest;
               });
            }
            catch (Exception exception)
            {
                this.MessageReceived = exception.Message;

            }
        }




        private void InitializeMessagesChoices()
        {
            try
            {
                var listStrings = EnumHelpers.EnumToList<PeripheralMessageType>(true);

                var exceptItems = new List<string>
                                      {
                                          "Unknown",
                                          "Status",
                                          "Ack",
                                          "Nak",
                                          "Switch Roles",
                                          "Data",
                                          "Audio Version Response",
                                          "Gpio Status Response",
                                          "Audio Status Response",
                                      };
                // exclude Image for now
                listStrings = listStrings.FindAll(m => m.StartsWith("Image") == false);
                listStrings.Add("Image Update");    // TODO
                var filteredItems = listStrings.Except(exceptItems);
                var sortedItems = new ObservableCollection<string>(filteredItems.OrderBy(i => i));
                this.InvokeOnUI(() => { this.PeripheralMessageTypesCollection = new ObservableCollection<string>(sortedItems); });
            }
            catch (Exception exception)
            {
                Logger.Debug(exception.Message);
            }
        }

        private void ScanAndInitSerialPorts()
        {
            string[] arrayComPortsNames = null;
            arrayComPortsNames = SerialPort.GetPortNames();
            var ports = new List<string>(arrayComPortsNames);
            foreach (var port in ports)
            {
                this.SerialPortList.Add(new SerialPortInfo { BaudRate = 115200, Name = port });
            }
        }

        private void SendToAudioSwitch()
        {
            if (this.UseLoopBackSerial)
            {
                this.audioSwitchSerialClientToUse?.Close();
                this.audioSwitchSerialClientToUse = null;
                this.audioSwitchSerialClientToUse = new AudioSwitchSerialClient(new SerialPortSettings(this.SelectedLoopBackComPort.Name));
            }
            else
            {
                this.audioSwitchSerialClientToUse = this.audioSwitchSerialClient;
            }

            if (this.SelectedComPort != null && this.audioSwitchSerialClientToUse != null)
            {
                if (this.audioSwitchSerialClientToUse.IsComPortOpen)
                {
                    this.InvokeOnUI(() => { this.MessageReceived = DateTime.Now + " Sending Message => " + this.SelectedMessage; });
                    switch (this.SelectedMessage)
                    {
                        case "Audio Config":
                            this.audioSwitchSerialClientToUse.WriteAudioStatusInterval((byte)this.AudioConfigViewModel.AudioStatusDelay);
                            break;
                        case "Set Volume":
                            this.audioSwitchSerialClientToUse.WriteSetVolume((byte)this.SetVolumeViewModel.InteriorVolume, (byte)this.SetVolumeViewModel.ExteriorVolume);
                            break;
                        case "Audio Output Enable":
                            this.audioSwitchSerialClientToUse.WriteAudioEnabled(this.AudioSpeakersViewModel.AudioStatus);
                            break;
                        case "Poll":
                            this.audioSwitchSerialClientToUse.WriteAudioStausRequest();
                            break;
                        case "Request Version":
                            this.audioSwitchSerialClientToUse.WriteVersionRequest();
                            break;
                        case "Image Update Start Version Update":    // TBD    Start an image update on the audio mux, ask use for the file                                                     
                        case "Image Update":
                            this.InvokeOnUI(() => { this.MessageReceived = "Image Updates is a Future operation!"; });
                            break;
                        default:
                            Logger.Debug("Should not be here");
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        #endregion
    }
}