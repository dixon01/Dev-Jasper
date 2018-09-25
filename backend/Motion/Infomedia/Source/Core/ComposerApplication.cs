// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.Composer;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Core.Data;
    using Gorba.Motion.Infomedia.Core.Presentation;

    using Microsoft.Practices.ServiceLocation;

#if PresentationPlayLogging
    using Luminator.PresentationPlayLogging;
    using Luminator.PresentationPlayLogging.Core;
#endif

#if WindowsCE
    using FileSystemEventArgs = OpenNETCF.IO.FileSystemEventArgs;

    // using FileSystemMonitor instead of FileSystemWatcher since it doesn't require a UI
    using FileSystemWatcher = OpenNETCF.IO.FileSystemMonitor;
    using NotifyFilters = OpenNETCF.IO.NotifyFilters;
#endif

    /// <summary>
    /// This class handles the management of Composer
    /// </summary>
    public class ComposerApplication : ApplicationBase, IManageable
    {
        /// <summary>
        /// The management name.
        /// </summary>
        internal static readonly string ManagementName = "Composer";

        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private readonly TableController tableController;

        private readonly ITimer reloadTimer;

        private readonly FileSystemWatcher presentationFileWatcher;

        private FileCheck fileCheck;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposerApplication"/> class.
        /// </summary>
        public ComposerApplication()
        {
            var configPath = PathManager.Instance.GetPath(FileType.Config, "Composer.xml");

            var configMgr = new ConfigManager<ComposerConfig>();
            configMgr.FileName = configPath;
            configMgr.EnableCaching = true;
            configMgr.XmlSchema = ComposerConfig.Schema;
            var config = configMgr.Config;

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            this.tableController = new TableController(config.XimpleInactivity);
            serviceContainer.RegisterInstance<ITableController>(this.tableController);

            this.presentationFileWatcher = new FileSystemWatcher();
            this.presentationFileWatcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.CreationTime
                | NotifyFilters.LastWrite | NotifyFilters.FileName;
            this.presentationFileWatcher.Created += this.OnPresentationFileChanged;
            this.presentationFileWatcher.Changed += this.OnPresentationFileChanged;
            this.presentationFileWatcher.Renamed += this.OnPresentationFileChanged;
            this.presentationFileWatcher.Deleted += this.OnPresentationFileChanged;

            this.reloadTimer = TimerFactory.Current.CreateTimer("ReloadPresentation");
            this.reloadTimer.Interval = TimeSpan.FromSeconds(5);
            this.reloadTimer.AutoReset = false;
            this.reloadTimer.Elapsed += this.ReloadTimerOnElapsed;

            var root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            var provider = MessageDispatcher.Instance.ManagementProviderFactory.CreateManagementProvider(
                ManagementName, root, this);
            root.AddChild(provider);

            if (config.EnablePresentationLogging)
            {
                this.Logger.Info("Initializing Presentation Play logging.");
                this.InitPresentationPlayLogging();
            }
            else
            {
                this.Logger.Info("Presentation Play logging disabled in Composer configuration.");
            }
        }

        private void InitPresentationPlayLogging()
        {
#if PresentationPlayLogging
            try
            {
                this.PresentationCsvLogging = PresentationPlayLoggingFactory.CreatePresentationInfotransitCsvLogging();
                this.PresentationCsvLogging.Start();
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                this.Logger.Warn("Failed to read Presentation Log file {0}", fileNotFoundException.Message);
            }
#endif
        }

        /// <summary>
        /// The presentation started.
        /// </summary>
        public event EventHandler<EventArgs> PresentationStarted;

        /// <summary>
        /// Gets the presentation manager.
        /// </summary>
        public PresentationManager PresentationManager { get; private set; }

        /// <summary>
        /// Gets the presentation file.
        /// </summary>
        public string File { get; private set; }

#if PresentationPlayLogging
        /// <summary>Gets the presentation logging.</summary>
        public PresentationInfotransitCsvLogging PresentationCsvLogging { get; private set; }
#endif      

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Table Controller", parent, this.tableController);
            yield return parent.Factory.CreateManagementProvider("Presentation", parent, this.PresentationManager);
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// Implementing classes should either override <see cref="ApplicationBase.DoRun()"/> or this method.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        protected override void DoRun(string[] args)
        {
            var presentationFilename = args.Length > 0 ? args[0] : "main.im2";
            this.File = PathManager.Instance.GetPath(FileType.Presentation, presentationFilename);

            if (this.File == null)
            {
                Logger.Error("Could Not file presentation file in {0}", presentationFilename);
                throw new FileNotFoundException("Couldn't find presentation:" + presentationFilename);
            }

            this.RestartPresentation();
            this.SetRunning();

            this.fileCheck = new FileCheck(this.File);
            this.presentationFileWatcher.Path = Path.GetDirectoryName(this.File);
            var fileWatchPath = Path.GetFileName(this.File);
            Logger.Info("FileWatcher set for changes on path={0}", fileWatchPath);
            this.presentationFileWatcher.Filter = fileWatchPath;
            this.presentationFileWatcher.EnableRaisingEvents = true;
            this.runWait.WaitOne();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="DoRun"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.presentationFileWatcher.EnableRaisingEvents = false;
            this.presentationFileWatcher.Dispose();

            if (this.PresentationManager != null)
            {
                this.PresentationManager.Dispose();
            }

            this.runWait.Set();
        }

        private void RaisePresentationStarted(EventArgs e)
        {
            var handler = this.PresentationStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnPresentationFileChanged(object sender, FileSystemEventArgs e)
        {
            if (this.fileCheck.CheckChanged() && this.fileCheck.Exists)
            {
                this.reloadTimer.Enabled = false;
                this.reloadTimer.Enabled = true;
            }
        }

        private void ReloadTimerOnElapsed(object sender, EventArgs e)
        {
            try
            {
                this.RestartPresentation();
            }
            catch (Exception ex)
            {
                Logger.Debug("Could not reload config from File {0}, Exception {1}", this.File, ex.Message);
            }
        }

        private void RestartPresentation()
        {
            this.Logger.Info("RestartPresentation Loading configuration {0}", this.File);
            var configContext = new PresentationConfigContext(this.File);
            if (configContext.Config == null)
            {
                throw new ConfiguratorException("Could not load config from " + this.File, typeof(InfomediaConfig));
            }

            if (this.PresentationManager != null)
            {
                this.PresentationManager.Dispose();
                this.PresentationManager = null;
            }

            this.PresentationManager = new PresentationManager();
            this.PresentationManager.Configure(configContext);
            this.PresentationManager.Start();
            this.RaisePresentationStarted(EventArgs.Empty);
        }
    }
}
