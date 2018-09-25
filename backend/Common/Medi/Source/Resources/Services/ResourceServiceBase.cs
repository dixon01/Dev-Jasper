// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceServiceBase.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceServiceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Services
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Streams;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Services;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.Medi.Resources.Messages;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// Base class for all <see cref="IResourceService"/>
    /// implementations in this namespace. Provides some helper methods
    /// and default implementations.
    /// </summary>
    internal abstract class ResourceServiceBase : IResourceServiceImpl, IServiceImpl
    {
        /// <summary>
        /// Management name of this class (or subclasses).
        /// </summary>
        internal static readonly string ManagementName = "ResourceService";

        /// <summary>
        /// Common logger for all subclasses
        /// </summary>
        protected readonly Logger Logger;

        private readonly Dictionary<ResourceId, List<IResourceLock>> locks =
            new Dictionary<ResourceId, List<IResourceLock>>();

        private IManagementProvider managementProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceBase"/> class.
        /// </summary>
        protected ResourceServiceBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);

            // take a local copy since we might change the file system ourselves afterwards
            this.FileSystem = (IWritableFileSystem)FileSystemManager.Local;
        }

        /// <summary>
        /// .NET 2.0 equivalent of <code>Func&lt;T&gt;</code>.
        /// </summary>
        /// <returns>
        /// An object of type <typeparamref name="T"/>.
        /// </returns>
        /// <typeparam name="T">
        /// The return type of the method.
        /// </typeparam>
        protected delegate T Method<T>();

        /// <summary>
        /// Event that is fired whenever a file is received.
        /// This event is triggered by a remote node using
        /// <see cref="IResourceService.SendFile"/> (or its asynchronous counterpart).
        /// </summary>
        public event EventHandler<FileReceivedEventArgs> FileReceived;

        /// <summary>
        /// Gets the message dispatcher.
        /// </summary>
        public IMessageDispatcherImpl Dispatcher { get; private set; }

        /// <summary>
        /// Gets the base directory for storing resources.
        /// </summary>
        public IWritableDirectoryInfo BaseDirectory { get; private set; }

        /// <summary>
        /// Gets the file system used by this service to manage resources.
        /// </summary>
        protected IWritableFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Gets the configuration of this service.
        /// </summary>
        protected ResourceServiceConfigBase Config { get; private set; }

        /// <summary>
        /// Gets the <see cref="ResourceId"/> for a given MD5 hash.
        /// </summary>
        /// <param name="hash">
        /// The MD5 hash.
        /// </param>
        /// <returns>
        /// The <see cref="ResourceId"/>.
        /// </returns>
        public static ResourceId HashToId(byte[] hash)
        {
            var sb = new StringBuilder(hash.Length * 2);
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return new ResourceId { Hash = sb.ToString() };
        }

        /// <summary>
        /// Registers a resource in this service.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginRegisterResource"/> instead.
        /// </summary>
        /// <param name="localFile">
        ///     A local file.
        /// </param>
        /// <param name="deleteLocal">
        ///     A flag indicating whether the local file should be
        ///     deleted after being registered.
        /// </param>
        /// <returns>
        /// The unique resource id for the given file.
        /// </returns>
        public virtual ResourceId RegisterResource(string localFile, bool deleteLocal)
        {
            this.Logger.Trace("RegisterResource({0},{1})", localFile, deleteLocal);
            var result = this.BeginRegisterResource(localFile, deleteLocal, null, null);
            return this.EndRegisterResource(result);
        }

        /// <summary>
        /// Begins the registration of a resource in this service.
        /// Use <see cref="EndRegisterResource"/> when the operation completed.
        /// </summary>
        /// <param name="localFile">
        ///     A local file.
        /// </param>
        /// <param name="deleteLocal">
        ///     A flag indicating whether the local file should be
        ///     deleted after being registered.
        /// </param>
        /// <param name="callback">
        ///     The asynchronous callback.
        /// </param>
        /// <param name="state">
        ///     The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="EndRegisterResource"/>.
        /// </returns>
        public IAsyncResult BeginRegisterResource(
            string localFile, bool deleteLocal, AsyncCallback callback, object state)
        {
            return this.BeginRegisterResource(
                localFile, this.Dispatcher.LocalAddress, deleteLocal, false, callback, state);
        }

        /// <summary>
        /// Ends the registration of a resource started with
        /// <see cref="BeginRegisterResource"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginRegisterResource"/>.
        /// </param>
        /// <returns>
        /// The unique resource id for the given file.
        /// </returns>
        public virtual ResourceId EndRegisterResource(IAsyncResult result)
        {
            return GetEndValue<ResourceId>(result);
        }

        /// <summary>
        /// Removes a resource from this service.
        /// Any local copies of the resource will be kept, but the
        /// service will no longer keep track of the given resource.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginRemoveResource"/> instead.
        /// </summary>
        /// <param name="id">
        /// The unique resource id.
        /// </param>
        /// <returns>
        /// True if the resource was found and successfully removed.
        /// </returns>
        public virtual bool RemoveResource(ResourceId id)
        {
            var result = this.BeginRemoveResource(id, null, null);
            return this.EndRemoveResource(result);
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
        /// An async result to be used with <see cref="EndRemoveResource"/>.
        /// </returns>
        public abstract IAsyncResult BeginRemoveResource(ResourceId id, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the removal of a resource started with
        /// <see cref="BeginRemoveResource"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginRemoveResource"/>.
        /// </param>
        /// <returns>
        /// True if the resource was found and successfully removed.
        /// </returns>
        public virtual bool EndRemoveResource(IAsyncResult result)
        {
            return GetEndValue<bool>(result);
        }

        /// <summary>
        /// Get a resource with a given unique id from this service.
        /// This method will only return when the resource is available
        /// locally.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginGetResource"/> instead.
        /// </summary>
        /// <param name="id">
        /// The unique resource id.
        /// </param>
        /// <returns>
        /// The resource information for the given resource.
        /// </returns>
        public virtual ResourceInfo GetResource(ResourceId id)
        {
            var result = this.BeginGetResource(id, null, null);
            return this.EndGetResource(result);
        }

        /// <summary>
        /// Updates the state of an existing resource.
        /// </summary>
        /// <param name="id">
        /// The resource Id.
        /// </param>
        public virtual void UpdateResource(ResourceId id)
        {
        }

        /// <summary>
        /// Begins getting a resource with a given unique id from this service.
        /// The result will only complete when the resource is available locally.
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
        /// An async result to be used with <see cref="EndGetResource"/>.
        /// </returns>
        public virtual IAsyncResult BeginGetResource(ResourceId id, AsyncCallback callback, object state)
        {
            return this.BeginGetResourceInfo(id, false, callback, state);
        }

        /// <summary>
        /// Ends getting a resource started with
        /// <see cref="BeginGetResource"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginGetResource"/>.
        /// </param>
        /// <returns>
        /// The resource information for the given resource.
        /// </returns>
        public virtual ResourceInfo EndGetResource(IAsyncResult result)
        {
            return this.EndGetResourceInfo(result);
        }

        /// <summary>
        /// Begins to get the status of a resource asynchronously.
        /// </summary>
        /// <param name="id">
        /// The resource id.
        /// </param>
        /// <param name="callback">
        /// The async callback.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="IResourceServiceImpl.EndGetResourceStatus"/>.
        /// </returns>
        public IAsyncResult BeginGetResourceStatus(ResourceId id, AsyncCallback callback, object state)
        {
            return this.BeginGetResourceInfo(id, true, callback, state);
        }

        /// <summary>
        /// Completes the asynchronous call started by <see cref="IResourceServiceImpl.BeginGetResourceStatus"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="IResourceServiceImpl.BeginGetResourceStatus"/>.
        /// </param>
        /// <returns>
        /// The status of the resource.
        /// </returns>
        public ResourceStatus EndGetResourceStatus(IAsyncResult result)
        {
            var info = this.EndGetResourceInfo(result);
            return new ResourceStatus
            {
                Id = info.Id,
                IsAvailableCompletely = info.State == ResourceState.Available,
                AvailableBytes = info.Size
            };
        }

        /// <summary>
        /// Begins adding a resource to the service being received over a Medi connection.
        /// This method can be asynchronous, but you won't be notified when the announcement was handled completely.
        /// </summary>
        /// <param name="source">
        ///     The original source of the announcement or <see cref="MediAddress.Empty"/> if unknown.
        /// </param>
        /// <param name="destination">
        ///     The original destination of the announcement or <see cref="MediAddress.Empty"/> if unknown.
        /// </param>
        /// <param name="announcement">
        ///     The resource announcement with the information about the resource.
        /// </param>
        public void AnnounceResource(MediAddress source, MediAddress destination, ResourceAnnouncement announcement)
        {
            this.AnnounceResource(source, announcement);
            this.HandleAnnouncedResource(source, destination, announcement);
        }

        /// <summary>
        /// Sends a local resource to a different node in the Medi network.
        /// The node can be local or remote. This will make sure the resource
        /// is available at the destination node. Use this method before sending
        /// a <see cref="ResourceId"/> to another node.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginSendResource"/> instead.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource"/>.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <returns>
        /// True if the resource was successfully sent out (it is not guaranteed
        /// the resource has already been delivered to the destination when
        /// this method returns.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// if the <see cref="destination"/> contains wildcards ("*").
        /// Resources can only be sent to a single Medi node.
        /// </exception>
        public virtual bool SendResource(ResourceInfo info, MediAddress destination)
        {
            return this.SendResource(info, this.Dispatcher.LocalAddress, destination, false, null);
        }

        /// <summary>
        /// Begins sending a local resource to a different node in the Medi network.
        /// See <see cref="SendResource"/> for more information.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource"/>.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="EndSendResource"/>.
        /// </returns>
        public virtual IAsyncResult BeginSendResource(
            ResourceInfo info, MediAddress destination, AsyncCallback callback, object state)
        {
            return this.BeginSendResource(
                info, this.Dispatcher.LocalAddress, destination, false, null, callback, state);
        }

        /// <summary>
        /// Ends sending a resource started with
        /// <see cref="BeginSendResource"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginSendResource"/>.
        /// </param>
        /// <returns>
        /// True if the resource was successfully sent out (it is not guaranteed
        /// the resource has already been delivered to the destination when
        /// this method returns.
        /// </returns>
        public virtual bool EndSendResource(IAsyncResult result)
        {
            return GetEndValue<bool>(result);
        }

        /// <summary>
        /// Gets a local copy of the given resource info.
        /// This will "check out" the resource, but it will still be tracked
        /// by the resource service. Do not modify or delete the file created
        /// by this method, but rather use <see cref="CheckinResource"/> to
        /// "check it in" again.
        /// If you want to modify a file or copy it to a removable device,
        /// use <see cref="ExportResource"/> instead.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginCheckoutResource"/> instead.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource"/>.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        public bool CheckoutResource(ResourceInfo info, string localFile)
        {
            this.Logger.Trace("CheckoutResource({0},{1})", info.Id, localFile);
            return this.GetLocalCopy(info, localFile, true);
        }

        /// <summary>
        /// Begins checking out the given resource.
        /// See <see cref="CheckoutResource"/> for more information.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource"/>.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="EndCheckoutResource"/>.
        /// </returns>
        public IAsyncResult BeginCheckoutResource(
            ResourceInfo info, string localFile, AsyncCallback callback, object state)
        {
            return this.BeginGetLocalCopy(info, localFile, true, callback, state);
        }

        /// <summary>
        /// Ends checking out a resource started with
        /// <see cref="BeginCheckoutResource"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginCheckoutResource"/>.
        /// </param>
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        public bool EndCheckoutResource(IAsyncResult result)
        {
            return this.EndGetLocalCopy(result);
        }

        /// <summary>
        /// Returns a file gotten by <see cref="CheckoutResource"/> to the resource service.
        /// </summary>
        /// <param name="localFile">
        /// The local file to return. The file will be deleted
        /// after it was returned to the service.
        /// </param>
        /// <seealso cref="CheckoutResource"/>
        public virtual void CheckinResource(string localFile)
        {
            this.Logger.Trace("CheckoutResource({0})", localFile);
            var result = this.BeginCheckinResource(localFile, null, null);
            this.EndCheckinResource(result);
        }

        /// <summary>
        /// Begins returning a file gotten by <see cref="CheckoutResource"/>
        /// to the resource service.
        /// </summary>
        /// <param name="localFile">
        /// The local file to return. The file will be deleted
        /// after it was returned to the service.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="EndCheckinResource"/>.
        /// </returns>
        public virtual IAsyncResult BeginCheckinResource(string localFile, AsyncCallback callback, object state)
        {
            // these are the same operations
            return this.BeginRegisterResource(localFile, true, callback, state);
        }

        /// <summary>
        /// Ends checking in a resource started with <see cref="BeginCheckinResource"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginCheckinResource"/>.
        /// </param>
        public virtual void EndCheckinResource(IAsyncResult result)
        {
            // these are the same operations
            this.EndRegisterResource(result);
        }

        /// <summary>
        /// Gets a local copy of the given resource info.
        /// This will copy the resource, and the copied file won't be tracked
        /// by the resource service. You are free to modify or delete the file created
        /// by this method.
        /// If you only want to use the file temporarily, use <see cref="CheckoutResource"/> instead.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginExportResource"/> instead.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource"/>.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        public bool ExportResource(ResourceInfo info, string localFile)
        {
            this.Logger.Trace("ExportResource({0},{1})", info.Id, localFile);
            return this.GetLocalCopy(info, localFile, false);
        }

        /// <summary>
        /// Begins exporting out the given resource.
        /// See <see cref="ExportResource"/> for more information.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource"/>.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="EndExportResource"/>.
        /// </returns>
        public IAsyncResult BeginExportResource(
            ResourceInfo info, string localFile, AsyncCallback callback, object state)
        {
            this.Logger.Trace("BeginExportResource({0},{1})", info.Id, localFile);
            return this.BeginGetLocalCopy(info, localFile, false, callback, state);
        }

        /// <summary>
        /// Ends exporting a resource started with <see cref="BeginExportResource"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginExportResource"/>.
        /// </param>
        /// <returns>
        /// True if the resource was exported successfully.
        /// </returns>
        public bool EndExportResource(IAsyncResult result)
        {
            return this.EndGetLocalCopy(result);
        }

        /// <summary>
        /// Sends a local file to a different node in the Medi network.
        /// The file will only be kept in resource management until it is sent,
        /// this is useful to transfer temporary files (e.g. log files).
        /// The node can be local or remote. This will make sure the file
        /// is available at the destination node.
        /// On the <see cref="destination"/> node, the event
        /// <see cref="IResourceService.FileReceived"/> event is fired.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="IResourceService.BeginSendFile"/> instead.
        /// </summary>
        /// <param name="localFile">
        /// The full path to a local file.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <returns>
        /// True if the file was successfully sent out (it is not guaranteed
        /// the resource has already been delivered to the destination when
        /// this method returns.
        /// </returns>
        public bool SendFile(string localFile, MediAddress destination)
        {
            this.Logger.Trace("SendFile({0},<{1}>)", localFile, destination);
            var id = this.RegisterResource(localFile, this.Dispatcher.LocalAddress, false, true);
            var info = this.GetResource(id);
            return this.SendResource(info, this.Dispatcher.LocalAddress, destination, true, localFile);
        }

        /// <summary>
        /// Begins sending a local file to a different node in the Medi network.
        /// See <see cref="IResourceService.SendFile"/> for more information.
        /// </summary>
        /// <param name="localFile">
        /// The full path to a local file.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="IResourceService.EndSendFile"/>.
        /// </returns>
        public IAsyncResult BeginSendFile(
            string localFile, MediAddress destination, AsyncCallback callback, object state)
        {
            this.Logger.Trace("BeginSendFile({0},<{1}>)", localFile, destination);
            var endResult = new SimpleAsyncResult<bool>(callback, state);
            this.BeginRegisterResource(
                localFile,
                this.Dispatcher.LocalAddress,
                false, // deleteLocal
                true, // temporary
                regRes =>
                this.BeginGetResource(
                    this.EndRegisterResource(regRes),
                    getRes =>
                    this.BeginSendResource(
                        this.EndGetResource(getRes),
                        this.Dispatcher.LocalAddress,
                        destination,
                        true,
                        localFile,
                        sendRes => endResult.Complete(this.EndSendResource(sendRes), false),
                        null),
                    null),
                null);
            return endResult;
        }

        /// <summary>
        /// Ends sending a file started with
        /// <see cref="IResourceService.BeginSendFile"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="IResourceService.BeginSendFile"/>.
        /// </param>
        /// <returns>
        /// True if the file was successfully sent out (it is not guaranteed
        /// the resource has already been delivered to the destination when
        /// this method returns.
        /// </returns>
        public bool EndSendFile(IAsyncResult result)
        {
            return GetEndValue<bool>(result);
        }

        /// <summary>
        /// Requests a given remote file from a remote system.
        /// </summary>
        /// <param name="remoteFile">
        /// The remote file.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        public virtual void RequestFile(string remoteFile, MediAddress address)
        {
            this.Dispatcher.Send(address, new RequestResourceMessage { FileName = remoteFile });
        }

        /// <summary>
        /// Indicates a new set of resources
        /// </summary>
        public virtual void BeginSet()
        {
        }

        /// <summary>
        /// Indicates the end of new set of resources
        /// </summary>
        public virtual void EndSet()
        {
        }

        /// <summary>
        /// Acquires a resource lock for a given <see cref="id"/>.
        /// A lock prevents the resource from being deleted while still in use.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ResourceLock"/>. Use <see cref="ResourceLock.Release"/>
        /// to release the lock again.
        /// </returns>
        public IResourceLock AcquireLock(ResourceId id)
        {
            this.Logger.Trace("AcquireLock({0})", id);
            List<IResourceLock> lockList;
            lock (this.locks)
            {
                if (!this.locks.TryGetValue(id, out lockList))
                {
                    lockList = new List<IResourceLock>();
                    this.locks.Add(id, lockList);
                }
            }

            var resourceLock = new ResourceLock(id, this);
            lock (lockList)
            {
                lockList.Add(resourceLock);

                if (lockList.Count == 1)
                {
                    this.AcquireFirstLock(resourceLock);
                }
            }

            return resourceLock;
        }

        /// <summary>
        /// Creates a download handler for the given id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IDownloadHandler"/>.
        /// </returns>
        public virtual IDownloadHandler CreateDownloadHandler(ResourceId id)
        {
            this.Logger.Trace("CreateDownloadHandler({0})", id);
            return new DownloadHandler(id, this);
        }

        /// <summary>
        /// Creates an upload handler for the given id.
        /// </summary>
        /// <param name="id">
        /// The resource id.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <returns>
        /// The <see cref="IUploadHandler"/>.
        /// </returns>
        public virtual IUploadHandler CreateUploadHandler(ResourceId id, MediAddress destination)
        {
            this.Logger.Trace("CreateUploadHandler({0})", id);
            return new UploadHandler(id, destination, this);
        }

        /// <summary>
        /// Starts this service.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that owns this service.
        /// </param>
        public virtual void Start(IMessageDispatcherImpl dispatcher)
        {
            if (this.Dispatcher != null)
            {
                return;
            }

            this.Dispatcher = dispatcher;

            var dir = this.Config.ResourceDirectory;

            // todo: improve replacement
            dir = dir.Replace("${AppName}", dispatcher.LocalAddress.Application);
            this.BaseDirectory = this.FileSystem.CreateDirectory(dir);

            this.Dispatcher.Subscribe<RequestResourceMessage>(this.HandleRequestResourceMessage);

            var factory = dispatcher.ManagementProviderFactory;
            this.managementProvider = factory.CreateManagementProvider(ManagementName, factory.LocalRoot, this);
            factory.LocalRoot.AddChild(this.managementProvider);
        }

        /// <summary>
        /// Stops this service.
        /// </summary>
        public virtual void Stop()
        {
            if (this.managementProvider == null)
            {
                return;
            }

            this.Dispatcher.Unsubscribe<RequestResourceMessage>(this.HandleRequestResourceMessage);

            this.managementProvider.Dispose();
            this.managementProvider = null;
        }

        /// <summary>
        /// Registers a resource in this service.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginRegisterResource"/> instead.
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
        /// <returns>
        /// The unique resource id for the given file.
        /// </returns>
        internal virtual ResourceId RegisterResource(
            string localFile, MediAddress source, bool deleteLocal, bool temporary)
        {
            var result = this.BeginRegisterResource(localFile, source, deleteLocal, temporary, null, null);
            return this.EndRegisterResource(result);
        }

        /// <summary>
        /// Begins the registration of a resource in this service.
        /// Use <see cref="EndRegisterResource"/> when the operation completed.
        /// </summary>
        /// <param name="localFile">
        ///     A local file.
        /// </param>
        /// <param name="source">
        ///     The source of the file.
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
        /// An async result to be used with <see cref="EndRegisterResource"/>.
        /// </returns>
        internal abstract IAsyncResult BeginRegisterResource(
            string localFile,
            MediAddress source,
            bool deleteLocal,
            bool temporary,
            AsyncCallback callback,
            object state);

        /// <summary>
        /// Announces a resource to the resource service.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="announcement">
        /// The announcement.
        /// </param>
        internal abstract void AnnounceResource(MediAddress source, ResourceAnnouncement announcement);

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
        /// An async result to be used with <see cref="EndGetResourceInfo"/>.
        /// </returns>
        internal abstract IAsyncResult BeginGetResourceInfo(
            ResourceId id, bool allowIncomplete, AsyncCallback callback, object state);

        /// <summary>
        /// Ends getting a resource started with
        /// <see cref="BeginGetResourceInfo"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginGetResourceInfo"/>.
        /// </param>
        /// <returns>
        /// The extended resource information for the given resource.
        /// </returns>
        internal virtual ExtendedResourceInfo EndGetResourceInfo(IAsyncResult result)
        {
            return GetEndValue<ExtendedResourceInfo>(result);
        }

        /// <summary>
        /// Gets a local copy of a resource.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource"/>.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <param name="keepTracking">
        /// A flag indicating if the resource service should continue tracking the
        /// copied file.
        /// </param>
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        internal virtual bool GetLocalCopy(ResourceInfo info, string localFile, bool keepTracking)
        {
            var result = this.BeginGetLocalCopy(info, localFile, keepTracking, null, null);
            return this.EndGetLocalCopy(result);
        }

        /// <summary>
        /// Begins getting a local copy of a resource.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource"/>.
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
        /// An async result to be used with <see cref="EndGetLocalCopy"/>.
        /// </returns>
        internal abstract IAsyncResult BeginGetLocalCopy(
            ResourceInfo info, string localFile, bool keepTracking, AsyncCallback callback, object state);

        /// <summary>
        /// Ends getting a local copy started with
        /// <see cref="BeginGetLocalCopy"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginGetLocalCopy"/>.
        /// </param>
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        internal virtual bool EndGetLocalCopy(IAsyncResult result)
        {
            return GetEndValue<bool>(result);
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
        internal abstract void SetDownloadState(ResourceId id, long downloadedBytes, string tempFilename);

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
        internal abstract void SetUploading(
            ResourceId id, MediAddress source, MediAddress destination, bool isTemporary, string originalName);

        /// <summary>
        /// Clears the upload state of a given resource.
        /// </summary>
        /// <param name="id">
        /// The id of the resource.
        /// </param>
        /// <param name="destination">
        /// The destination address where the resource was sent to.
        /// </param>
        internal abstract void ClearUploading(ResourceId id, MediAddress destination);

        /// <summary>
        /// Creates a <see cref="ResourceId"/> from the contents of a file.
        /// </summary>
        /// <param name="file">
        /// The file from which to get the hash.
        /// </param>
        /// <returns>
        /// An MD5 hash of the file contents.
        /// </returns>
        protected static ResourceId CreateId(IFileInfo file)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] result;
            using (var input = file.OpenRead())
            {
                result = md5.ComputeHash(input);
            }

            return HashToId(result);
        }

        /// <summary>
        /// Asynchronously calls a method and completes an async result.
        /// </summary>
        /// <param name="result">
        /// The result to be completed.
        /// </param>
        /// <param name="method">
        /// The method to be called.
        /// </param>
        /// <typeparam name="T">
        /// the type of the result.
        /// </typeparam>
        protected void DoAsync<T>(SimpleAsyncResult<T> result, Method<T> method)
        {
            ThreadPool.QueueUserWorkItem(
                s =>
                {
                    try
                    {
                        T retVal;
                        try
                        {
                            retVal = method();
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Debug(ex, "Coudln't execute async method");
                            result.CompleteException(ex, false);
                            return;
                        }

                        result.Complete(retVal, false);
                    }
                    catch (Exception ex)
                    {
                        var reason = "Coudln't complete async request";
                        this.Logger.Warn(reason);
                        this.Logger.Debug(ex, reason);
                    }
                });
        }

        /// <summary>
        /// Configures this service with the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        protected void Configure(ResourceServiceConfigBase config)
        {
            this.Config = config;
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
        protected abstract IResourceAccess GetResourceAccess(ExtendedResourceInfo info);

        /// <summary>
        /// Method called when a lock is acquired for the first time.
        /// </summary>
        /// <param name="resourceLock">
        /// The acquired resource lock.
        /// </param>
        protected abstract void AcquireFirstLock(IResourceLock resourceLock);

        /// <summary>
        /// Method called when a lock is finally released (all locks are released).
        /// </summary>
        /// <param name="resourceLock">
        /// The resource lock.
        /// </param>
        protected abstract void ReleaseLastLock(IResourceLock resourceLock);

        /// <summary>
        /// Checks if the given resource id is locked.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// True if somebody holds a lock on the given resource.
        /// </returns>
        protected bool IsResourceLocked(ResourceId id)
        {
            List<IResourceLock> lockList;
            lock (this.locks)
            {
                if (!this.locks.TryGetValue(id, out lockList))
                {
                    return false;
                }
            }

            return lockList.Count > 0;
        }

        /// <summary>
        /// Raises the <see cref="FileReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseFileReceived(FileReceivedEventArgs e)
        {
            var handler = this.FileReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Sends a resource to the given <paramref name="destination"/>.
        /// </summary>
        /// <param name="info">
        /// The information about the resource.
        /// </param>
        /// <param name="source">
        /// The source address from which the resource comes.
        /// </param>
        /// <param name="destination">
        /// The destination address to which the resource has to be sent.
        /// </param>
        /// <param name="isTemporary">
        /// A flag indicating if the resource transfer is temporary.
        /// </param>
        /// <param name="originalName">
        /// The original file name of the file being sent (this can be null for non-temporary transfers).
        /// </param>
        /// <returns>
        /// True if the resource was successfully sent out (this doesn't mean that
        /// the resource was also received at the destination, but just that there is a route
        /// to send the resource).
        /// </returns>
        protected virtual bool SendResource(
            ResourceInfo info, MediAddress source, MediAddress destination, bool isTemporary, string originalName)
        {
            this.Logger.Trace("SendResource({0},<{1}>,{2},'{3}')", info.Id, destination, isTemporary, originalName);
            if (destination.Application == MediAddress.Wildcard || destination.Unit == MediAddress.Wildcard)
            {
                throw new ArgumentException("Can't send a resource to a wildcard address");
            }

            var infoEx = (ExtendedResourceInfo)info;

            var announcement = new ResourceAnnouncement
            {
                Id = info.Id,
                IsTemporary = isTemporary,
                OriginalName = originalName ?? infoEx.OriginalName
            };

            if (!this.Dispatcher.Send(source, destination, announcement, null))
            {
                this.Logger.Warn("Can't send {0} to {1}, no subscription found", info.Id, destination);
                return false;
            }

            return this.ForwardResource(infoEx, source, destination, isTemporary, originalName);
        }

        private static T GetEndValue<T>(IAsyncResult result)
        {
            var res = result as SimpleAsyncResult<T>;
            if (res == null)
            {
                throw new ArgumentException("Call End method with result provided by Begin method");
            }

            if (!res.IsCompleted)
            {
                res.AsyncWaitHandle.WaitOne();
            }

            return res.Value;
        }

        private IAsyncResult BeginSendResource(
            ResourceInfo info,
            MediAddress source,
            MediAddress destination,
            bool isTemporary,
            string originalName,
            AsyncCallback callback,
            object state)
        {
            var result = new SimpleAsyncResult<bool>(callback, state);
            this.DoAsync(result, () => this.SendResource(info, source, destination, isTemporary, originalName));
            return result;
        }

        private void HandleRequestResourceMessage(object sender, MessageEventArgs<RequestResourceMessage> e)
        {
            this.BeginSendFile(
                e.Message.FileName,
                e.Source,
                ar =>
                {
                    try
                    {
                        if (!this.EndSendFile(ar))
                        {
                            this.Logger.Warn("Couldn't send file " + e.Message.FileName + " to " + e.Source);
                        }
                    }
                    catch (Exception ex)
                    {
                        var reason = "Couldn't send file " + e.Message.FileName + " to " + e.Source;
                        this.Logger.Warn(reason);
                        this.Logger.Debug(ex, reason);
                    }
                },
                null);
        }

        private void HandleAnnouncedResource(
            MediAddress source, MediAddress destination, ResourceAnnouncement announcement)
        {
            var isLocal = SessionIds.Local.Equals(this.Dispatcher.RoutingTable.GetSessionId(destination));

            if (isLocal && !announcement.IsTemporary)
            {
                // we don't have to do anything with this announcement
                return;
            }

            this.HandleAnnouncedResource(source, destination, announcement, isLocal, 0);
        }

        private void HandleAnnouncedResource(
            MediAddress source,
            MediAddress destination,
            ResourceAnnouncement announcement,
            bool isLocal,
            int counter)
        {
            this.Logger.Trace(
                "HandleAnnouncedResource(<{0}>,<{1}>,{2},{3},{4})",
                source,
                destination,
                announcement,
                isLocal,
                counter);

            var resourceLock = this.AcquireLock(announcement.Id);
            this.BeginGetResourceInfo(
                announcement.Id,
                false,
                ar =>
                {
                    ExtendedResourceInfo info;
                    try
                    {
                        info = this.EndGetResourceInfo(ar);
                    }
                    catch (Exception ex)
                    {
                        if (++counter > 3)
                        {
                            var reason = string.Format(
                                "Couldn't get resource information for {0}, aborting",
                                announcement.Id);
                            this.Logger.Warn(reason);
                            this.Logger.Debug(ex, reason);
                            return;
                        }

                        var message =
                            string.Format(
                                "Couldn't get resource information for {0}, retrying once more ({1})",
                                announcement.Id,
                                counter);

                        this.Logger.Info(message);
                        this.Logger.Debug(ex, message);

                        // I'm sorry, this recursion here is very ugly, but in fact we are
                        // just trying to call "GetResourceInfo" a second time if we fail the first time.
                        // This usually happens when the resource announcement has not yet arrived at the
                        // remote resource service.
                        this.HandleAnnouncedResource(source, destination, announcement, isLocal, counter);
                        resourceLock.Release();
                        return;
                    }

                    this.HandleAnnouncedResource(source, destination, announcement, isLocal, info, resourceLock);
                },
                null);
        }

        private void HandleAnnouncedResource(
            MediAddress source,
            MediAddress destination,
            ResourceAnnouncement announcement,
            bool isLocal,
            ExtendedResourceInfo info,
            IResourceLock resourceLock)
        {
            if (!isLocal)
            {
                // the announcement is for somebody else,
                // so we have to forward the resource to the next peer
                try
                {
                    this.ForwardResource(
                        info, source, destination, announcement.IsTemporary, announcement.OriginalName);
                }
                finally
                {
                    resourceLock.Release();
                }

                return;
            }

            // it is a temporary transfer and for us, so let everybody know when we got the file
            try
            {
                var resource = this.GetResourceAccess(info);
                this.RaiseFileReceived(new FileReceivedEventArgsImpl(source, announcement.OriginalName, resource));
            }
            finally
            {
                resourceLock.Release();
            }

            if (info.IsTemporary)
            {
                this.BeginRemoveResource(info.Id, asyncResult => this.EndRemoveResource(asyncResult), null);
            }
        }

        private bool ForwardResource(
            ExtendedResourceInfo info,
            MediAddress source,
            MediAddress destination,
            bool isTemporary,
            string originalName)
        {
            this.Logger.Trace("ForwardResource({0},<{1}>)", info.Id, destination);
            var header = new StreamHeader
            {
                Source = this.Dispatcher.LocalAddress,
                Destination = destination,
                Hash = info.Id.Hash,
                Offset = 0,
                Length = info.Size
            };
            var resource = this.GetResourceAccess(info);
            var resourceLock = this.AcquireLock(info.Id);
            this.SetUploading(info.Id, source, destination, isTemporary, originalName);
            var streamMessage = new ResourceStreamMessage(header, resource, resourceLock);
            return this.Dispatcher.Send(this.Dispatcher.LocalAddress, destination, streamMessage, null);
        }

        private void ReleaseLock(ResourceLock resourceLock)
        {
            this.Logger.Trace("ReleaseLock({0})", resourceLock.Id);
            List<IResourceLock> lockList;
            lock (this.locks)
            {
                if (!this.locks.TryGetValue(resourceLock.Id, out lockList))
                {
                    return;
                }
            }

            lock (lockList)
            {
                lockList.Remove(resourceLock);

                if (lockList.Count == 0)
                {
                    this.ReleaseLastLock(resourceLock);
                }
            }
        }

        private class ResourceLock : IResourceLock
        {
            private readonly ResourceServiceBase service;

            private volatile bool released;

            public ResourceLock(ResourceId id, ResourceServiceBase service)
            {
                this.service = service;
                this.Id = id;
            }

            public ResourceId Id { get; private set; }

            public void Release()
            {
                if (this.released)
                {
                    return;
                }

                lock (this)
                {
                    if (!this.released)
                    {
                        this.service.ReleaseLock(this);
                        this.released = true;
                    }
                }
            }

            void IDisposable.Dispose()
            {
                this.Release();
            }
        }
    }
}