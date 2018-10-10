namespace Luminator.Multicast.Core
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Reflection;

    using Gorba.Common.Utility.Core;

    using NLog;

    public class MulticastConfig
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        #region Constructors and Destructors

        public MulticastConfig()
        {
            //var appConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            //string dllConfigData = appConfig.AppSettings.Settings["FtpUserName"].Value;

            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config";
            if (!File.Exists(map.ExeConfigFilename))
            {
                Logger.Error($"Config file {map.ExeConfigFilename} File not Found - Instead of Closing the Application we pass back an empty config.");
                
                this.FtpPassword = string.Empty;
                this.FtpUsername = string.Empty;
                this.LocalFilePath = string.Empty;
                this.MulticastAddress = IPAddress.None;
                this.MulticastPort = -1;
                this.ProgramStepInterval = -1;
                this.UseWindowsInputSimulator = false;
                this.CreateCompleteStatus = true;
            }
            
            var libConfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            var section = libConfig.GetSection("appSettings") as AppSettingsSection;
            if (section != null)
            {
                this.FtpPassword = section.Settings["FtpPassword"].Value;

                this.FtpUsername = section.Settings["FtpUserName"].Value;

                this.LocalFilePath = ".\\Resources\\Facade.txt";

                this.MulticastAddress = IPAddress.Parse(section.Settings["MulticastIp"].Value);

                this.MulticastPort = int.Parse(section.Settings["MulticastPort"].Value);

                this.ProgramStepInterval = int.Parse(section.Settings["ProgramStepInterval"].Value);

                this.UseWindowsInputSimulator = bool.Parse(section.Settings["UseWindowsInputSimulator"].Value);

                this.CreateCompleteStatus = bool.Parse(section.Settings["CreateCompleteStatus"].Value);

            }
            else
            {
                Logger.Error($"{map.ExeConfigFilename} missing appSettings config section.");
            }
        }

        #endregion

        #region Public Properties

        public string ConnectedFilename { get; set; }

        public string FtpPassword { get; set; }

        public string FtpUsername { get; set; }

        public string LocalFilePath { get; set; }

        public IPAddress MulticastAddress { get; set; }

        public int MulticastPort { get; set; }

        public int ProgramStepInterval { get; set; }

        public bool UseWindowsInputSimulator { get; set; }

        public bool CreateCompleteStatus { get; set; }

        #endregion
    }
}