// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationHost.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;
    using NLog.Config;
    using NLog.LayoutRenderers;
    using NLog.Layouts;
    using NLog.Targets;

    /// <summary>
    /// Application host that does most setup and teardown work required when running a Gorba application.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the application to be run in this host.
    /// </typeparam>
    public class ApplicationHost<T>
        where T : IRunnableApplication, new()
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly string[] arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationHost{T}"/> class.
        /// This will create a new instance of the <see cref="T"/> application once
        /// all configuration is set up (NLog, Medi and Service Container).
        /// </summary>
        /// <param name="arguments">
        /// The command line arguments.
        /// </param>
        public ApplicationHost(params string[] arguments)
        {
            this.arguments = arguments;

            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomainOnUnhandledException;

#if __UseLuminatorTftDisplay
            var nlogConfig = System.IO.Path.Combine(ApplicationHelper.CurrentDirectory, "nlog.config");
            if (File.Exists(nlogConfig) == false)
            {
                nlogConfig = PathManager.Instance.GetPath(FileType.Config, "nlog.config");
            }
            LogManager.Configuration = new XmlLoggingConfiguration(nlogConfig);

            // Enable Auto Reload
            var xmlLoggingConfiguration = ((XmlLoggingConfiguration)LogManager.Configuration);
            xmlLoggingConfiguration.AutoReload = true;
            
            // Add Sentinal target and rule if missing - To be Tested
            bool EnableSentinal = false;
            if (EnableSentinal)
            {
                const string Sentinal = "sentinal";
                var sentinalTarget = xmlLoggingConfiguration.FindTargetByName(Sentinal);
                if (sentinalTarget == null)
                {
                    sentinalTarget = new NLogViewerTarget
                    {
                        IncludeSourceInfo = true,
                        Name = Sentinal,
                        Address = Layout.FromString("udp://127.0.0.1:9999"),
                        IncludeNLogData = false
                    };
                    xmlLoggingConfiguration.AddTarget(Sentinal, sentinalTarget);

                    // Add the Rule if missing
                    var sentinalRule = new LoggingRule("*", LogLevel.Debug, sentinalTarget);
                    LogManager.Configuration.LoggingRules.Add(sentinalRule);
                }

        
                LogManager.ReconfigExistingLoggers();
            }
           

#else
            var nlogConfig = PathManager.Instance.GetPath(FileType.Config, "nlog.config");
            LogManager.Configuration = new XmlLoggingConfiguration(nlogConfig);
#endif

            this.Logger = LogHelper.GetLogger(this.GetType());
            var asmLocation = ApplicationHelper.GetEntryAssemblyLocation();
            var version = ApplicationHelper.GetApplicationFileVersion();
            this.Logger.Info(
                "Starting {0} {1} on {2}",
                System.IO.Path.GetFileName(asmLocation),
                version,
                ApplicationHelper.MachineName);

            this.SetConsoleTitleVersion(version);

            this.Logger.Info("NLog configuration: {0}", nlogConfig);

            var serviceContainer = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => new ServiceContainerLocator(serviceContainer));

            var mediConfig = PathManager.Instance.GetPath(FileType.Config, "medi.config");
            this.Logger.Info("Medi configuration: {0}", mediConfig);
            var configurator = new FileConfigurator(mediConfig);
            MessageDispatcher.Instance.Configure(configurator);

            // change the path provider because we don't want to have .cache files
            // in the Config or Presentation directories
            CachePathProvider.Current = new ManagedCachePathProvider();

            this.Application = new T();
            serviceContainer.RegisterInstance(this.Application);
        }

        private void SetConsoleTitleVersion(string version)
        {
            try
            {
                var mainWindow = Process.GetCurrentProcess().MainWindowHandle;
                if (mainWindow != IntPtr.Zero)
                {
                    Console.Title = string.Format("{0} - {1}", ApplicationHelper.GetEntryAssemblyName(), version);
                }
            }
            catch (IOException)
            {
                // Ignored
            }
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        public T Application { get; private set; }

        /// <summary>
        /// Runs this application.
        /// </summary>
        /// <param name="name">
        /// The name of the application.
        /// </param>
        public virtual void Run(string name)
        {
            this.Application.Configure(name);
            this.Application.Run(this.arguments);
            FlushLog(TimeSpan.FromSeconds(1));
        }

        private static void FlushLog(TimeSpan timeout)
        {
            try
            {
                LogManager.Flush(timeout);
            }
            catch (NLogRuntimeException)
            {
                // ignore this exception since we can't log it
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                this.Logger.Fatal(ex, "!!!! CurrentDomainOnUnhandledException Unhandled Exception; terminating={0}", e.IsTerminating);
            }
            else
            {
                this.Logger.Fatal("!!!! CurrentDomainOnUnhandledException Unhandled Exception; terminating!");
            }

            FlushLog(TimeSpan.FromSeconds(10));
        }
    }
}