// PresentationPlayLogging
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Luminator.PresentationPlayLogging.Config;
    using Luminator.PresentationPlayLogging.Core.Interfaces;
    using Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking;
    using Luminator.Utility.CsvFileHelper;
    using NLog;

    /// <inheritdoc />
    /// <typeparam name="T">Class entity to log to csv</typeparam>
    /// <summary>The presentation logging.</summary>
    public abstract class PresentationPlayCsvLogging<T> : IPresentationPlayCsvLogging<T>
        where T : class, IPresentationInfo
    {
        /// <summary>The default presentation play log file name.</summary>
        public const string DefaultPresentationCsvPlayLogFileName = @"D:\PresentationPlayLogs\PresentationPlayCsvLogging.csv";

        /// <summary>Dictionary xml file used to process Ximple messages</summary>
        private const string DictionaryFileName = "dictionary.xml";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ILoggingManager loggingManager;

        private bool disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PresentationPlayCsvLogging{T}" /> class. Initializes a new instance
        ///     of the <see cref="PresentationPlayLogging" /> class.
        /// </summary>
        protected PresentationPlayCsvLogging(ILoggingManager loggingManager)
        {
            this.loggingManager = loggingManager;
            this.InitConfig(null);
            this.Dictionary = ReadDictionaryFile();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PresentationPlayCsvLogging{T}" /> class. Initializes a new instance
        ///     of the <see cref="PresentationPlayLogging" /> class.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="dictionary">The dictionary or null to read it in.</param>
        protected PresentationPlayCsvLogging(PresentationPlayLoggingConfig config, ILoggingManager loggingManager, Dictionary dictionary = null)
        {
            this.loggingManager = loggingManager;
            this.InitConfig(config);
            this.Dictionary = dictionary ?? ReadDictionaryFile();
        }

        /// <summary>Initializes a new instance of the <see cref="PresentationPlayCsvLogging{T}" /> class.</summary>
        /// <param name="configFileName">The config file name.</param>
        protected PresentationPlayCsvLogging(string configFileName, ILoggingManager loggingManager)
        {
            this.loggingManager = loggingManager;
            this.InitConfig(PresentationPlayLoggingConfig.ReadConfig(configFileName));
            this.Dictionary = ReadDictionaryFile();
        }

        /// <summary>The feedback message event when a medi message is received.</summary>
        public event EventHandler<MessageEventArgs<UnitsFeedBackMessage<ScreenChanges>>> OnFeedbackMessageReceived;

        /// <summary>The on presentation play data received.</summary>
        public event EventHandler<IPresentationInfo> OnMediPresentationPlayDataReceived;

        public event EventHandler<string> OnNewLogFileCreated;

        /// <summary>The on vehicle position changed.</summary>
        public event EventHandler<VehiclePositionMessage> OnVehiclePositionChanged;

        public event EventHandler<VideoPlaybackEvent> OnVideoPlaybackEvent;

        public event EventHandler<VehicleUnitInfo> OnUnitInfoChanged;

        /// <summary>The on Ximple message received.</summary>
        public event EventHandler<Ximple> OnXimpleMessageReceived;

        /// <summary>Gets the Presentation Logging Configuration.</summary>
        public PresentationPlayLoggingConfig Config { get; protected set; }

        /// <summary>Gets a value indicating whether medi is initialized.</summary>
        public bool IsMediInitialized { get; private set; }

        /// <summary>Gets a value indicating whether the logging is started.</summary>
        public bool IsStarted
        {
            get
            {
                return this.loggingManager.IsStarted;
            }
        }

        /// <summary>Gets or sets the vehicle position message.</summary>
        public VehiclePositionMessage LastVehiclePositionMessage { get; set; } = new VehiclePositionMessage();

        public ILoggingManager LoggingManager
        {
            get
            {
                return this.loggingManager;
            }
        }

        /// <summary>Gets or sets the dictionary component used for Ximple exchange.</summary>
        protected Dictionary Dictionary { get; set; }

        /// <summary>Helper method to read all the csv file records into collection.</summary>
        /// <param name="fileName">The Csv file name.</param>
        /// <returns>The <see cref="List{T}" />List of Records.</returns>
        public static List<T> ReadAll(string fileName)
        {
            using (var csvFileHelper = new CsvFileHelper<T>(fileName))
            {
                return csvFileHelper.ReadAll();
            }
        }

        /// <summary>The read dictionary file.</summary>
        /// <param name="dictionaryFileName">The dictionary file name.</param>
        /// <returns>The <see cref="Dictionary" />.</returns>
        public static Dictionary ReadDictionaryFile(string dictionaryFileName = DictionaryFileName)
        {
            try
            {
                var dictionaryFile = PathManager.Instance.GetPath(FileType.Config, dictionaryFileName);
                if (string.IsNullOrEmpty(dictionaryFile))
                {
                    dictionaryFile = DictionaryFileName;
                    if (File.Exists(dictionaryFile) == false)
                    {
                        if (File.Exists(Path.Combine(@"D:\Config\Protran", DictionaryFileName)))
                        {
                            dictionaryFile = @"D:\Config\Protran\dictionary.xml";
                            Logger.Warn("Cannot find dictionary.xml with binaries in current application folder. Defaulting to use {0}", dictionaryFile);
                        }
                    }
                }

                var cfg = new ConfigManager<Dictionary> { FileName = dictionaryFile };
                return cfg.Config;
            }
            catch (FileNotFoundException fileNotFound)
            {
                Logger.Error("Dictionary.xml file missing {0}", fileNotFound.Message);
                throw;
            }
        }

        /// <summary>The create feedback screen change message.</summary>
        /// <param name="screenChanges">The screen Changes.</param>
        /// <param name="unitName">The unit name.</param>
        /// <returns>The <see cref="object" />UnitsFeedBackMessage</returns>
        public UnitsFeedBackMessage<ScreenChanges> CreateFeedbackScreenChangeMessage(ScreenChanges screenChanges, string unitName)
        {
            var feedbackMessage = new UnitsFeedBackMessage<ScreenChanges>(screenChanges, unitName);
            return feedbackMessage;
        }

        /// <summary>The create feedback screen change message.</summary>
        /// <param name="ScreenChanges">The screen change.</param>
        /// <returns>The <see cref="UnitsFeedBackMessage{T}" />FeedbackMessage</returns>
        public UnitsFeedBackMessage<ScreenChanges> CreateFeedbackScreenChangesMessage(ScreenChanges ScreenChanges)
        {
            return new UnitsFeedBackMessage<ScreenChanges>(ScreenChanges, Environment.MachineName);
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>The dispose.</summary>
        /// <param name="disposing">The disposing.</param>
        public virtual void Dispose(bool disposing)
        {
            Info(nameof(PresentationPlayLogging) + ".Dispose");
            if (!this.disposed)
            {
                this.disposed = true;

                if (this.IsMediInitialized)
                {
                    Info("{0} UnSubscribed Medi Messages", nameof(PresentationPlayLogging));
                    this.UnInitMedi();
                    this.IsMediInitialized = false;
                }

                this.Stop();
                this.loggingManager.Dispose();
            }
        }

        /// <summary>
        ///     Initialize the Configuration
        /// </summary>
        /// <param name="config">Config or null for defaults</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <see>PresentationPlayLoggingConfig</see>
        public void InitConfig(PresentationPlayLoggingConfig config)
        {
            if (config == null)
            {
                // read in our configuration from xml file
                var configFile = PresentationPlayLoggingConfig.ConfigFileName;
                if (!File.Exists(configFile))
                {
                    configFile = PathManager.Instance.GetPath(FileType.Config, PresentationPlayLoggingConfig.ConfigFileName);
                    if (string.IsNullOrEmpty(configFile))
                    {
                        throw new FileNotFoundException("Failed to find Config file " + configFile);
                    }
                }

                // use class map type to customize if provided
                this.Config = PresentationPlayLoggingConfig.ReadConfig(configFile);
            }
            else
            {
                this.Config = config;
            }
        }

        /// <summary>Test function to simulate a medi feedback message boradcast.</summary>
        /// <param name="screenChanges">The message.</param>
        public void SimulateMediFeedbackMessage(ScreenChanges screenChanges)
        {
            var feedBackMessage = new UnitsFeedBackMessage<ScreenChanges>(screenChanges, Environment.MachineName);
            MessageDispatcher.Instance.Broadcast(feedBackMessage);
        }

        /// <summary>Start Logging.</summary>
        /// <exception cref="DirectoryNotFoundException">Failed to create target folder</exception>
        public void Start()
        {
            var mediFileName = PathManager.Instance.GetPath(FileType.Config, "medi.config");
            this.loggingManager.OnFileMoved += this.CsvPlayLogOnFileMoved;
            var clientConfig = this.Config.ClientConfig;

            string logFileName = clientConfig.LogFilePath;
            if (string.IsNullOrEmpty(logFileName))
            {
                logFileName = DefaultPresentationCsvPlayLogFileName;
            }

            this.loggingManager.Start(logFileName, clientConfig.RollOverLogOutputFolder, clientConfig.FileNameRolloverType, clientConfig.MaxFileSize, clientConfig.MaxRecords);
            this.InitMedi(mediFileName);
        }

        /// <summary>Stop Logging.</summary>
        public void Stop()
        {
            this.UnInitMedi();
            this.loggingManager.Stop();
        }

        /// <summary>The info.</summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected static void Info(string format, params object[] args)
        {
            Logger.Info(format, args);
            Debug.WriteLine(format, args);
        }

        /// <summary>The warn.</summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected static void Warn(string format, params object[] args)
        {
            Logger.Warn(format, args);
            Debug.WriteLine(format, args);
        }

        /// <summary>Initialize Medi message subscriptions</summary>
        /// <param name="mediFileName">Medi config file name or null to default to 'medi.config'</param>
        /// <exception cref="FileNotFoundException">File not found</exception>
        protected virtual void InitMedi(string mediFileName)
        {
            // Subscribe to the Medi message we want
            if (string.IsNullOrEmpty(mediFileName))
            {
                mediFileName = "medi.config";
            }

            if (this.IsMediInitialized)
            {
                return;
            }

            //string t = Path.GetDirectoryName(".");
            //var t1 = Environment.CurrentDirectory;

            if (File.Exists(mediFileName))
            {
                try
                {
                    // use the same mechanics as the host apps so we can run them standalone and debug messages to/from them                        
                    var fileConfigurator = new FileConfigurator(mediFileName, Environment.MachineName, Environment.MachineName);

                    if (MessageDispatcher.Instance.IsValidLocalAddress == false)
                    {
                        MessageDispatcher.Instance.Configure(fileConfigurator);
                    }

                    this.SubscribeToMediMessages();

                    this.IsMediInitialized = true;
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to Init Medi {0}", ex.Message);
                }
            }
            else
            {
                throw new FileNotFoundException("Medi.config file not found, messaging for Presentation Play Logging not supported!", mediFileName);
            }
        }

        /// <summary>
        /// Notify subscribers that a feedback message has been received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="messageEventArgs">
        /// The message event args.
        /// </param>
        protected void RaiseFeedbackMessageReceived(object sender, MessageEventArgs<UnitsFeedBackMessage<ScreenChanges>> messageEventArgs)
        {
            this.OnFeedbackMessageReceived?.Invoke(sender, messageEventArgs);
        }

        /// <summary>Raise the event when VehicleUnitInfo has changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="vehicleUnitInfo">The vehicle unit info.</param>
        protected void RaiseOnUnitInfoChanged(object sender, VehicleUnitInfo vehicleUnitInfo)
        {
            this.OnUnitInfoChanged?.Invoke(sender, vehicleUnitInfo);
        }

        /// <summary>
        /// The raise on medi presentation play data received.
        /// </summary>
        /// <param name="presentationInfo">
        /// The presentation info.
        /// </param>
        protected void RaiseOnMediPresentationPlayDataReceived(T presentationInfo)
        {
            this.OnMediPresentationPlayDataReceived?.Invoke(this, presentationInfo);
        }

        /// <summary>
        /// The raise vehicle position changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="vehiclePositionMessage">
        /// The vehicle position message.
        /// </param>
        protected void RaiseVehiclePositionChanged(object sender, VehiclePositionMessage vehiclePositionMessage)
        {
            this.OnVehiclePositionChanged?.Invoke(sender, vehiclePositionMessage);
        }

        /// <summary>
        /// Notify listeners that a Ximple message was received.
        /// </summary>
        /// <param name="ximple">
        /// The ximple.
        /// </param>
        protected void RaiseXimpleMessageReceived(Ximple ximple)
        {
            if (ximple != null)
            {
                this.OnXimpleMessageReceived?.Invoke(this, ximple);
            }
        }

        /// <summary>
        /// Subscribe to the specific medi message type
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <typeparam name="T1">The message type </typeparam>
        /// <exception cref="ArgumentNullException">Thrown if the message handles in not given. </exception>
        protected void SubscribeFeedBackMessages<T1>(EventHandler<MessageEventArgs<T1>> handler)
            where T1 : class
        {
            Info($"Enter Subscribe to Medi message for {nameof(this.SubscribeFeedBackMessages)}, type {typeof(T1)}");
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "SubscribeScreenChanges received null handler object!");
            }

            // subscribe to medi feedback messages
            MessageDispatcher.Instance.Subscribe(handler);
        }

        /// <summary>
        /// Handle Medi message subscription.
        /// </summary>
        protected abstract void SubscribeToMediMessages();

        /// <summary>
        /// Unsubscribe from listening to a medi message
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <typeparam name="T1">The message type </typeparam>
        protected void UnsubscribeFromMediMessage<T1>(EventHandler<MessageEventArgs<T1>> handler)
            where T1 : class
        {
            MessageDispatcher.Instance.Unsubscribe(handler);
        }

        /// <summary>
        /// Derived classes should call message un subscription.
        /// </summary>
        protected abstract void UnsubscribeFromMediMessages();

        private void CsvPlayLogOnFileMoved(object sender, string fileName)
        {
            Logger.Info("New CsvHelper file created {0}", fileName);
            this.RaiseOnNewLogFileCreated(fileName);
        }

        private void MoveLogFile(string fileName, string destinationFolder)
        {
            if (string.IsNullOrEmpty(destinationFolder))
            {
                return;
            }

            if (Directory.Exists(destinationFolder) == false)
            {
                throw new DirectoryNotFoundException(destinationFolder);
            }

            try
            {
                if (File.Exists(fileName) == false)
                {
                    return;
                }

                var destFile = Path.Combine(destinationFolder, Path.GetFileName(fileName));
                if (File.Exists(destFile) == false)
                {
                    var fileNameOnly = Path.GetFileNameWithoutExtension(destFile);
                    var ext = Path.GetExtension(destFile);
                    var path = destinationFolder;
                    if (string.IsNullOrEmpty(path))
                    {
                        path = string.Empty;
                    }

                    var count = 0;
                    string newDestinationFile;

                    do
                    {
                        newDestinationFile = Path.Combine(path, $"{fileNameOnly}-{++count}{ext}");
                    }
                    while (File.Exists(newDestinationFile));
                    destFile = newDestinationFile;
                }

                Logger.Info("Moving new CsvHelper Log file {0} => {1}", fileName, destFile);
                File.Move(fileName, destFile);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to move file {0} to {1} {2}", fileName, destinationFolder, ex.Message);
            }
        }

        private void RaiseOnNewLogFileCreated(string fileName)
        {
            this.OnNewLogFileCreated?.Invoke(this, fileName);
        }

        private void RaiseVideoPlaybackEvent(VideoPlaybackEvent videoPlaybackEvent)
        {
            if (videoPlaybackEvent != null)
            {
                this.OnVideoPlaybackEvent?.Invoke(this, videoPlaybackEvent);
            }
        }

        private void SubscribeFeedBackMessages(EventHandler<MessageEventArgs<UnitsFeedBackMessage<ScreenChanges>>> handler)
        {
            Info("Enter Subscribe to Medi message for {0}", nameof(this.SubscribeFeedBackMessages));
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "SubscribeScreenChanges received null handler object!");
            }

            // subscribe to medi feedback messages
            MessageDispatcher.Instance.Subscribe(handler);
        }

        private void UnInitMedi()
        {
            // UnsSubscribe to the Medi messages
            try
            {
                Info("Presentation lay Log UnInitMedi Enter");
                this.UnsubscribeFromMediMessages();
            }
            catch (Exception ex)
            {
                Warn("UnInitMedi Exception {0}", ex.Message);
            }
            finally
            {
                this.IsMediInitialized = false;
            }
        }
    }
}