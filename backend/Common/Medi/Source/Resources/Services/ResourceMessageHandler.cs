// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceMessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceMessageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Services
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.Medi.Resources.Messages;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Handler class for Medi messages used to remotely control a
    /// resource service. This class is only used by
    /// <see cref="LocalResourceService"/>.
    /// </summary>
    internal sealed class ResourceMessageHandler
    {
        // Needed for CF 3.5:
        // ReSharper disable RedundantTypeArgumentsOfMethod

        /// <summary>
        /// Named dispatcher name of the resource service.
        /// </summary>
        internal static readonly string DispatcherName = "ResourceService";

        private static readonly Logger Logger = LogHelper.GetLogger<ResourceMessageHandler>();

        private readonly Dictionary<MediAddress, Dictionary<ResourceId, IResourceLock>> locks =
            new Dictionary<MediAddress, Dictionary<ResourceId, IResourceLock>>();

        private readonly LocalResourceService service;

        private readonly IMessageDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMessageHandler"/> class.
        /// </summary>
        /// <param name="service">
        /// The service that handles the remote calls.
        /// </param>
        public ResourceMessageHandler(LocalResourceService service)
        {
            this.service = service;
            this.dispatcher = service.Dispatcher.GetNamedDispatcher(DispatcherName);
        }

        // ReSharper disable once TypeParameterCanBeVariant
        private delegate T AsyncEnd<T>(IAsyncResult result);

        /// <summary>
        /// Starts this handler by subscribing to all required messages.
        /// </summary>
        public void Start()
        {
            this.dispatcher.Subscribe<RegisterResourceMessage>(this.RegisterResource);
            this.dispatcher.Subscribe<RemoveResourceMessage>(this.RemoveResource);
            this.dispatcher.Subscribe<GetResourceMessage>(this.GetResource);
            this.dispatcher.Subscribe<GetLocalCopyMessage>(this.GetLocalCopy);
            this.dispatcher.Subscribe<AnnounceResourceMessage>(this.AnnounceResource);
            this.dispatcher.Subscribe<ResourceLockMessage>(this.HandleResourceLock);
            this.dispatcher.Subscribe<SetDownloadStateMessage>(this.SetDownloadState);
            this.dispatcher.Subscribe<SetUploadStateMessage>(this.SetUploadState);

            this.service.Dispatcher.RoutingTable.Updated += this.RoutingTableOnUpdated;
        }

        /// <summary>
        /// Stops this handler by unsubscribing from all required messages.
        /// </summary>
        public void Stop()
        {
            this.dispatcher.Unsubscribe<RegisterResourceMessage>(this.RegisterResource);
            this.dispatcher.Unsubscribe<RemoveResourceMessage>(this.RemoveResource);
            this.dispatcher.Unsubscribe<GetResourceMessage>(this.GetResource);
            this.dispatcher.Unsubscribe<GetLocalCopyMessage>(this.GetLocalCopy);
            this.dispatcher.Unsubscribe<AnnounceResourceMessage>(this.AnnounceResource);
            this.dispatcher.Unsubscribe<ResourceLockMessage>(this.HandleResourceLock);
            this.dispatcher.Unsubscribe<SetDownloadStateMessage>(this.SetDownloadState);
            this.dispatcher.Unsubscribe<SetUploadStateMessage>(this.SetUploadState);

            this.service.Dispatcher.RoutingTable.Updated -= this.RoutingTableOnUpdated;
        }

        private void RegisterResource(object sender, MessageEventArgs<RegisterResourceMessage> e)
        {
            var msg = e.Message;
            this.service.BeginRegisterResource(
                msg.LocalFile,
                msg.Source,
                msg.DeleteLocal,
                msg.Temporary,
                ar => this.SendResult<ResourceId>(ar, this.service.EndRegisterResource, msg, e.Source),
                null);
        }

        private void RemoveResource(object sender, MessageEventArgs<RemoveResourceMessage> e)
        {
            var msg = e.Message;
            this.service.BeginRemoveResource(
                msg.Id,
                ar => this.SendResult<bool>(ar, this.service.EndRemoveResource, msg, e.Source),
                null);
        }

        private void GetResource(object sender, MessageEventArgs<GetResourceMessage> e)
        {
            var msg = e.Message;
            this.service.BeginGetResourceInfo(
                msg.Id,
                msg.AllowIncomplete,
                ar => this.SendResult<ExtendedResourceInfo>(ar, this.service.EndGetResourceInfo, msg, e.Source),
                null);
        }

        private void GetLocalCopy(object sender, MessageEventArgs<GetLocalCopyMessage> e)
        {
            var msg = e.Message;
            this.service.BeginGetLocalCopy(
                msg.Info,
                msg.LocalFile,
                msg.KeepTracking,
                ar => this.SendResult<bool>(ar, this.service.EndCheckoutResource, msg, e.Source),
                null);
        }

        private void AnnounceResource(object sender, MessageEventArgs<AnnounceResourceMessage> e)
        {
            var msg = e.Message;
            this.service.AnnounceResource(msg.Source, msg.Announcement);
        }

        private void HandleResourceLock(object sender, MessageEventArgs<ResourceLockMessage> e)
        {
            Dictionary<ResourceId, IResourceLock> lockList;
            lock (this.locks)
            {
                if (!this.locks.TryGetValue(e.Source, out lockList))
                {
                    lockList = new Dictionary<ResourceId, IResourceLock>();
                    this.locks.Add(e.Source, lockList);
                }
            }

            lock (lockList)
            {
                IResourceLock resourceLock;
                if (!lockList.TryGetValue(e.Message.Id, out resourceLock))
                {
                    if (!e.Message.Lock)
                    {
                        Logger.Warn("Trying to release unknown resource lock for {0} from {1}", e.Message.Id, e.Source);
                        return;
                    }

                    resourceLock = this.service.AcquireLock(e.Message.Id);
                    lockList.Add(e.Message.Id, resourceLock);
                }
                else if (!e.Message.Lock)
                {
                    resourceLock.Release();
                    lockList.Remove(resourceLock.Id);
                }
                else
                {
                    Logger.Warn("Trying to lock an already locked resource ({0}) from {1}", e.Message.Id, e.Source);
                }
            }
        }

        private void SetDownloadState(object sender, MessageEventArgs<SetDownloadStateMessage> e)
        {
            this.service.SetDownloadState(e.Message.Id, e.Message.DownloadedBytes, e.Message.TempFilename);
        }

        private void SetUploadState(object sender, MessageEventArgs<SetUploadStateMessage> e)
        {
            if (e.Message.Uploading)
            {
                this.service.SetUploading(
                    e.Message.Id,
                    e.Message.Source,
                    e.Message.Destination,
                    e.Message.IsTemporary,
                    e.Message.OriginalName);
            }
            else
            {
                this.service.ClearUploading(e.Message.Id, e.Message.Destination);
            }
        }

        private void SendResult<T>(
            IAsyncResult ar, AsyncEnd<T> endMethod, ResourceMessage request, MediAddress destination)
        {
            var message = new ResourceResultMessage<T> { RequestId = request.RequestId };
            try
            {
                message.Result = endMethod(ar);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Remote call failed, sending exception to caller");
                message.ExceptionType = ex.GetType().AssemblyQualifiedName;
                message.ExceptionMessage = ex.Message;
            }

            this.dispatcher.Send(destination, message);
        }

        private void RoutingTableOnUpdated(object sender, RouteUpdatesEventArgs e)
        {
            // remove all locks that are held by an application that just disconnected
            var updates = new List<RouteUpdate>(e.Updates);
            lock (this.locks)
            {
                foreach (var update in updates)
                {
                    if (update.Added || updates.FindIndex(i => i.Address.Equals(update.Address) && i.Added) >= 0)
                    {
                        // address was not removed or [removed and added in the same transaction], we keep the locks
                        continue;
                    }

                    if (this.locks.Remove(update.Address))
                    {
                        Logger.Debug("Removed locks for {0}", update.Address);
                    }
                }
            }
        }
    }
}