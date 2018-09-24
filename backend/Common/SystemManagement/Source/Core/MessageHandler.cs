// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core
{
    using System;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Messages;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Motion.SystemManager.ServiceModel.Messages;

    using NLog;

    /// <summary>
    /// The message handler for the <see cref="SystemManagementControllerBase"/>.
    /// </summary>
    internal class MessageHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<MessageHandler>();

        private readonly SystemManagementControllerBase controller;

        private readonly IMessageDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public MessageHandler(SystemManagementControllerBase controller)
        {
            this.controller = controller;

            this.dispatcher = MessageDispatcher.Instance.GetNamedDispatcher(Addresses.SystemManagerDispatcher);
        }

        /// <summary>
        /// Starts this handler and registers it with Medi.
        /// </summary>
        public void Start()
        {
            this.dispatcher.Subscribe<ApplicationInfosRequest>(this.HandleApplicationInfosRequest);
            this.dispatcher.Subscribe<SystemInfoRequest>(this.HandleSystemInfoRequest);
            this.dispatcher.Subscribe<RebootSystemRequest>(this.HandleRebootSystemRequest);
        }

        /// <summary>
        /// Stops this handler and de-registers it from Medi.
        /// </summary>
        public void Stop()
        {
            if (this.dispatcher == null)
            {
                return;
            }

            this.dispatcher.Unsubscribe<ApplicationInfosRequest>(this.HandleApplicationInfosRequest);
            this.dispatcher.Unsubscribe<SystemInfoRequest>(this.HandleSystemInfoRequest);
            this.dispatcher.Unsubscribe<RebootSystemRequest>(this.HandleRebootSystemRequest);
        }

        private void HandleApplicationInfosRequest(object sender, MessageEventArgs<ApplicationInfosRequest> e)
        {
            var response = new ApplicationInfosResponse();
            foreach (var appController in this.controller.ApplicationManager.Controllers)
            {
                response.Infos.Add(appController.CreateApplicationInfo().ToMessage());
            }

            this.dispatcher.Send(e.Source, response);
        }

        private void HandleSystemInfoRequest(object sender, MessageEventArgs<SystemInfoRequest> e)
        {
            var response = new SystemInfoResponse
                               {
                                   CpuUsage = this.controller.SystemResources.CpuUsage,
                                   AvailableRam = this.controller.SystemResources.AvailableRam,
                                   TotalRam = this.controller.SystemResources.TotalRam
                               };

            foreach (var drive in FileSystemManager.Local.GetDrives())
            {
                try
                {
                    response.Disks.Add(
                        new DiskInfo
                            {
                                Name = drive.Name,
                                AvailableFreeSpace = drive.AvailableFreeSpace,
                                TotalSize = drive.TotalSize
                            });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Couldn't get disk information");
                }
            }

            this.dispatcher.Send(e.Source, response);
        }

        private void HandleRebootSystemRequest(object sender, MessageEventArgs<RebootSystemRequest> e)
        {
            var reason = string.Format("From {0}: {1}", e.Source, e.Message.Reason);
            ThreadPool.QueueUserWorkItem(s => this.controller.RequestReboot(reason));
        }
    }
}