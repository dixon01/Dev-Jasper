// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core
{
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Common;

    /// <summary>
    /// The IBIS (VDV 300) application.
    /// </summary>
    public class IbisApplication : ApplicationBase
    {
        /// <summary>
        /// The management name used in Medi and System Manager.
        /// </summary>
        public static readonly string ManagementName = "IbisControl";

        private IbisMaster ibisMaster;

        private IbisConfig config;

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// </summary>
        protected override void DoRun()
        {
            TimeProvider.Current = new GpsTimeProvider();

            RemoteEventHandler.Initialize();

            var configMgr = new ConfigManager<IbisConfig>();
            configMgr.FileName = PathManager.Instance.GetPath(FileType.Config, "ibis.xml");
            this.config = configMgr.Config;

            this.ibisMaster = new IbisMaster();
            this.ibisMaster.Configure(this.config);

            this.SetRunning();
            this.ibisMaster.Run();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// </summary>
        protected override void DoStop()
        {
            this.ibisMaster.Stop();
        }
    }
}
