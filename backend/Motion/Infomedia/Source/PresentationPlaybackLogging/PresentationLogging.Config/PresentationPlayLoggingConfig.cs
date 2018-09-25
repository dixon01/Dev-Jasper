// PresentationPlayLogging
// PresentationPlayLogging.Config
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.Config
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using Luminator.PresentationPlayLogging.Config.Interfaces;
    using Luminator.Utility.CsvFileHelper;

    using NLog;

    /// <summary>The presentation play logging config.</summary>
    [Serializable]
    public class PresentationPlayLoggingConfig : IPresentationPlayLoggingConfig
    {
        /// <summary>The presentation logging config file name.</summary>
        public const string ConfigFileName = "PresentationPlayLoggingConfig.xml";

        /// <summary>The default output folder for the tft units.</summary>
        public const string DefaultLoggingOutputFolder = @"D:\PresentationPlayLogs";

        public const string DefaultUploadsFolder = @"D:\Data\Update\Uploads";

        private const string DefaultDebugConnectionString =
                @"Data Source=.\SQLEXPRESS;Initial Catalog=Luminator.Reporting.PresentationPlayLogging;Integrated Security=true;"
            ;

        private const string DefaultReleaseConnectionString =
                @"Data Source=.\SQLEXPRESS;Initial Catalog=Luminator.Reporting.PresentationPlayLogging;Integrated Security=true;"
            ;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>Initializes a new instance of the <see cref="PresentationPlayLoggingConfig" /> class.</summary>
        public PresentationPlayLoggingConfig()
        {
            this.ClientConfig =
                new PresentationPlayLoggingClientConfig
                    {
                        FileNameRolloverType = FileNameRolloverType.Numerical,
                        MaxFileSize = 0,
                        MaxRecords = 5000,
                        RollOverLogOutputFolder = DefaultLoggingOutputFolder
                    };
            this.ServerConfig =
                new PresentationPlayLoggingServerConfig
                    {
                        BadFileFolder = string.Empty,
                        ConnectionStringDebug = DefaultDebugConnectionString,
                        ConnectionStringRelease = DefaultReleaseConnectionString,
                        FileWatchFolder = @"D:\ftproot",
                        WatchFileFilter = "*.csv",
                        DestinationsUnitUri = String.Empty,
                        DestinationsConnectionString = String.Empty
                };
        }

        [XmlElement("ClientConfig")]
        public PresentationPlayLoggingClientConfig ClientConfig { get; set; }

        [XmlElement("ServerConfig")]
        public PresentationPlayLoggingServerConfig ServerConfig { get; set; }

        /// <summary>The read presentation logging config.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="PresentationPlayLoggingConfig" />.</returns>
        public static PresentationPlayLoggingConfig ReadConfig(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new FileNotFoundException("File Name is missing unable to read in Presentation Play Log!");
            }

            if (Path.GetExtension(fileName).ToLower() == ".csv")
            {
                throw new FileNotFoundException("File Name is CSV file type not config, check file name!");
            }

            var serializer = new XmlSerializer(typeof(PresentationPlayLoggingConfig));
            using (var stream = File.OpenRead(fileName))
            {
                return (PresentationPlayLoggingConfig)serializer.Deserialize(stream);
            }
        }

        /// <summary>The write presentation logging config.</summary>
        /// <param name="config">The config.</param>
        /// <param name="fileName">The file name.</param>
        public static void WriteConfig(IPresentationPlayLoggingConfig config, string fileName = ConfigFileName)
        {
            var serializer = new XmlSerializer(typeof(PresentationPlayLoggingConfig));
            using (var stream = File.Create(fileName))
            {
                serializer.Serialize(stream, config);
            }
        }
    }
}