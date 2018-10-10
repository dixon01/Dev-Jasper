namespace Luminator.PresentationPlayLogging.Config
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Luminator.Utility.CsvFileHelper;

    [Serializable]
    public class PresentationPlayLoggingClientConfig
    {
        public PresentationPlayLoggingClientConfig()
        {
            this.RollOverLogOutputFolder = string.Empty;
        }

        /// <summary>Gets or sets the file rollover type to use or None.</summary>
        public FileNameRolloverType FileNameRolloverType { get; set; } = FileNameRolloverType.None;

        /// <summary>Gets or sets the max file size for the output file.</summary>
        public long MaxFileSize { get; set; }

        /// <summary>Gets or sets the max records for the output file.</summary>
        public long MaxRecords { get; set; }

        public int MaxZipFileEntries { get; set; }
        
        /// <summary>Gets or sets the roll over file output folder used when File Rollover is selected.</summary>
        public string RollOverLogOutputFolder { get; set; }

        // Gets or sets the flag tp zip the csv output
        public bool ZipOutput { get; set; }

        /// <summary>
        /// The full path to the log file that will be created.
        /// </summary>
        public string LogFilePath { get; set; }
    }
}