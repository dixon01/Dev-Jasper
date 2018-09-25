// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerPeripheralSerialClient.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Models;
    using Luminator.PeripheralDimmer.Types;

    using NLog;

    /// <summary>The dimmer peripheral serial client.</summary>
    public class DimmerPeripheralSerialClient : PeripheralSerialClient, IPeripheralDimmerSerialClient
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<DimmerMessageType, Type> messageTypesDictionary = new Dictionary<DimmerMessageType, Type>();

        #endregion

        #region Fields

        /// <summary>The dimmer query response event.</summary>
        protected AutoResetEvent DimmerQueryResponseEvent = new AutoResetEvent(false);

        /// <summary>The dimmer version response event.</summary>
        protected AutoResetEvent DimmerVersionResponseEvent = new AutoResetEvent(false);

        private readonly object timerLock = new object();

        private Timer backgroundRequestTimer;

        private VersionInfo dimmerVersionInfo;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes static members of the <see cref="DimmerPeripheralSerialClient" /> class.</summary>
        static DimmerPeripheralSerialClient()
        {
            messageTypesDictionary.Add(DimmerMessageType.Nak, typeof(DimmerNak));
            messageTypesDictionary.Add(DimmerMessageType.Ack, typeof(DimmerAck));
            messageTypesDictionary.Add(DimmerMessageType.QueryResponse, typeof(DimmerQueryResponse));
            messageTypesDictionary.Add(DimmerMessageType.VersionResponse, typeof(DimmerVersionResponse));
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerPeripheralSerialClient" /> class.</summary>
        public DimmerPeripheralSerialClient()
        {
            this.CommonConstruct();
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerPeripheralSerialClient"/> class.</summary>
        /// <param name="serialPortSettings">The serial port settings.</param>
        public DimmerPeripheralSerialClient(ISerialPortSettings serialPortSettings)
            : base(serialPortSettings)
        {
            this.CommonConstruct();
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerPeripheralSerialClient"/> class.</summary>
        /// <param name="serialPortSettingsFileName">The serial port settings file name.</param>
        public DimmerPeripheralSerialClient(string serialPortSettingsFileName)
            : base(serialPortSettingsFileName)
        {
            this.CommonConstruct();
        }

        #endregion

        #region Public Events

        /// <summary>The dimmer sensor levels changed.</summary>
        public event EventHandler<DimmerQueryResponse> DimmerSensorLevelsChanged;

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is connected.</summary>
        public virtual bool IsConnected
        {
            get
            {
                return this.IsOpen && this.VersionInfo != null;
            }
        }

        /// <summary>Gets or sets a value indicating whether timer proc enabled.</summary>
        public bool TimerProcEnabled { get; set; }

        /// <summary>Gets the version info.</summary>
        public VersionInfo VersionInfo
        {
            get
            {
                lock (typeof(VersionInfo))
                {
#if DEBUG
                    var stackTrace = new StackTrace();
                    Debug.WriteLine("Dimmer Version Get from " + stackTrace.GetFrame(1).GetMethod().Name + "()");
#endif
                    return dimmerVersionInfo;
                }
            }

            set
            {
                lock (typeof(VersionInfo))
                {
#if DEBUG
                    var stackTrace = new StackTrace();
                    Debug.WriteLine("Dimmer Version Set from " + stackTrace.GetFrame(1).GetMethod().Name + "()");
#endif
                    this.dimmerVersionInfo = value;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The find message type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="Type"/>.</returns>
        public static Type FindMessageType(DimmerMessageType type)
        {
            return messageTypesDictionary.ContainsKey(type) ? messageTypesDictionary[type] : null;
        }

        /// <summary>The close.</summary>
        public override void Close()
        {
            StopBackgroundProcessing();
            base.Close();
        }

        /// <summary>The read.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="networkByteOrderToHost">The network byte order to host.</param>
        /// <returns>The <see cref="object"/>.</returns>
        /// <exception cref="MissingMethodException">Default Constructor missing.</exception>
        public override IPeripheralBaseMessage Read(byte[] bytes, bool networkByteOrderToHost = true)
        {
            try
            {
                var header = new PeripheralHeader(bytes, networkByteOrderToHost);
                switch ((DimmerMessageType)header.MessageType)
                {
                    default:
                        return null;
                    case DimmerMessageType.QueryResponse:
                        var message = this.Read<DimmerQueryResponse>(bytes, networkByteOrderToHost);
                        if (message != null)
                        {
                            this.DimmerQueryResponseEvent.Set();
                            return message;
                        }

                        break;
                    case DimmerMessageType.VersionResponse:
                        return this.Read<DimmerVersionResponse>(bytes, networkByteOrderToHost);
                    case DimmerMessageType.Ack:
                        return this.Read<DimmerAck>(bytes, networkByteOrderToHost);
                    case DimmerMessageType.Nak:
                        return this.Read<DimmerNak>(bytes, networkByteOrderToHost);
                }
            }
            catch (NotSupportedException notSupportedException)
            {
                Logger.Error(notSupportedException.Message);
            }

            return null;
        }

        /// <summary>The start background processing.</summary>
        /// <param name="msInterval">The ms interval.</param>
        public virtual void StartBackgroundProcessing(int msInterval)
        {
            Logger.Info("StartBackgroundProcessing({0})", msInterval);
            this.StopBackgroundProcessing();
            this.backgroundRequestTimer = new Timer(this.TimerProc);
            this.backgroundRequestTimer.Change(0, msInterval);
        }

        /// <summary>The stop background processing.</summary>
        public virtual void StopBackgroundProcessing()
        {
            if (this.backgroundRequestTimer != null)
            {
                Logger.Info("StopBackgroundProcessing()");
                this.backgroundRequestTimer.Dispose();
                this.backgroundRequestTimer = null;
            }
        }

        /// <summary>The write brightness.</summary>
        /// <param name="brightnessLevel">The brightness level.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool WriteBrightness(byte brightnessLevel)
        {
            Logger.Info("WriteBrightness = {0}", brightnessLevel);
            return this.Write(new DimmerSetBrightness(brightnessLevel), true) > 0;
        }

        /// <summary>Write the collection of brightness levels setting the Scale and Brightness values.</summary>
        /// <param name="brightnessLevels">The brightness levels.</param>
        /// <param name="msWriteDelay">The ms Write Delay.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool WriteBrightnessLevels(DimmerBrightnessLevels brightnessLevels, int msWriteDelay = DimmerConstants.WriteDelayDefault)
        {
            if (brightnessLevels == null || brightnessLevels.IsBrightnessValid == false)
            {
                Logger.Debug("WriteBrightnessLevels Ignored, incomplete inputs");
                return false;
            }

            // Set the Scale when supplied and valid (0-3)
            if (brightnessLevels.IsLightSensorScaleValid)
            {
                WriteSensorScale(brightnessLevels.LightSensorScale);
            }

            // 0 - 255
            foreach (var b in brightnessLevels.Brightness)
            {
                WriteBrightness(b);
                if (msWriteDelay > 0)
                {
                    Thread.Sleep(msWriteDelay);
                }
            }
            return true;
        }

        /// <summary>The write power on mode.</summary>
        /// <param name="powerOnMode">The power on mode.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool WritePowerOnMode(PowerOnMode powerOnMode)
        {
            return this.Write(new DimmerSetPowerOnMode(powerOnMode), true) > 0;
        }

        /// <summary>Write query request to get the sensor levels. Wait for response</summary>
        /// <param name="msTimeout">The ms Timeout.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool WriteQueryRequest(int msTimeout = 1000)
        {
            if (this.Write(new DimmerQueryRequest()) > 0)
            {
                if (msTimeout > 0)
                {
                    var responseEvent = this.DimmerQueryResponseEvent.WaitOne(msTimeout);
                    if (!responseEvent)
                    {
                        return false;
                    }

                    Debug.WriteLine("Received QuerResponse for dimmer levels");
                }

                return true;
            }

            return false;
        }

        /// <summary>The write sensor scale.</summary>
        /// <param name="scale">The scale.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool WriteSensorScale(byte scale)
        {
            Logger.Info("WriteSensorScale = {0}", scale);
            return this.Write(new DimmerSetSensorScale(scale), true) > 0;
        }
        
        /// <summary>The write poll request.</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool WritePollRequest()
        {
            return this.Write(new DimmerPoll(), true) > 0;
        }

        /// <summary>The write version request.</summary>
        /// <returns>The <see cref="DimmerVersionInfo" />.</returns>
        public VersionInfo WriteVersionRequest()
        {
            if (this.IsConnected)
            {
                Debug.WriteLine("Dimmer Peripheral Connected Version info " + this.VersionInfo.ToString());
                return this.VersionInfo;
            }

            VersionInfo versionInfo = null;
            if (this.Write(new DimmerVersionRequest()) > 0)
            {
                Debug.WriteLine("Waiting for Version response");
                var signaled = DimmerVersionResponseEvent.WaitOne(2000);
                if (signaled && this.VersionInfo != null)
                {
                    versionInfo = this.VersionInfo;
                    Logger.Info("Version requested result = {0}, versionInfo={1}", signaled, versionInfo);
                }
                else
                {
                    Logger.Warn("Not Signaled for Rx of Peripheral Version info");
                }
            }
            else
            {
                Logger.Error("WriteVersionRequest Failed ");
            }

            return versionInfo;
        }

        #endregion

        #region Methods

        /// <summary>The process new peripheral message.</summary>
        /// <param name="peripheralBaseMessage">The peripheral base message.</param>
        /// <param name="bytesRead">The bytes read.</param>
        protected override void ProcessNewPreipheralMessage(IPeripheralBaseMessage peripheralBaseMessage, byte[] bytesRead)
        {
            if (this.VersionInfo == null && peripheralBaseMessage is DimmerVersionResponse)
            {
                var v = ((DimmerVersionResponse)peripheralBaseMessage).VersionInfo;
                this.VersionInfo = new VersionInfo(v.HardwareVersionText, v.SoftwareVersionText);
                this.DimmerVersionResponseEvent.Set();
            }

            if (peripheralBaseMessage is DimmerAck)
            {
                Logger.Warn("*** Ack response received ***");                
                this.AckReceivedEvent.Set();
            }

            base.ProcessNewPreipheralMessage(peripheralBaseMessage, bytesRead);
        }

        private void CommonConstruct()
        {
            this.TimerProcEnabled = true;
            this.PeripheralDataReady += (sender, arg) =>
                {
                    if (arg?.Message is DimmerQueryResponse)
                    {
                        var message = (DimmerQueryResponse)arg.Message;
                        var handler = this.DimmerSensorLevelsChanged;
                        handler?.Invoke(this, message);
                    }
                };
        }

        private void DoTimerRequest()
        {
            // Request dimmer sensor values then process this response
            if (this.IsOpen)
            {
                if (this.TimerProcEnabled)
                {
                    Logger.Debug("DoTimerRequest Enter");
                    Debug.WriteLine("DoTimerRequest Enter");
                    this.WriteQueryRequest();
                }
                else
                {
                    Logger.Warn("Dimmer DoTimerRequest() ignored, disabled");
                }
            }
        }

        private void TimerProc(object state)
        {
            Debug.WriteLine("TimerProc Enter");

            if (Monitor.TryEnter(timerLock, 10))
            {
                DoTimerRequest();
                Monitor.Exit(timerLock);
            }
            else
            {
                Debug.WriteLine("TimerPoc ignored unable to acquire Lock");
            }
        }

        #endregion
    }
}