namespace Luminator.PresentationPlayLogging.Core
{
    using System.IO;

    using Gorba.Common.SystemManagement.Host.Path;

    using Luminator.PresentationPlayLogging.Config;
    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking;

    public static class PresentationPlayLoggingFactory
    {
        public static PresentationPlayLoggingConfig ReadConfig(
            string configFileName = PresentationPlayLoggingConfig.ConfigFileName)
        {
            if (File.Exists(configFileName) == false)
            {
                configFileName = PathManager.Instance.GetPath(
                    FileType.Config,
                    PresentationPlayLoggingConfig.ConfigFileName);
            }

            return PresentationPlayLoggingConfig.ReadConfig(configFileName);
        }

        public static PresentationInfotransitCsvLogging CreatePresentationInfotransitCsvLogging(
            string configFileName = PresentationPlayLoggingConfig.ConfigFileName)
        {
            return new PresentationInfotransitCsvLogging(ReadConfig(configFileName),
                new ProofOfPlayLoggingManager<InfotransitPresentationInfo>(new CSVLogger<InfotransitPresentationInfo>()));
        }

        public static bool IsPresentationPlayLogConfigFound(
            string configFileName = PresentationPlayLoggingConfig.ConfigFileName)
        {
            if (File.Exists(configFileName))
            {
                return true;
            }

            var presentationLogConfig = PathManager.Instance.GetPath(FileType.Config, configFileName);
            return File.Exists(presentationLogConfig);
        }
    }
}