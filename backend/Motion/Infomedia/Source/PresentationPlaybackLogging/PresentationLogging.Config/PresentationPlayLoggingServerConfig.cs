namespace Luminator.PresentationPlayLogging.Config
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable]
    public class PresentationPlayLoggingServerConfig
    {
        private const string InfotransitDestinationsConnectionString =
            "Server=(local);Database=Infotransit.Destinations;Trusted_Connection=True;MultipleActiveResultSets=true;";

        public string BadFileFolder { get; set; } = @"D:\BadPresentationPlayLogs";

        /// <summary>Gets the connection string for the appropriate build.</summary>
        [XmlIgnore]
        public string ConnectionString
        {
            get
            {
#if DEBUG
                return this.ConnectionStringDebug;
#else
                return this.ConnectionStringRelease;
#endif
            }
        }

        /// <summary>Gets or sets the connection string for debug.</summary>
        public string ConnectionStringDebug { get; set; }

        /// <summary>Gets or sets the connection string for release.</summary>
        public string ConnectionStringRelease { get; set; }

        public string DestinationsConnectionString { get; set; } = InfotransitDestinationsConnectionString;

        public string DestinationsUnitUri { get; set; } = "http:\\\\";

        public string FileWatchFolder { get; set; }

        /// <summary>Gets or sets the flag to control if Units are looked up and used in the import.</summary>
        public bool LookupUnits { get; set; }

        /// <summary>Gets or sets the output file filter example: (*.csv).</summary>
        public string WatchFileFilter { get; set; } = "*.csv";

        /// <summary>
        /// Gets or sets the polling interval at which files to import are checked for.
        /// </summary>
        [XmlIgnore]
        public TimeSpan PollInterval { get; set; }

        /// <summary>
        /// Gets or sets the polling interval as an XML serializable string.
        /// </summary>
        [XmlElement("PollInterval", DataType = "duration")]
        public string PollIntervalString
        {
            get
            {
                return XmlConvert.ToString(this.PollInterval);
            }

            set
            {
                this.PollInterval = XmlConvert.ToTimeSpan(value);
            }
        }
    }

    [Serializable]
    public class FtpConfig
    {
        public string HostUri { get; set; }
        public string UserName { get;set; }

        public string Password { get; set; }
    }
}