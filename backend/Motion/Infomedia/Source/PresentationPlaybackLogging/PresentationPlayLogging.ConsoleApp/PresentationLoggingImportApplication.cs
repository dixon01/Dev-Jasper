
namespace Luminator.PresentaionPlayLogging.ConsoleApp
{
    using System.IO;
    using System.Threading;

    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;

    using Luminator.PresentationPlayLogging.Config;
    using Luminator.PresentationPlayLogging.ImportService;

    /// <summary>
    /// The dimmer application.
    /// </summary>
    public class PresentationLoggingImportApplication : ApplicationBase
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private PresentationPlayLoggingImpl presentationPlayLoggingImpl;

        #region Default Constructors

        /// <summary>
        /// Default constructor.  Initializes a new instance of the <see cref="PresentationLoggingImportApplication"/> class.
        /// </summary>
        public PresentationLoggingImportApplication()
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Methods

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// </summary>
        protected override void DoRun()
        {
            // get full path to our config file, read that in
            var configFileName = PresentationPlayLoggingConfig.ConfigFileName;
            var configFileFullPath = PathManager.Instance.GetPath(FileType.Config, configFileName);

            this.Logger.Info($"Loading Config: {configFileFullPath}");
            if (File.Exists(configFileFullPath) == false)
            {
                configFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);
            }

            var config = PresentationPlayLoggingConfig.ReadConfig(configFileFullPath);
            this.presentationPlayLoggingImpl = new PresentationPlayLoggingImpl(config);
            this.presentationPlayLoggingImpl.Start();
            this.SetRunning();
            this.runWait.WaitOne();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="DoRun"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.runWait.Set();
        }

        #endregion
    }
}
