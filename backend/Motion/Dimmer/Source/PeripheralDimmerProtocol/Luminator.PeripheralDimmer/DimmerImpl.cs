// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerImpl.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer
{
    using System;

    using Luminator.PeripheralDimmer.Models;
    using Luminator.PeripheralDimmer.Processor;

    using NLog;

    /// <summary>
    ///     Main class of this library
    /// </summary>
    public class DimmerImpl : IDisposable
    {
        #region Static Fields

        /// <summary>
        ///     The management name.
        /// </summary>
        internal static readonly string ManagementName = "Dimmer";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        private readonly DimmerProcessor dimmerProcessor;

        private DimmerPeripheralSerialClient client;

        private DimmerPeripheralConfig dimmerPeripheralConfig;

        #endregion

        #region Properties

        private byte MaximumPercentage
        {
            get
            {
                return this.dimmerPeripheralConfig.MaximumPercentage;
            }
        }

        private byte MinimumPercentage
        {
            get
            {
                return this.dimmerPeripheralConfig.MinimumPercentage;
            }
        }

        public DimmerPeripheralSerialClient Client
        {
            get { return client; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Implements IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        ///     Starts this Dimmer
        /// </summary>
        public void Start()
        {
            // Start the background timer for requesting dimmers sensor levels and updating the new values
            try
            {
                Logger.Info("Construct DimmerImpl");
                if (this.Config == null)
                {
                    throw new ArgumentNullException("Invalid DimmerPeripheralConfig");
                }

                this.dimmerPeripheralConfig = this.Config;
                var serialPortSettings = this.dimmerPeripheralConfig.SerialPortSettings;
                var environmentVarComPort = DimmerConstants.DefaultComPort;
                if (string.IsNullOrEmpty(environmentVarComPort) == false)
                {
                    serialPortSettings.ComPort = environmentVarComPort;
                }

                this.client = new DimmerPeripheralSerialClient(serialPortSettings);
                this.client.DimmerSensorLevelsChanged += this.ClientOnDimmerSensorLevelsChanged;

                var version = this.client.WriteVersionRequest();
                if (version != null)
                {
                    int requestTimeout = this.dimmerPeripheralConfig.DimmerSensorRequestInterval;
                    if (requestTimeout <= 500)
                    {
                        requestTimeout = 1000;
                    }

                    this.client.StartBackgroundProcessing(requestTimeout);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Start failed {0}", ex.Message);
            }
        }

        /// <summary>The stop.</summary>
        public void Stop()
        {
            if (this.client != null)
            {
                this.client.DimmerSensorLevelsChanged -= this.ClientOnDimmerSensorLevelsChanged;
                this.client.Dispose();
                this.client = null;
            }
        }

        #endregion

        #region Methods

        private void ClientOnDimmerSensorLevelsChanged(object sender, DimmerQueryResponse dimmerQueryResponse)
        {
            Logger.Debug("Dimmer Sensor levels changed");
            this.client.TimerProcEnabled = false;

            // Process and set the new values then re-enable the TimerProcEnabled
            this.ProcessSensorLevelChanges(dimmerQueryResponse);
        }

        private void ProcessSensorLevelChanges(DimmerQueryResponse dimmerQueryResponse)
        {
            try
            {
                if (dimmerQueryResponse != null)
                {
                    var calculateDimmerOutput = this.dimmerProcessor.CalculateDimmerOutput(
                        this.MinimumPercentage, 
                        this.MaximumPercentage, 
                        dimmerQueryResponse.LightLevel, 
                        dimmerQueryResponse.LightSensorScale, 
                        dimmerQueryResponse.Brightness);

                    if (calculateDimmerOutput != null && calculateDimmerOutput.IsValid)
                    {
                        var dimmerBrightnessLevels = new DimmerBrightnessLevels(
                            calculateDimmerOutput.BrightnessSetting, 
                            calculateDimmerOutput.RangeScale);
                        var intervalWriteDelay = calculateDimmerOutput.IntervalDelay;
                        this.client.WriteBrightnessLevels(dimmerBrightnessLevels, intervalWriteDelay);
                    }
                    else
                    {
                        Logger.Warn("CalculateDimmerOutput Ignored");
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.Error("ProcessSensorLevelChanges input values invalid! {0}", ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.Error("General Exception setting Brightness levels {0}", ex.Message);
            }
            finally
            {
                // resume allowing the timer to request a query for sensor brightness values
                this.client.TimerProcEnabled = true;
            }
        }

        #endregion

        #region Constructors

        private int WriteBrightnessLevelsIntervalDelay
        {
            get
            {
                return this.dimmerPeripheralConfig?.DimmerProcessorTuningParams.IntervalDelay ?? DimmerConstants.WriteDelayDefault;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DimmerImpl" /> class.
        ///     Default constructor.
        /// </summary>
        public DimmerImpl()
        {
            this.dimmerProcessor = new DimmerProcessor();
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerImpl"/> class.</summary>
        /// <param name="config">The config.</param>
        public DimmerImpl(DimmerPeripheralConfig config)
            : this()
        {
            this.Config = config;
            this.dimmerProcessor = new DimmerProcessor(config.DimmerProcessorTuningParams);
        }

        /// <summary>Gets or sets the config.</summary>
        public DimmerPeripheralConfig Config { get; set; }

        #endregion
    }
}