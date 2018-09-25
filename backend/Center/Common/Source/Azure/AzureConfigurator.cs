// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureConfigurator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Azure
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.WindowsAzure.ServiceRuntime;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    /// <summary>
    /// Configure a Gorba application running on Azure.
    /// </summary>
    public abstract class AzureConfigurator
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        static AzureConfigurator()
        {
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureConfigurator"/> class.
        /// </summary>
        protected AzureConfigurator()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the current configurator.
        /// </summary>
        public static AzureConfigurator Current { get; private set; }

        /// <summary>
        /// Resets the current configurator.
        /// </summary>
        public static void Reset()
        {
            Set(new DefaultAzureConfigurator());
        }

        /// <summary>
        /// Sets the <paramref name="instance"/> as current configurator.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public static void Set(AzureConfigurator instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Configures the Azure system.
        /// </summary>
        public abstract void EnsureConfiguration();

        /// <summary>
        /// The default Azure configurator.
        /// </summary>
        public class DefaultAzureConfigurator : AzureConfigurator
        {
            private const string LogCommandText =
                "INSERT INTO [dbo].[LogEntries]"
                + " ([Application], [Timestamp], [Level], [Logger], [Message], [AdditionalData])"
                + " VALUES ('{0}', @timestamp, @level, @logger, @message, @additionalData)";

            private const int MaxLevel = 5;

            /// <summary>
            /// Configures the Azure system.
            /// </summary>
            public override void EnsureConfiguration()
            {
                try
                {
                    Trace.TraceInformation("Reconfiguring logging");
                    this.ConfigureLogging();
                }
                catch (Exception exception)
                {
                    SimpleConfigurator.ConfigureForTargetLogging(new DebuggerTarget());
                    Trace.TraceError(
                        "Error while configuring NLog. NLog configured to write to the Debugger. Stack trace: {0}",
                        exception.StackTrace);
                }
            }

            /// <summary>
            /// Configures logging.
            /// </summary>
            private void ConfigureLogging()
            {
                var logLevelName = RoleEnvironment.GetConfigurationSettingValue(PredefinedAzureItems.Settings.LogLevel);
                if (string.IsNullOrEmpty(logLevelName))
                {
                    Trace.TraceInformation("Couldn't configure");
                    return;
                }

                int logLevelOrdinal;
                if (!LogLevelOrdinalLayoutRenderer.TryGetOrdinalValue(logLevelName, out logLevelOrdinal))
                {
                    Trace.TraceError("NLog level not valid. Provided value: '{0}'", logLevelName);
                    return;
                }

                var connectionString =
                    RoleEnvironment.GetConfigurationSettingValue(PredefinedAzureItems.Settings.CenterDataContext);
                var logCommandText = string.Format(LogCommandText, RoleEnvironment.DeploymentId);
                var databaseTarget = new DatabaseTarget
                                         {
                                             CommandText = logCommandText,
                                             ConnectionString = connectionString
                                         };
                databaseTarget.Parameters.Add(new DatabaseParameterInfo("@timestamp", "${longdate}"));
                ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition(
                    "levelOrdinal",
                    typeof(LogLevelOrdinalLayoutRenderer));
                databaseTarget.Parameters.Add(new DatabaseParameterInfo("@level", "${levelOrdinal}"));
                databaseTarget.Parameters.Add(new DatabaseParameterInfo("@logger", "${logger}"));
                databaseTarget.Parameters.Add(new DatabaseParameterInfo("@message", "${message}"));
                databaseTarget.Parameters.Add(
                    new DatabaseParameterInfo(
                        "@additionalData",
                        "${onexception:${exception:format=ToString,stacktrace:maxInnerExceptionLevel=10}} "));

                var configuration = new LoggingConfiguration();
                var rule = new LoggingRule("*", databaseTarget);
                Enumerable.Range(logLevelOrdinal, MaxLevel - logLevelOrdinal)
                    .Select(LogLevel.FromOrdinal)
                    .ToList()
                    .ForEach(rule.EnableLoggingForLevel);
                configuration.LoggingRules.Add(rule);
                LogManager.Configuration = configuration;
                LogManager.ReconfigExistingLoggers();
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info("Logging reconfigured");
            }
        }
    }
}