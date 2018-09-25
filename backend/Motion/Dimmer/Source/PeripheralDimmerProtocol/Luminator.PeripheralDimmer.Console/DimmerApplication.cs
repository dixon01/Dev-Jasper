// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DimmerApplication.cs" company="Luminator Technology Group">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <summary>
//   The dimmer application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralDimmer.Console
{
    using System.IO;
    using System.Threading;

    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;

    using Luminator.PeripheralDimmer;

    /// <summary>
    /// The dimmer application.
    /// </summary>
    public class DimmerApplication : ApplicationBase
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        #region Default Constructors

        /// <summary>
        /// Default constructor.  Initializes a new instance of the <see cref="DimmerApplication"/> class.
        /// </summary>
        public DimmerApplication()
        {
      
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the dimmer main class.
        /// </summary>
        public DimmerImpl DimmerImpl { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// </summary>
        protected override void DoRun()
        {
            // get full path to our config file, read that in
            var configFileName = DimmerPeripheralConfig.DefaultDimmerPeripheralConfigFileName;
            var configFileFullPath = PathManager.Instance.GetPath(FileType.Config, configFileName);
            if (File.Exists(configFileFullPath) == false)
            {
                configFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);
            }

            var config = DimmerPeripheralConfig.ReadDimmerPeripheralConfig(configFileFullPath);
            this.DimmerImpl = new DimmerImpl(config);
            this.DimmerImpl.Start();
            this.SetRunning();
            this.runWait.WaitOne();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="DoRun"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.DimmerImpl.Dispose();
            this.runWait.Set();
        }

        #endregion
    }
}
