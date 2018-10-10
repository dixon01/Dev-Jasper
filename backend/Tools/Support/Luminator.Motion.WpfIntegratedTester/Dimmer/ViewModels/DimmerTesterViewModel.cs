namespace Luminator.Motion.WpfIntegratedTester.Dimmer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Common.SystemManagement.Host.Path;

    using Luminator.AudioSwitch.WpfSerialPortTester.ViewModels;
    using Luminator.PeripheralDimmer;
    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Models;
    using Luminator.PeripheralDimmer.Processor;
    using Luminator.PeripheralDimmer.Types;
    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;
    
    public class DimmerTesterViewModel : BaseViewModel, IDisposable
    {
        #region Fields

        private DimmerProcessor dimmerProcessor;

        private RangeScaleType selectedRangeScale;

        private ObservableCollection<SerialPortInfo> serialPorts;

        private SerialPortInfo selectedSerialPort;

        private ObservableCollection<SerialMessage> messages;

        private DimmerImpl dimmer;

        private DimmerPeripheralSerialClient serialClient;

        private DimmerPeripheralConfig dimmerConfig;

        private DimmerQueryResponse lastDimmerResponse;

        private string togglePortDisplay;

        private string toggleAutomaticDimmingDisplay;

        private byte manualBrightnessLevel;

        private object serialLock = new object();

        private DimmerSendCommands selectedCommand;

        private DelegateCommand togglePortCommand;

        private DelegateCommand sendSelectedCommand;

        private DelegateCommand clearResponseMessagesCommand;

        private DelegateCommand toggleAutomaticDimmingCommand;
        #endregion

        #region Public Properties

        public DimmerProcessor DimmerProcessor
        {
            get { return dimmerProcessor; }
            set
            {
                if (dimmerProcessor == value) return;
                dimmerProcessor = value;
                RaisePropertyChanged(() => DimmerProcessor);
            }
        }

        public DimmerQueryResponse LastDimmerResponse
        {
            get { return lastDimmerResponse; }
            set
            {
                if (lastDimmerResponse == value) return;
                lastDimmerResponse = value;
                RaisePropertyChanged(() => LastDimmerResponse);
            }
        }

        public RangeScaleType SelectedRangeScale
        {
            get { return selectedRangeScale; }
            set
            {
                if (selectedRangeScale == value) return;
                selectedRangeScale = value;
                RaisePropertyChanged(() => SelectedRangeScale);

                if (IsOpen)
                    SendSetSensorScaleRequestMessage(selectedRangeScale);
            }
        }

        public ObservableCollection<SerialPortInfo> SerialPorts
        {
            get { return serialPorts; }
            set
            {
                if (serialPorts == value) return;
                serialPorts = value;
                RaisePropertyChanged(() => SerialPorts);

                SelectedSerialPort = serialPorts?.FirstOrDefault();
            }
        }

        public SerialPortInfo SelectedSerialPort
        {
            get { return selectedSerialPort; }
            set
            {
                if (selectedSerialPort == value) return;
                selectedSerialPort = value;
                RaisePropertyChanged(() => SelectedSerialPort);

                TogglePortCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<SerialMessage> Messages
        {
            get { return messages; }
            set
            {
                if (messages == value) return;
                messages = value;
                RaisePropertyChanged(() => Messages);

                ClearResponseMessagesCommand.RaiseCanExecuteChanged();
            }
        }

        public DimmerPeripheralSerialClient SerialClient
        {
            get { return this.serialClient; }
            set
            {
                if (serialClient == value) return;
                serialClient = value;
                RaisePropertyChanged(() => SerialClient);
                RaisePropertyChanged(() => IsOpen);
                TogglePortCommand.RaiseCanExecuteChanged();
            }
        }

        public string TogglePortDisplay
        {
            get { return this.togglePortDisplay; }
            set
            {
                if (this.togglePortDisplay == value) return;
                this.togglePortDisplay = value;
                RaisePropertyChanged(() => TogglePortDisplay);
                RaisePropertyChanged(() => IsOpen);
                SendSelectedCommand.RaiseCanExecuteChanged();
            }
        }

        public string ToggleAutomaticDimmingDisplay
        {
            get { return this.toggleAutomaticDimmingDisplay; }
            set
            {
                if (toggleAutomaticDimmingDisplay == value) return;
                toggleAutomaticDimmingDisplay = value;
                RaisePropertyChanged(() => ToggleAutomaticDimmingDisplay);
                RaisePropertyChanged(() => IsAutomaticDimming);
            }
        }

        public bool IsOpen
        {
            get
            {
                if (this.serialClient == null) return false;
                return this.serialClient.IsOpen;
            }
            set
            {
                RaisePropertyChanged(() => IsOpen);
                TogglePortCommand.RaiseCanExecuteChanged();
                SendSelectedCommand.RaiseCanExecuteChanged();
            }
        }

        // Hack, automatic dimming status
        public bool IsAutomaticDimming
        {
            get
            {
                return dimmer != null;
            }
        }

        public byte ManualBrightnessLevel
        {
            get { return manualBrightnessLevel; }
            set
            {
                if (manualBrightnessLevel == value) return;

                //if (value < 0x14)
                //{
                //    manualBrightnessLevel = 0x14;
                //}
                //else
                //{
                //    manualBrightnessLevel = value;
                //}
                manualBrightnessLevel = value;
                RaisePropertyChanged(() => ManualBrightnessLevel);

                if (IsOpen)
                {
                    SendSetBrightnessRequestMessage(manualBrightnessLevel);
                }
            }
        }

        public DimmerSendCommands SelectedCommand
        {
            get { return this.selectedCommand; }
            set
            {
                if (this.selectedCommand == value) return;
                this.selectedCommand = value;
                RaisePropertyChanged(() => SelectedCommand);
            }
        }
        
        public DelegateCommand TogglePortCommand
        {
            get
            {
                if (this.togglePortCommand == null)
                    this.togglePortCommand = new DelegateCommand(p => this.TogglePort(), this.CanTogglePort);

                return this.togglePortCommand;
            }
        }
        
        public DelegateCommand SendSelectedCommand
        {
            get
            {
                if (this.sendSelectedCommand == null)
                    this.sendSelectedCommand =
                        new DelegateCommand(p => this.SendSelected(selectedCommand),
                            this.CanSendSelected);

                return this.sendSelectedCommand;
            }
        }
        
        public DelegateCommand ClearResponseMessagesCommand
        {
            get
            {
                if (this.clearResponseMessagesCommand == null)
                    this.clearResponseMessagesCommand =
                        new DelegateCommand(p => this.ClearResponseMessages(),
                            this.CanClearResponseMessages);

                return this.clearResponseMessagesCommand;
            }
        }

        public DelegateCommand ToggleAutomaticDimmingCommand
        {
            get
            {
                if (this.toggleAutomaticDimmingCommand == null)
                    this.toggleAutomaticDimmingCommand =
                        new DelegateCommand(p => this.ToggleAutomaticDimming(),
                            this.CanToggleAutomaticDimming);

                return this.toggleAutomaticDimmingCommand;
            }
        }

        #endregion

        #region Constructor

        public DimmerTesterViewModel()
        {
            manualBrightnessLevel = 0x14;
            DimmerProcessor = new DimmerProcessor();
            Messages = new ObservableCollection<SerialMessage>();

            Initialize();
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            if (this.serialClient != null)
            {
                this.serialClient.PeripheralDataReady -= SerialClientOnPeripheralDataReady;
                this.serialClient.Close();
            }
        }

        #endregion

        #region Member Methods and Operators

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize()
        {
            // get full path to our config file, read that in
            var configFileName = DimmerPeripheralConfig.DefaultDimmerPeripheralConfigFileName;
            var configFileFullPath = PathManager.Instance.GetPath(FileType.Config, configFileName);
            if (File.Exists(configFileFullPath) == false)
            {
                configFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);
            }

            this.dimmerConfig = DimmerPeripheralConfig.ReadDimmerPeripheralConfig(configFileFullPath);

            InitializeSerialPorts();
        }

        /// <summary>
        /// Initializes the serial ports list
        /// </summary>
        private void InitializeSerialPorts()
        {
            TogglePortDisplay = "CONNNECT";
            SerialPorts = new ObservableCollection<SerialPortInfo>();

            string[] comPortNames = null;
            comPortNames = SerialPort.GetPortNames();
            var ports = new List<string>(comPortNames);

            foreach (var port in ports)
            {
                SerialPorts.Add(new SerialPortInfo { BaudRate = 9600, Name = port });
            }

            SelectedSerialPort = serialPorts.FirstOrDefault(p => p.Name == "COM2");

            if (this.selectedSerialPort == null)
            {
                SelectedSerialPort = serialPorts.FirstOrDefault();
            }
        }
        
        /// <summary>
        /// Can open port
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanTogglePort(object obj)
        {
            return true;
        }

        /// <summary>
        /// Open the port.  Assume CPU is connected via RS-232 to the COM2 interface at 9600 bps 8N1
        /// </summary>
        private void TogglePort()
        {
            if (dimmerConfig == null || dimmerConfig.SerialPortSettings == null)
            {
                messages.Add(new SerialMessage
                {
                    Name = "ERROR",
                    ObjectData = "UNABLE TO READ DIMMER CONFIG FILE",
                });

                return;
            }

            // Stop the automatic dimmer
            if (dimmer != null)
            {
                ToggleAutomaticDimming();
            }

            if (this.serialClient != null && this.serialClient.IsOpen)
            {
                this.serialClient.PeripheralDataReady -= SerialClientOnPeripheralDataReady;
                this.serialClient.Close();
                this.serialClient.Dispose();

                messages.Add(new SerialMessage
                {
                    Name = "DISCONNECTED",
                    ObjectData = null,
                });

                TogglePortDisplay = "CONNNECT";
                return;
            }
            
            // Simply just change the name and baud rates
            dimmerConfig.SerialPortSettings.ComPort = selectedSerialPort.Name;
            dimmerConfig.SerialPortSettings.BaudRate = selectedSerialPort.BaudRate;
            dimmerConfig.SerialPortSettings.DataBits = selectedSerialPort.DataBits;
            dimmerConfig.SerialPortSettings.DtrControl = selectedSerialPort.DtrControl;
            dimmerConfig.SerialPortSettings.RtsControl = selectedSerialPort.RtsControl;
            
            SerialClient = new DimmerPeripheralSerialClient(dimmerConfig.SerialPortSettings);
            this.serialClient.PeripheralDataReady += SerialClientOnPeripheralDataReady;
            
            try
            {
                this.serialClient.StartBackgroundProcessing(250);
                
                if (this.serialClient.IsOpen)
                {
                    TogglePortDisplay = "DISCONNNECT";

                    messages.Add(new SerialMessage
                    {
                        Name = string.Format("CONNECTED: {0}", selectedSerialPort.Name),
                        ObjectData = null,
                    });
                }
            }
            catch (Exception ex)
            {
                messages.Add(new SerialMessage
                {
                    Name = "Error",
                    ObjectData = ex
                });

                if (this.serialClient != null && this.serialClient.IsOpen)
                {
                    this.serialClient.PeripheralDataReady -= SerialClientOnPeripheralDataReady;
                    this.serialClient.Close();
                    this.serialClient.Dispose();
                }

                TogglePortDisplay = "CONNNECT";
            }
        }

        /// <summary>
        /// Received a serial message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialClientOnPeripheralDataReady(object sender, PeripheralDataReadyEventArg e)
        {
            if (e.Message is DimmerQueryResponse)
            {
                // TODO Validate the checksum
                LastDimmerResponse = e.Message as DimmerQueryResponse;
            }
        }

        /// <summary>
        /// Send the poll request message
        /// </summary>
        private void SendPollRequestMessage()
        {
            if (!IsOpen)
            {
                messages.Add(new SerialMessage
                {
                    Name = "ERROR",
                    ObjectData = "NOT CONNECTED"
                });
                return;
            }
            
            byte[] buffer = new DimmerPoll().ToBytes();
            messages.Add(new SerialMessage
            {
                Name = string.Format("TX({0}): Poll Request {1} ", buffer.Length, this.BytesToDisplayString(buffer)),
                ObjectData = System.Text.Encoding.ASCII.GetString(buffer)
            });

            bool response = this.serialClient.WritePollRequest();

            if (!response)
            {
                MessageBox.Show("SEND Poll Request TIMEOUT");
                messages.Add(new SerialMessage
                {
                    Name = "SEND Poll Request TIMEOUT",
                    ObjectData = null,
                });
            }
            else
            {
                messages.Add(new SerialMessage
                {
                    Name = string.Format("RX: Poll Request OK, ACKED???"),
                    ObjectData = null
                });
            }
        }

        /// <summary>
        /// Send the version request message
        /// </summary>
        private void SendVersionRequestMessage()
        {
            if (!IsOpen)
            {
                messages.Add(new SerialMessage
                {
                    Name = "ERROR",
                    ObjectData = "NOT CONNECTED"
                });
                return;
            }

            byte[] buffer = (new DimmerVersionRequest()).ToBytes();
            messages.Add(new SerialMessage
            {
                Name = string.Format("TX({0}): Version Request {1} ", buffer.Length, this.BytesToDisplayString(buffer)),
                ObjectData = System.Text.Encoding.ASCII.GetString(buffer)
            });

            VersionInfo response = this.serialClient.WriteVersionRequest();

            if (response == null)
            {
                MessageBox.Show("SEND VersionRequestMessage TIMEOUT");
                messages.Add(new SerialMessage
                {
                    Name = "SEND VersionRequestMessage TIMEOUT",
                    ObjectData = null,
                });
            }
            else
            {
                //buffer = response.ToBytes();
                //messages.Add(new SerialMessage
                //{
                //    Name = string.Format("RX({0}): Version Response", buffer.Length),
                //    ObjectData = this.BytesToDisplayString(buffer)
                //});

                messages.Add(new SerialMessage
                {
                    Name = string.Format("RX: Version Response"),
                    ObjectData = response.ToString()
                });
            }
        }

        /// <summary>
        /// Send the query request message
        /// </summary>
        private void SendQueryRequestMessage()
        {
            if (!IsOpen)
            {
                messages.Add(new SerialMessage
                {
                    Name = "ERROR",
                    ObjectData = "NOT CONNECTED"
                });
                return;
            }

            byte[] buffer = (new DimmerQueryRequest()).ToBytes();
            messages.Add(new SerialMessage
            {
                Name = string.Format("TX({0}): Query Request {1} ", buffer.Length, this.BytesToDisplayString(buffer)),
                ObjectData = System.Text.Encoding.ASCII.GetString(buffer)
            });

            bool response = this.serialClient.WriteQueryRequest();

            if (!response)
            {
                MessageBox.Show("SEND Query Request TIMEOUT");
                messages.Add(new SerialMessage
                {
                    Name = "SEND Query Request TIMEOUT",
                    ObjectData = null,
                });
            }
            else
            {
                messages.Add(new SerialMessage
                {
                    Name = string.Format("RX: Query Request OK, ACKED???"),
                    ObjectData = null
                });
            }
        }

        /// <summary>
        /// Send the set sensor scale  request message
        /// </summary>
        private void SendSetSensorScaleRequestMessage(RangeScaleType scale)
        {
            if (!IsOpen)
            {
                messages.Add(new SerialMessage
                {
                    Name = "ERROR",
                    ObjectData = "NOT CONNECTED"
                });
                return;
            }
            
        }

        /// <summary>
        /// Send the set brightness request message
        /// </summary>
        private void SendSetBrightnessRequestMessage(byte brightness)
        {
            if (!IsOpen)
            {
                messages.Add(new SerialMessage
                {
                    Name = "ERROR",
                    ObjectData = "NOT CONNECTED"
                });
                return;
            }
            
        }

        /// <summary>
        /// Can send command
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanSendSelected(object obj)
        {
            return IsOpen && this.selectedCommand != null;
        }

        /// <summary>
        /// Send command
        /// </summary>
        /// <returns></returns>
        private void SendSelected(DimmerSendCommands command)
        {
            switch (command)
            {
                case DimmerSendCommands.PollRequest:
                    SendPollRequestMessage();
                    break;

                case DimmerSendCommands.QueryRequest:
                    SendQueryRequestMessage();
                    break;

                case DimmerSendCommands.VersionRequest:
                    SendVersionRequestMessage();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Can clear response messages
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanClearResponseMessages(object obj)
        {
            return true;
        }

        /// <summary>
        /// Clear response messages
        /// </summary>
        /// <returns></returns>
        private void ClearResponseMessages()
        {
            this.messages.Clear();
        }

        /// <summary>
        /// Can toggle automatic dimming
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanToggleAutomaticDimming(object obj)
        {
            return true;
        }

        /// <summary>
        /// Toggle the automatic dimming
        /// </summary>
        private void ToggleAutomaticDimming()
        {
            if (this.IsOpen)
            {
                TogglePort();
            }

            if (dimmer == null)
            {
                dimmerConfig.SerialPortSettings.ComPort = selectedSerialPort.Name;
                dimmerConfig.SerialPortSettings.BaudRate = selectedSerialPort.BaudRate;
                dimmerConfig.SerialPortSettings.DataBits = selectedSerialPort.DataBits;
                dimmerConfig.SerialPortSettings.DtrControl = selectedSerialPort.DtrControl;
                dimmerConfig.SerialPortSettings.RtsControl = selectedSerialPort.RtsControl;

                dimmer = new DimmerImpl(dimmerConfig);
                dimmer.Start();
                dimmer.Client.PeripheralDataReady += SerialClientOnPeripheralDataReady;

                messages.Add(new SerialMessage
                {
                    Name = "DIMMER STARTED",
                    ObjectData = null,
                });

                ToggleAutomaticDimmingDisplay = "STOP";
            }
            else
            {
                dimmer.Client.PeripheralDataReady -= SerialClientOnPeripheralDataReady;
                dimmer.Stop();
                dimmer.Dispose();
                dimmer = null;

                messages.Add(new SerialMessage
                {
                    Name = "DIMMER STOPPED",
                    ObjectData = null,
                });

                ToggleAutomaticDimmingDisplay = "STOP";
            }
        }

        /// <summary>
        /// Formats byte array to readable string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string BytesToDisplayString(byte[] bytes)
        {
            string result = bytes.Aggregate(string.Empty, (current, b) => current + b.ToString("X2") + " ");

            return result.TrimEnd();
        }

        #endregion
    }
}
