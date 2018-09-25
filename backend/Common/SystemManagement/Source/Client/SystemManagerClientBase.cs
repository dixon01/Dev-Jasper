// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerClientBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerClientBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Messages;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Motion.SystemManager.ServiceModel.Messages;

    /// <summary>
    /// The base class for system manager clients.
    /// </summary>
    public abstract class SystemManagerClientBase
    {
        private readonly List<SimpleAsyncResult<IList<ApplicationInfo>>> awaitingRequests =
            new List<SimpleAsyncResult<IList<ApplicationInfo>>>();

        private readonly List<SimpleAsyncResult<SystemInfo>> awaitingSystemRequests =
            new List<SimpleAsyncResult<SystemInfo>>();

        private readonly IMessageDispatcher messageDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManagerClientBase"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The <see cref="MessageDispatcher"/> to use for communication with the remote System Manager.
        /// </param>
        /// <param name="unitName">
        /// The name of the unit on which the System Manager resides.
        /// </param>
        protected SystemManagerClientBase(IMessageDispatcher messageDispatcher, string unitName)
        {
            this.messageDispatcher = messageDispatcher;
            this.SystemManagerAddress = new MediAddress(unitName, Addresses.SystemManagerDispatcher);
            this.messageDispatcher.Subscribe<ApplicationInfosResponse>(this.HandleApplicationInfosResponse);
            this.messageDispatcher.Subscribe<SystemInfoResponse>(this.HandleSystemInfoResponse);
        }

        /// <summary>
        /// Gets the Medi address of the system manager.
        /// </summary>
        public MediAddress SystemManagerAddress { get; private set; }

        /// <summary>
        /// Begins to get information about all applications managed by the System Manager.
        /// </summary>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to use to call <see cref="EndGetApplicationInfos"/>.
        /// </returns>
        public IAsyncResult BeginGetApplicationInfos(AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<IList<ApplicationInfo>>(callback, state);
            lock (this.awaitingRequests)
            {
                this.awaitingRequests.Add(result);
            }

            this.SendMessageToSystemManager(new ApplicationInfosRequest());
            return result;
        }

        /// <summary>
        /// Ends the asynchronous request started with <see cref="BeginGetApplicationInfos"/>.
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result returned by <see cref="BeginGetApplicationInfos"/>.
        /// </param>
        /// <returns>
        /// A list of all applications that are managed by the System Manager.
        /// </returns>
        public IList<ApplicationInfo> EndGetApplicationInfos(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<IList<ApplicationInfo>>;
            if (result == null)
            {
                throw new ArgumentException("Provide the result returned by BeginGetApplicationInfos()", "ar");
            }

            result.WaitForCompletionAndVerify();
            return result.Value;
        }

        /// <summary>
        /// Creates an <see cref="IApplicationStateObserver"/> for the given application.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <returns>
        /// The <see cref="IApplicationStateObserver"/>.
        /// </returns>
        public IApplicationStateObserver CreateApplicationStateObserver(ApplicationInfo application)
        {
            var observer = new ApplicationStateObserver(application, this.messageDispatcher, this.SystemManagerAddress);
            this.SendMessageToSystemManager(new ApplicationStateRequest { ApplicationId = application.Id });
            return observer;
        }

        /// <summary>
        /// Requests the given <see cref="ApplicationInfo"/> to exit with the given reason.
        /// </summary>
        /// <param name="application">
        /// The application to exit.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void ExitApplication(ApplicationInfo application, string reason)
        {
            this.SendMessageToSystemManager(
                new ExitApplicationRequest { ApplicationId = application.Id, Reason = reason });
        }

        /// <summary>
        /// Requests the given <see cref="ApplicationInfo"/> to re-launch with the given reason.
        /// </summary>
        /// <param name="application">
        /// The application to re-launch.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void RelaunchApplication(ApplicationInfo application, string reason)
        {
            this.SendMessageToSystemManager(
                new RelaunchApplicationRequest { ApplicationId = application.Id, Reason = reason });
        }

        /// <summary>
        /// Begins to get information about the system managed by the System Manager.
        /// </summary>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to use to call <see cref="EndGetSystemInfo"/>.
        /// </returns>
        public IAsyncResult BeginGetSystemInfo(AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<SystemInfo>(callback, state);
            lock (this.awaitingSystemRequests)
            {
                this.awaitingSystemRequests.Add(result);
            }

            this.SendMessageToSystemManager(new SystemInfoRequest());
            return result;
        }

        /// <summary>
        /// Ends the asynchronous request started with <see cref="BeginGetSystemInfo"/>.
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result returned by <see cref="BeginGetSystemInfo"/>.
        /// </param>
        /// <returns>
        /// The system information known by the System Manager.
        /// </returns>
        public SystemInfo EndGetSystemInfo(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<SystemInfo>;
            if (result == null)
            {
                throw new ArgumentException("Provide the result returned by BeginGetSystemInfo()", "ar");
            }

            result.WaitForCompletionAndVerify();
            return result.Value;
        }

        /// <summary>
        /// Requests to reboot the system.
        /// </summary>
        /// <param name="reason">
        /// The reason for the reboot.
        /// </param>
        public void Reboot(string reason)
        {
            this.SendMessageToSystemManager(new RebootSystemRequest { Reason = reason });
        }

        /// <summary>
        /// Closes this client and clears all waiting requests.
        /// </summary>
        protected void Close()
        {
            this.messageDispatcher.Unsubscribe<ApplicationInfosResponse>(this.HandleApplicationInfosResponse);
            this.messageDispatcher.Unsubscribe<SystemInfoResponse>(this.HandleSystemInfoResponse);

            SimpleAsyncResult<IList<ApplicationInfo>>[] requests;
            lock (this.awaitingRequests)
            {
                if (this.awaitingRequests.Count == 0)
                {
                    return;
                }

                requests = this.awaitingRequests.ToArray();
                this.awaitingRequests.Clear();
            }

            var emtpy = new ApplicationInfo[0];
            foreach (var request in requests)
            {
                request.Complete(emtpy, false);
            }
        }

        private void SendMessageToSystemManager(object message)
        {
            this.messageDispatcher.Send(this.SystemManagerAddress, message);
        }

        private void HandleApplicationInfosResponse(object sender, MessageEventArgs<ApplicationInfosResponse> e)
        {
            SimpleAsyncResult<IList<ApplicationInfo>>[] requests;
            lock (this.awaitingRequests)
            {
                requests = this.awaitingRequests.ToArray();
                this.awaitingRequests.Clear();
            }

            var infos = e.Message.Infos.ConvertAll(i => new ApplicationInfo(i));

            foreach (var request in requests)
            {
                request.Complete(infos, false);
            }
        }

        private void HandleSystemInfoResponse(object sender, MessageEventArgs<SystemInfoResponse> e)
        {
            SimpleAsyncResult<SystemInfo>[] requests;
            lock (this.awaitingSystemRequests)
            {
                requests = this.awaitingSystemRequests.ToArray();
                this.awaitingSystemRequests.Clear();
            }

            var info = new SystemInfo(e.Message);

            foreach (var request in requests)
            {
                request.Complete(info, false);
            }
        }
    }
}