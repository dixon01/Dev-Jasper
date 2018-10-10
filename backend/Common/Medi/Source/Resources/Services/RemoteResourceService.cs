// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteResourceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.Medi.Resources.Messages;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Resource service that accesses a service in a different Medi node.
    /// The node still has to be on the same unit, but it can be in a different process.
    /// </summary>
    internal class RemoteResourceService : ResourceServiceBase, IConfigurable<RemoteResourceServiceConfig>
    {
        private readonly Dictionary<int, IAsyncResult> pendingRequests = new Dictionary<int, IAsyncResult>();

        private MediAddress remoteAddress;

        private int nextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteResourceService"/> class.
        /// </summary>
        public RemoteResourceService()
        {
            this.nextId = new Random().Next();
        }

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="config">
        /// The config object.
        /// </param>
        public void Configure(RemoteResourceServiceConfig config)
        {
            this.Configure((ResourceServiceConfigBase)config);
        }

        /// <summary>
        /// Starts this service.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that owns this service.
        /// </param>
        public override void Start(IMessageDispatcherImpl dispatcher)
        {
            base.Start(dispatcher);

            this.remoteAddress = new MediAddress
                                     {
                                         Unit = this.Dispatcher.LocalAddress.Unit,
                                         Application = ResourceMessageHandler.DispatcherName
                                     };

            this.Dispatcher.Subscribe<ResourceResultMessage<ResourceId>>(this.HandleResultMessage);
            this.Dispatcher.Subscribe<ResourceResultMessage<ExtendedResourceInfo>>(this.HandleResultMessage);
            this.Dispatcher.Subscribe<ResourceResultMessage<bool>>(this.HandleResultMessage);
        }

        /// <summary>
        /// Stops this service.
        /// </summary>
        public override void Stop()
        {
            if (this.Dispatcher == null)
            {
                return;
            }

            base.Stop();

            this.Dispatcher.Unsubscribe<ResourceResultMessage<ResourceId>>(this.HandleResultMessage);
            this.Dispatcher.Unsubscribe<ResourceResultMessage<ExtendedResourceInfo>>(this.HandleResultMessage);
            this.Dispatcher.Unsubscribe<ResourceResultMessage<bool>>(this.HandleResultMessage);
        }

        /// <summary>
        /// Begins the removal of a resource from this service.
        /// Any local copies of the resource will be kept, but the
        /// service will no longer keep track of the given resource.
        /// </summary>
        /// <param name="id">
        /// The unique resource id.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="IResourceService.EndRemoveResource"/>.
        /// </returns>
        public override IAsyncResult BeginRemoveResource(ResourceId id, AsyncCallback callback, object state)
        {
            this.Logger.Trace("BeginRemoveResource({0})", id);
            var msg = new RemoveResourceMessage { Id = id };
            return this.SendMessage<bool>(msg, callback, state);
        }

        /// <summary>
        /// Begins the registration of a resource in this service.
        /// Use <see cref="ResourceServiceBase.EndRegisterResource"/> when the operation completed.
        /// </summary>
        /// <param name="localFile">
        ///     A local file.
        /// </param>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="deleteLocal">
        ///     A flag indicating whether the local file should be
        ///     deleted after being registered.
        /// </param>
        /// <param name="temporary">
        ///     A flag indicating whether the file is only to be kept in the store
        ///     temporarily.
        /// </param>
        /// <param name="callback">
        ///     The asynchronous callback.
        /// </param>
        /// <param name="state">
        ///     The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="ResourceServiceBase.EndRegisterResource"/>.
        /// </returns>
        internal override IAsyncResult BeginRegisterResource(
            string localFile,
            MediAddress source,
            bool deleteLocal,
            bool temporary,
            AsyncCallback callback,
            object state)
        {
            this.Logger.Trace("BeginRegisterResource({0},<{1}>,{2},{3})", localFile, source, deleteLocal, temporary);
            var msg = new RegisterResourceMessage
                          {
                              LocalFile = Path.GetFullPath(localFile),
                              Source = source,
                              DeleteLocal = deleteLocal,
                              Temporary = temporary
                          };
            return this.SendMessage<ResourceId>(msg, callback, state);
        }

        /// <summary>
        /// Announces a resource to the resource service.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="announcement">
        /// The announcement.
        /// </param>
        internal override void AnnounceResource(MediAddress source, ResourceAnnouncement announcement)
        {
            this.Logger.Trace("AnnounceResource(<{0}>,{1})", source, announcement);
            var msg = new AnnounceResourceMessage { Source = source, Announcement = announcement };
            this.Dispatcher.Send(this.remoteAddress, msg);
        }

        /// <summary>
        /// Begins getting a resource with a given unique id from this service.
        /// The result will only complete when the resource is available locally.
        /// </summary>
        /// <param name="id">
        ///     The unique resource id.
        /// </param>
        /// <param name="allowIncomplete">
        /// A flag indicating if an incomplete resource is allowed as a result.
        /// </param>
        /// <param name="callback">
        ///     The asynchronous callback.
        /// </param>
        /// <param name="state">
        ///     The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="ResourceServiceBase.EndGetResourceInfo"/>.
        /// </returns>
        internal override IAsyncResult BeginGetResourceInfo(
            ResourceId id, bool allowIncomplete, AsyncCallback callback, object state)
        {
            this.Logger.Trace("BeginGetResourceInfo({0},{1})", id, allowIncomplete);
            var msg = new GetResourceMessage { Id = id, AllowIncomplete = allowIncomplete };
            return this.SendMessage<ExtendedResourceInfo>(msg, callback, state);
        }

        /// <summary>
        /// Begins getting a local copy of a resource.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="ResourceServiceBase.GetResource"/>.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <param name="keepTracking">
        /// A flag indicating if the resource service should continue tracking the
        /// copied file.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="ResourceServiceBase.EndGetLocalCopy"/>.
        /// </returns>
        internal override IAsyncResult BeginGetLocalCopy(
            ResourceInfo info, string localFile, bool keepTracking, AsyncCallback callback, object state)
        {
            localFile = Path.GetFullPath(localFile);
            this.Logger.Trace("BeginGetLocalCopy({0},{1},{2})", info.Id, localFile, keepTracking);
            var msg = new GetLocalCopyMessage { Info = info, LocalFile = localFile, KeepTracking = keepTracking };
            return this.SendMessage<bool>(msg, callback, state);
        }

        /// <summary>
        /// Sets the download state of a given resource.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="downloadedBytes">
        /// The downloaded bytes.
        /// </param>
        /// <param name="tempFilename">
        /// The temp filename.
        /// </param>
        internal override void SetDownloadState(ResourceId id, long downloadedBytes, string tempFilename)
        {
            this.Logger.Trace("SetDownloadState({0},{1},{2})", id, downloadedBytes, tempFilename);
            var msg = new SetDownloadStateMessage
                          {
                              Id = id,
                              DownloadedBytes = downloadedBytes,
                              TempFilename = tempFilename
                          };
            this.Dispatcher.Send(this.remoteAddress, msg);
        }

        /// <summary>
        /// Sets the upload state of a given resource.
        /// </summary>
        /// <param name="id">
        /// The id of the resource.
        /// </param>
        /// <param name="source">
        /// The source node where the upload takes place.
        /// </param>
        /// <param name="destination">
        /// The destination address where the resource is being sent to.
        /// </param>
        /// <param name="isTemporary">
        /// A flag indicating if the upload is temporary (i.e. only a "send file").
        /// </param>
        /// <param name="originalName">
        /// The original file name of the file being sent (this can be null for non-temporary transfers).
        /// </param>
        internal override void SetUploading(
            ResourceId id,
            MediAddress source,
            MediAddress destination,
            bool isTemporary,
            string originalName)
        {
            this.Logger.Trace(
                "SetUploading({0},<{1}>,<{2}>,{3},'{4}')",
                id,
                source,
                destination,
                isTemporary,
                originalName);
            var msg = new SetUploadStateMessage
                          {
                              Id = id,
                              Source = source,
                              Destination = destination,
                              IsTemporary = isTemporary,
                              OriginalName = originalName,
                              Uploading = true
                          };
            this.Dispatcher.Send(this.remoteAddress, msg);
        }

        /// <summary>
        /// Clears the upload state of a given resource.
        /// </summary>
        /// <param name="id">
        /// The id of the resource.
        /// </param>
        /// <param name="destination">
        /// The destination address where the resource was sent to.
        /// </param>
        internal override void ClearUploading(ResourceId id, MediAddress destination)
        {
            this.Logger.Trace("ClearUploading({0},<{1}>)", id, destination);
            var msg = new SetUploadStateMessage { Id = id, Destination = destination, Uploading = false };
            this.Dispatcher.Send(this.remoteAddress, msg);
        }

        /// <summary>
        /// Requests read access to the contents of the given resource.
        /// </summary>
        /// <param name="info">
        /// The extended resource information.
        /// </param>
        /// <returns>
        /// The <see cref="IResourceAccess"/> that can be used for reading the resource.
        /// </returns>
        protected override IResourceAccess GetResourceAccess(ExtendedResourceInfo info)
        {
            // we have no other way than to access the file through info.LocalPath;
            // as long as the file is really local, this isn't a problem
            return new FileResourceAccess(this.FileSystem.GetFile(info.LocalPath));
        }

        /// <summary>
        /// Method called when a lock is acquired for the first time.
        /// </summary>
        /// <param name="resourceLock">
        /// The acquired resource lock.
        /// </param>
        protected override void AcquireFirstLock(IResourceLock resourceLock)
        {
            this.Logger.Trace("AcquireFirstLock({0})", resourceLock.Id);
            var msg = new ResourceLockMessage { Id = resourceLock.Id, Lock = true };
            this.Dispatcher.Send(this.remoteAddress, msg);
        }

        /// <summary>
        /// Method called when a lock is finally released (all locks are released).
        /// </summary>
        /// <param name="resourceLock">
        /// The resource lock.
        /// </param>
        protected override void ReleaseLastLock(IResourceLock resourceLock)
        {
            this.Logger.Trace("ReleaseLastLock({0})", resourceLock.Id);
            var msg = new ResourceLockMessage { Id = resourceLock.Id, Lock = false };
            this.Dispatcher.Send(this.remoteAddress, msg);
        }

        private IAsyncResult SendMessage<TResult>(ResourceMessage msg, AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<TResult>(callback, state);
            var id = Interlocked.Increment(ref this.nextId);
            msg.RequestId = id;
            lock (this.pendingRequests)
            {
                this.pendingRequests.Add(id, result);
            }

            this.Dispatcher.Send(this.remoteAddress, msg);
            return result;
        }

        private void HandleResultMessage<T>(object sender, MessageEventArgs<ResourceResultMessage<T>> e)
        {
            IAsyncResult result;
            lock (this.pendingRequests)
            {
                if (this.pendingRequests.TryGetValue(e.Message.RequestId, out result))
                {
                    this.pendingRequests.Remove(e.Message.RequestId);
                }
            }

            var res = result as SimpleAsyncResult<T>;
            if (res == null)
            {
                // this is pretty bad!
                Logger.Warn("Got a result message for a wrong request: {0}", e.Message.RequestId);
                return;
            }

            try
            {
                if (e.Message.ExceptionType == null)
                {
                    res.Complete(e.Message.Result, false);
                    return;
                }

                var type = Type.GetType(e.Message.ExceptionType, false) ?? typeof(ApplicationException);
                var ctor = type.GetConstructor(new[] { typeof(string) });
                Exception exception;
                if (ctor == null)
                {
                    exception = (Exception)Activator.CreateInstance(type);
                }
                else
                {
                    exception = (Exception)ctor.Invoke(new object[] { e.Message.ExceptionMessage });
                }

                res.CompleteException(exception, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Unhandled exception when completing remote request");
            }
        }
    }
}