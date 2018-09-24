// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHostExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The service host extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Setup
{
    using System;
    using System.ServiceModel;
    using System.Text;

    using NLog;

    /// <summary>
    /// The service host extensions.
    /// </summary>
    public static class ServiceHostExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The handle events.
        /// </summary>
        /// <param name="serviceHost">
        /// The service host.
        /// </param>
        public static void HandleEvents(this ServiceHost serviceHost)
        {
            serviceHost.Closed += OnHostClosed;
            serviceHost.Closing += OnHostClosing;
            serviceHost.Faulted += OnHostFaulted;
            serviceHost.Opened += OnHostOpened;
            serviceHost.Opening += OnHostOpening;
        }

        private static void OnHostClosed(object sender, EventArgs e)
        {
            var description = GetDescription(sender);
            Logger.Trace("Host closed: {0}", description);
        }

        private static void OnHostClosing(object sender, EventArgs e)
        {
            var description = GetDescription(sender);
            Logger.Trace("Host closing: {0}", description);
        }

        private static void OnHostFaulted(object sender, EventArgs e)
        {
            var description = GetDescription(sender);
            Logger.Error("Host faulted: {0}", description);
        }

        private static void OnHostOpened(object sender, EventArgs e)
        {
            var description = GetDescription(sender);
            Logger.Debug("Host opened: {0}", description);
        }

        private static void OnHostOpening(object sender, EventArgs e)
        {
            var description = GetDescription(sender);
            Logger.Trace("Host opening: {0}", description);
        }

        private static string GetDescription(object sender)
        {
            var host = sender as ServiceHost;
            if (host == null)
            {
                return sender == null ? "-" : sender.ToString();
            }

            var stringBuilder = new StringBuilder("Host [");
            try
            {
                var first = true;
                foreach (var endpoint in host.Description.Endpoints)
                {
                    if (!first)
                    {
                        stringBuilder.Append(';');
                    }

                    first = false;
                    stringBuilder.AppendFormat(
                        "{0},{1}",
                        endpoint.Contract.ContractType.Name,
                        host.SingletonInstance);
                }
            }
            catch (Exception exception)
            {
                Logger.Warn("Error while creating description for host {0}", exception);
            }

            stringBuilder.Append(']');
            return stringBuilder.ToString();
        }
    }
}