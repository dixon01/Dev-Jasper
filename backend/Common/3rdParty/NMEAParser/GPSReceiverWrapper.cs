using System;
using System.Collections.Generic;
using System.Text;
using PUtils;
using System.IO.Ports;

namespace NMEA
{
    public struct SatelliteData
    {
        public int PRNNumber;
        public int Elevation;
        public int Azimuth;
        public int SNR;

        public override string ToString()
        {
            return string.Format("PRN: {0:00}, Elevation: {1:00}°, Azimuth: {2:000}°, SNR: {3:00} dB", PRNNumber, Elevation, Azimuth, SNR);
        }

        public SatelliteData(int prn, int elevation, int azimuth, int snr)
        {
            PRNNumber = prn;
            Elevation = elevation;
            Azimuth = azimuth;
            SNR = snr;
        }
    }

    public class GPSReceiverWrapper
    {
        #region Properties

        SerialPort serialPort;
        StringBuilder sb;
        SynchronizedObjectQueue outQueue;

        StringBuilder textReceiver;

        #region Receiver data

        List<SatelliteData> satellites;

        public static bool FunctioningControl { get; private set; }

        public static string ReceiverStrings;

        #endregion

        public SerialPortSettings PortSettings { get; set; }

        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        private delegate void ProcessCommandDelegate(object[] parameters);
        private Dictionary<NMEA.SentenceIdentifiers, ProcessCommandDelegate> commandProcessor;

        delegate T NullChecker<T>(object parameter);
        NullChecker<int> intNullChecker = (x => x == null ? -1 : (int)x);
        NullChecker<double> dobleNullChecker = (x => x == null ? double.NaN : (double)x);
        NullChecker<string> stringNullChecker = (x => x == null ? string.Empty : (string)x);

        #endregion

        #region Constructor

        public GPSReceiverWrapper()
        {
            commandProcessor = new Dictionary<SentenceIdentifiers, ProcessCommandDelegate>()
            {
                { NMEA.SentenceIdentifiers.GGA, new ProcessCommandDelegate(ProcessGGA)},
                { NMEA.SentenceIdentifiers.GSV, new ProcessCommandDelegate(ProcessGSV)},
                { NMEA.SentenceIdentifiers.GLL, new ProcessCommandDelegate(ProcessGLL)},
                { NMEA.SentenceIdentifiers.RMC, new ProcessCommandDelegate(ProcessRMC)},
                { NMEA.SentenceIdentifiers.VTG, new ProcessCommandDelegate(ProcessVTG)},
                { NMEA.SentenceIdentifiers.TXT, new ProcessCommandDelegate(ProcessTXT)}
            };

            satellites = new List<SatelliteData>();

            sb = new StringBuilder();
            textReceiver = new StringBuilder();

            outQueue = new SynchronizedObjectQueue(100);
            outQueue.ElementAdded += new SynchronizedObjectQueue.ElementAddedEventHandler(outQueue_ElementAdded);

            serialPort = new SerialPort();
            serialPort.NewLine = "\r\n";
            serialPort.Encoding = Encoding.ASCII;
            serialPort.ReadTimeout = 50;
            serialPort.WriteTimeout = 50;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort_ErrorReceived);
        }        

        #endregion

        #region Private methods

        private void ProcessTXT(object[] parameters)
        {
            try
            {
                var totalMessages = (int)parameters[0];
                var currentMessageNumber = (int)parameters[1];

                if (currentMessageNumber == 1)
                    textReceiver = new StringBuilder();

                textReceiver.Append(parameters[3]);

                if (totalMessages == currentMessageNumber)
                {
                    if (GPSTextReceived != null)
                    {
                        GPSTextReceived(textReceiver.ToString());
                    }
                }
            }
            catch
            {
                //
            }
        }

        private void ProcessGGA(object[] parameters)
        {
            try
            {
                var gpsQualityIndicator = (string)parameters[5];
                var satellitesInUse = intNullChecker(parameters[6]);
                var precisionHorizontalDilution = dobleNullChecker(parameters[7]);
                var antennaAltitude = dobleNullChecker(parameters[8]);
                var antennaAltitudeUnits = (string)parameters[9];
                var geoidalSeparation = dobleNullChecker(parameters[10]);
                var geoidalSeparationUnits = (string)parameters[11];
                var differentialReferenceStation = intNullChecker(parameters[12]);

                if (GlobalGPSData != null)
                {
                    GlobalGPSData(gpsQualityIndicator, satellitesInUse, precisionHorizontalDilution, antennaAltitude, antennaAltitudeUnits,
                        geoidalSeparation, geoidalSeparationUnits, differentialReferenceStation);
                }
            }
            catch
            {
                //
            }
        }

        private void ProcessGSV(object[] paramters)
        {
            try
            {
                int totalMessages = (int)paramters[0];
                int currentMessageNumber = (int)paramters[1];

                if (currentMessageNumber == 1)
                    satellites.Clear();

                int satellitesDataItemsCount = (paramters.Length - 3) / 4;
                
                for (int i = 0; i < satellitesDataItemsCount; i++)
                {
                    satellites.Add(
                        new SatelliteData(
                            intNullChecker(paramters[3 + 4 * i]),
                            intNullChecker(paramters[4 + 4 * i]),
                            intNullChecker(paramters[5 + 4 * i]),
                            intNullChecker(paramters[6 + 4 * i])));
                }

                if (currentMessageNumber == totalMessages)
                {
                    if (SatellitesInfoUpdated != null)
                        SatellitesInfoUpdated(satellites.ToArray());
                }
            }
            catch
            {    
                //
            }
        }

        private void ProcessGLL(object[] parameters)
        {
            try
            {
                if (parameters[5].ToString() == "Valid")
                {
                    if (NewFix != null)
                    {
                        NewFix((DateTime)parameters[4],
                            new GeographicDimension((double)parameters[0], (Cardinals)Enum.Parse(typeof(Cardinals), (string)parameters[1])),
                            new GeographicDimension((double)parameters[2], (Cardinals)Enum.Parse(typeof(Cardinals), (string)parameters[3])));
                    }
                }
            }
            catch
            {
                //
            }
        }

        private void ProcessVTG(object[] parameters)
        {
            try
            {
                var trackTrue = dobleNullChecker(parameters[0]);
                var trackMagnetic = dobleNullChecker(parameters[2]);
                var speedKnots = dobleNullChecker(parameters[4]);
                var skUnits = (string)parameters[5];
                var speedKmh = dobleNullChecker(parameters[6]);
                var sKmUnits = (string)parameters[7];

                if (GroundSpeedMeashurement != null)
                {
                    GroundSpeedMeashurement(trackTrue, trackMagnetic, speedKnots, skUnits, speedKmh, sKmUnits);
                }
            }
            catch
            {
                //
            }
        }

        private void ProcessRMC(object[] parameters)
        {
            // not used
        }

        #endregion

        #region Handlers

        private void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (PortError != null)
                PortError(e);
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = serialPort.BytesToRead;
            byte[] bytes = new byte[bytesToRead];
            serialPort.Read(bytes, 0, bytesToRead);
            sb.Append(Encoding.ASCII.GetString(bytes));

            var temp = sb.ToString();
            int index = temp.LastIndexOf(serialPort.NewLine);
            if (index > 0)
            {
                sb.Remove(0, index + serialPort.NewLine.Length);
                temp.Remove(index);

                int startIndex = temp.IndexOf(NMEAParser.SentenceStartDelimiter);

                if (startIndex > 0)
                    temp = temp.Remove(0, startIndex);

                var lines = temp.Split(serialPort.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    outQueue.Add(string.Format("{0}{1}", lines[i], NMEAParser.SentenceEndDelimiter));
                }
            }
        }

        private void outQueue_ElementAdded()
        {
            try
            {
                var parsedSentence = NMEAParser.Parse((string)outQueue.Dequeque());
                FunctioningControl = true; // something parsed

                if (parsedSentence is NMEAStandartSentence)
                {
                    var parsedStandartSentence = (parsedSentence as NMEAStandartSentence);
                    if (commandProcessor.ContainsKey(parsedStandartSentence.SentenceID))
                    {
                        commandProcessor[parsedStandartSentence.SentenceID](parsedStandartSentence.parameters);
                    }
                }
            }
            catch
            {
                // is it a GPS receiver?
                FunctioningControl = false;
            }
        }

        #endregion

        #region Methods

        public void Open()
        {
            if (!IsOpen)
            {
                serialPort.PortName = PortSettings.PortName;
                serialPort.BaudRate = (int)PortSettings.PortBaudRate;
                serialPort.DataBits = (int)PortSettings.PortDataBits;
                serialPort.StopBits = PortSettings.PortStopBits;
                serialPort.Parity = PortSettings.PortParity;
                serialPort.Handshake = PortSettings.PortHandshake;

                serialPort.Open();
            }
            else
            {
                throw new InvalidOperationException("Port already opened");
            }
        }

        public void Close()
        {
            try
            {
                serialPort.Close();
            }
            catch
            {
            }
        }

        #endregion

        #region Events

        public delegate void NewFixHandler(DateTime fixTime, GeographicDimension latitude, GeographicDimension longitude);
        public NewFixHandler NewFix;

        public delegate void SatellitesInfoUpdatedHandler(SatelliteData[] satellites);
        public SatellitesInfoUpdatedHandler SatellitesInfoUpdated;

        public delegate void GlobalGPSDataHandler(string GPSQuality, int usedSatellitesNumber,
            double precisionHorizontalDilution, double antennaAltitude, string altitudeUnits,
            double geoidalSeparation, string geoidalSeparationUnits,
            int differentialReferenceStation);
        public GlobalGPSDataHandler GlobalGPSData;

        public delegate void GroundSpeedMeasurementHandler(double trackTrue, double trackMagnetic, double speedKnots, string skUnits, double speedKmh, string sKmUnits);
        public GroundSpeedMeasurementHandler GroundSpeedMeashurement;

        public delegate void GPSTextReceivedHandler(string text);
        public GPSTextReceivedHandler GPSTextReceived;

        public delegate void PortErrorHandler(SerialErrorReceivedEventArgs e);
        public PortErrorHandler PortError;

        #endregion
    }
}
