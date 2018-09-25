// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Resource service that has a local directory and a XML-file-based database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Common.Utility.Core.Factory;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Resource service that has a local directory and a XML-file-based database.
    /// </summary>
    internal class LocalResourceService
        : ResourceServiceBase, IConfigurable<LocalResourceServiceConfig>, IManageableTable
    {
        private readonly List<ResourceId> resourcesToDelete = new List<ResourceId>();

        private IResourceDataStore resourceDataStore;

        private ResourceMessageHandler handler;

        private LocalResourceServiceConfig config;

        private IResourceStore resourceStore;

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="serviceConfig">
        /// The config object.
        /// </param>
        public void Configure(LocalResourceServiceConfig serviceConfig)
        {
            this.config = serviceConfig;
            this.Configure((ResourceServiceConfigBase)serviceConfig);
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

            this.resourceDataStore =
                        ConfigImplementationFactory.CreateFromConfig<IResourceDataStore>(
                            this.config.DataStore ?? new FileResourceDataStoreConfig());

            this.resourceStore =
                ConfigImplementationFactory.CreateFromConfig<IResourceStore>(
                    this.config.ResourceStore ?? new FileResourceStoreConfig());

            lock (this.resourceDataStore)
            {
                lock (this.resourceStore)
                {
                    this.resourceDataStore.Initialize(this.BaseDirectory, this.config.MaxSizeMb);

                    this.resourceStore.Initialize(this.BaseDirectory);

                    foreach (var resource in new List<IStoredResource>(this.resourceDataStore.GetAll()))
                    {
                        if (resource.State == ResourceState.Deleting)
                        {
                            this.Logger.Debug(
                                "Resource {0} is in state {1}, removing the resource",
                                resource.Id,
                                resource.State);
                            this.RemoveResource(resource.Id);
                            continue;
                        }

                        if (!this.resourceStore.IsAvailable(resource))
                        {
                            this.RemoveResource(resource.Id);
                        }
                    }
                }
            }

            if (this.handler == null)
            {
                this.handler = new ResourceMessageHandler(this);
            }

            this.handler.Start();

            this.Dispatcher.RoutingTable.Updated += this.RoutingTableOnUpdated;

            this.ResendResources();
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

            this.Dispatcher.RoutingTable.Updated -= this.RoutingTableOnUpdated;

            base.Stop();

            this.handler.Stop();

            lock (this.resourceStore)
            {
                this.resourceStore.Dispose();
                this.resourceStore = null;
            }

            lock (this.resourceDataStore)
            {
                this.resourceDataStore.Dispose();
                this.resourceDataStore = null;
            }
        }

        /// <summary>
        /// Registers a resource in this service.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="ResourceServiceBase.BeginRegisterResource"/> instead.
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
        public override ResourceId RegisterResource(string localFile, bool deleteLocal)
        {
            return this.RegisterResource(localFile, this.Dispatcher.LocalAddress, deleteLocal, false);
        }

        /// <summary>
        /// Removes a resource from this service.
        /// Any local copies of the resource will be kept, but the
        /// service will no longer keep track of the given resource.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="ResourceServiceBase.BeginRemoveResource"/> instead.
        /// </summary>
        /// <param name="id">
        /// The unique resource id.
        /// </param>
        /// <returns>
        /// True if the resource was found and successfully removed.
        /// </returns>
        public override bool RemoveResource(ResourceId id)
        {
            this.Logger.Trace("RemoveResource({0})", id);
            IStoredResource resource;
            lock (this.resourceDataStore)
            {
                resource = this.resourceDataStore.Get(id);
                if (resource == null)
                {
                    return false;
                }
            }

            if (this.IsResourceLocked(id))
            {
                // ok, this resource is locked, we can only remove it later (when the last lock was removed)
                lock (this.resourcesToDelete)
                {
                    this.Logger.Debug("Trying to remove a locked resource {0}, enqueueing it for later removal", id);
                    this.resourcesToDelete.Add(id);
                }

                lock (resource)
                {
                    resource.State = ResourceState.Deleting;
                    this.resourceDataStore.Update(resource);
                }

                // we lie a little bit, we will only remove it later, but that's ok
                return true;
            }

            lock (this.resourceDataStore)
            {
                lock (resource)
                {
                    this.resourceDataStore.Remove(resource);
                }
            }

            lock (resource)
            {
                this.resourceStore.Remove(resource);
            }

            return true;
        }

        /// <summary>
        /// Indicates a new set of resources
        /// </summary>
        public override void BeginSet()
        {
            if (this.resourceDataStore != null)
            {
                lock (this.resourceDataStore)
                {
                    this.resourceDataStore.BeginSet();
                }
            }
        }

        /// <summary>
        /// Indicates the end of new set of resources
        /// </summary>
        public override void EndSet()
        {
            if (this.resourceDataStore != null)
            {
                lock (this.resourceDataStore)
                {
                    this.resourceDataStore.EndSet();
                }
            }
        }

        /// <summary>
        /// Updates the state of an existing resource.
        /// </summary>
        /// <param name="id">
        /// The resource Id.
        /// </param>
        public override void UpdateResource(ResourceId id)
        {
            IStoredResource resource;
            lock (this.resourceDataStore)
            {
                resource = this.resourceDataStore.Get(id);
                if (resource != null)
                {
                    lock (resource)
                    {
                        this.resourceDataStore.Update(resource);
                    }
                }
            }
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
        /// An async result to be used with <see cref="ResourceServiceBase.EndRemoveResource"/>.
        /// </returns>
        public override IAsyncResult BeginRemoveResource(ResourceId id, AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<bool>(callback, state);
            this.DoAsync(result, () => this.RemoveResource(id));
            return result;
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var resource in this.resourceDataStore.GetAll())
            {
                var references = string.Join(";", resource.References.ToArray());
                yield return new List<ManagementProperty>
                    {
                        new ManagementProperty<string>("Id", resource.Id.ToString(), true),
                        new ManagementProperty<long>("Size", resource.Size, true),
                        new ManagementProperty<string>("State", resource.State.ToString(), true),
                        new ManagementProperty<string>("StoreReference", resource.StoreReference, true),
                        new ManagementProperty<string>("References", references, true)
                    };
            }
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
            try
            {
                IStoredResource resource;
                lock (this.resourceDataStore)
                {
                    resource = this.resourceDataStore.Get(announcement.Id);
                    if (resource == null)
                    {
                        resource = this.resourceDataStore.Create(announcement.Id);
                        resource.State = ResourceState.Announced;
                        resource.Source = source;
                        resource.OriginalFileName = announcement.OriginalName;
                        resource.IsTemporary = announcement.IsTemporary;

                        this.resourceDataStore.Add(resource);
                    }
                }

                lock (resource)
                {
                    var update = false;
                    if (resource.IsTemporary && !announcement.IsTemporary)
                    {
                        // upgrade to normal resource
                        resource.IsTemporary = false;
                        update = true;
                    }

                    if (resource.Source == null)
                    {
                        resource.Source = source;
                        update = true;
                    }

                    if (update)
                    {
                        this.resourceDataStore.Update(resource);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = "Couldn't announce resource";
                this.Logger.Warn(message);
                this.Logger.Debug("{0} - {1}", message, ex);
            }
        }

        /// <summary>
        /// Registers a resource in this service.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="ResourceServiceBase.BeginRegisterResource"/> instead.
        /// </summary>
        /// <param name="localFile">
        ///     A local file.
        /// </param>
        /// <param name="source">
        ///     The source of the resource.
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
        internal override ResourceId RegisterResource(
            string localFile, MediAddress source, bool deleteLocal, bool temporary)
        {
            this.Logger.Trace("RegisterResource({0},<{1}>,{2},{3})", localFile, source, deleteLocal, temporary);
            var localFileInfo = this.FileSystem.GetFile(localFile);
            var id = CreateId(localFileInfo);
            this.Logger.Trace("Computed id {0} for {1}", id, localFile);
            IStoredResource resource;
            lock (this.resourceDataStore)
            {
                resource = this.resourceDataStore.Get(id);
                if (resource == null)
                {
                    this.Logger.Trace("Resource {0} doesn't exist yet, creating it", id);
                    resource = this.resourceDataStore.Create(id);
                    resource.IsTemporary = temporary;
                    this.RegisterResource(resource, localFileInfo, source, deleteLocal);
                    this.resourceDataStore.Add(resource);
                    return id;
                }
            }

            lock (resource)
            {
                this.Logger.Trace("Found resource {0} in state {1}, updating it", id, resource.State);

                if (resource.State == ResourceState.Announced || resource.State == ResourceState.Downloading)
                {
                    if (resource.IsTemporary && !temporary)
                    {
                        // upgrade from temporary to non-temporary
                        resource.IsTemporary = false;
                    }

                    this.RegisterResource(resource, localFileInfo, source, deleteLocal);
                    this.resourceDataStore.Update(resource);
                }
                else if (deleteLocal)
                {
                    this.ReturnLocalCopy(resource, localFileInfo);
                }
            }

            return id;
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
            var result = new SimpleAsyncResult<ResourceId>(callback, state);
            this.DoAsync(result, () => this.RegisterResource(localFile, source, deleteLocal, temporary));
            return result;
        }

        /// <summary>
        /// Begins getting a resource with a given unique id from this service.
        /// The result will only complete when the resource is available locally.
        /// </summary>
        /// <param name="id">
        ///     The unique resource id.
        /// </param>
        /// <param name="allowIncomplete">
        /// Flag indicating if an incomplete resource info is allowed.
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
            var result = new SimpleAsyncResult<ExtendedResourceInfo>(callback, state);
            try
            {
                IStoredResource resource;
                lock (this.resourceDataStore)
                {
                    resource = this.resourceDataStore.Get(id);
                    if (resource == null)
                    {
                        throw new KeyNotFoundException("Couldn't find resource " + id);
                    }
                }

                lock (resource)
                {
                    if (resource.State != ResourceState.Available && !allowIncomplete)
                    {
                        // we have to wait for the resource to be on this machine first
                        resource.StateChanged += (sender, args) =>
                        {
                            lock (resource)
                            {
                                if (!result.IsCompleted && resource.State == ResourceState.Available)
                                {
                                    result.Complete(CreateResourceInfo(resource), false);
                                }
                            }
                        };
                        return result;
                    }
                }

                result.Complete(CreateResourceInfo(resource), true);
            }
            catch (Exception ex)
            {
                if (!result.TryCompleteException(
                        new ApplicationException("Couldn't get the local resource info for " + id, ex), true))
                {
                    var message = "Couldn't get the local resource info for " + id;
                    this.Logger.Warn(message);
                    this.Logger.Debug("{0} - {1}", message, ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a local copy of a resource.
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
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        internal override bool GetLocalCopy(ResourceInfo info, string localFile, bool keepTracking)
        {
            this.Logger.Trace("GetLocalCopy({0},{1},{2})", info.Id, localFile, keepTracking);
            IStoredResource resource;
            lock (this.resourceDataStore)
            {
                resource = this.resourceDataStore.Get(info.Id);
            }

            if (resource == null)
            {
                return false;
            }

            if (!this.GetLocalCopy(resource, localFile, keepTracking))
            {
                return false;
            }

            if (keepTracking)
            {
                lock (resource)
                {
                    resource.References.Add(localFile);
                    this.resourceDataStore.Update(resource);
                }
            }

            return true;
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
            var result = new SimpleAsyncResult<bool>(callback, state);
            this.DoAsync(result, () => this.GetLocalCopy(info, localFile, keepTracking));
            return result;
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
            IStoredResource resource;
            lock (this.resourceDataStore)
            {
                resource = this.resourceDataStore.Get(id);
                if (resource == null)
                {
                    throw new KeyNotFoundException("Couldn't find resource " + id);
                }
            }

            lock (resource)
            {
                resource.State = ResourceState.Downloading;
                resource.Size = downloadedBytes;
                if (resource.StoreReference == null)
                {
                    resource.StoreReference = tempFilename;
                }

                this.resourceDataStore.Update(resource);
            }
        }

        /// <summary>
        /// Sets the upload state of a given resource.
        /// </summary>
        /// <param name="id">
        /// The id.
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
            ResourceId id, MediAddress source, MediAddress destination, bool isTemporary, string originalName)
        {
            this.Logger.Trace("SetUploading({0},<{1}>,<{2}>,{3})", id, source, destination, isTemporary);
            IStoredResource resource;
            lock (this.resourceDataStore)
            {
                resource = this.resourceDataStore.Get(id);
                if (resource == null)
                {
                    throw new KeyNotFoundException("Couldn't find resource " + id);
                }
            }

            var transfer = new ResourceTransfer
            {
                Source = source,
                Destination = destination,
                IsTemporary = isTemporary,
                OriginalName = originalName
            };

            lock (resource)
            {
                if (resource.Transfers.Contains(transfer))
                {
                    return;
                }

                resource.Transfers.Add(transfer);
                this.Logger.Debug("Added transfer to {0}: <{1}> --> <{2}>", id, source, destination);

                this.resourceDataStore.Update(resource);
            }
        }

        /// <summary>
        /// Clears the upload state of a given resource.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="destination">
        /// The destination address where the resource is being sent to.
        /// </param>
        internal override void ClearUploading(ResourceId id, MediAddress destination)
        {
            this.Logger.Trace("ClearUploading({0},<{1}>)", id, destination);
            IStoredResource resource;
            lock (this.resourceDataStore)
            {
                resource = this.resourceDataStore.Get(id);
            }

            if (resource == null)
            {
                this.Logger.Debug("Couldn't find resource to clear uploading flag: " + id);
                return;
            }

            lock (resource)
            {
                var transfer = resource.Transfers.Find(t => t.Destination.Equals(destination));
                if (transfer == null)
                {
                    return;
                }

                this.Logger.Debug("Removed transfer from {0}: <{1}> --> <{2}>", id, transfer.Source, destination);
                resource.Transfers.Remove(transfer);
                this.resourceDataStore.Update(resource);
            }
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
            IStoredResource resource;
            lock (this.resourceDataStore)
            {
                resource = this.resourceDataStore.Get(info.Id);
            }

            return this.resourceStore.GetResourceAccess(resource);
        }

        /// <summary>
        /// Method called when a lock is acquired for the first time.
        /// </summary>
        /// <param name="resourceLock">
        /// The acquired resource lock.
        /// </param>
        protected override void AcquireFirstLock(IResourceLock resourceLock)
        {
            // nothing to do
            this.Logger.Trace("AcquireFirstLock({0})", resourceLock.Id);
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

            lock (this.resourcesToDelete)
            {
                if (!this.resourcesToDelete.Remove(resourceLock.Id))
                {
                    // ok, nobody wanted to delete the resource
                    return;
                }
            }

            this.Logger.Debug("Resource {0} unlocked, now we can remove it", resourceLock.Id);

            // do this in a separate thread to prevent eventual deadlocks on our many locks
            this.BeginRemoveResource(resourceLock.Id, ar => this.EndRemoveResource(ar), null);
        }

        private static ExtendedResourceInfo CreateResourceInfo(IStoredResource resource)
        {
            var path = resource.State == ResourceState.Available
                           ? resource.StoreReference ?? resource.References[0]
                           : null;
            return new ExtendedResourceInfo
            {
                Id = resource.Id,
                Size = resource.Size,
                LocalPath = path,
                State = resource.State,
                IsTemporary = resource.IsTemporary,
                OriginalName = resource.OriginalFileName
            };
        }

        private void RegisterResource(
            IStoredResource resource, IWritableFileInfo localFileInfo, MediAddress source, bool deleteLocal)
        {
            // this needs to be done before moving the localFileInfo (the reference becomes invalid afterwards)
            if (resource.OriginalFileName == null)
            {
                resource.OriginalFileName = localFileInfo.FullName;
            }

            resource.Size = localFileInfo.Size;
            this.resourceStore.Add(resource, localFileInfo, deleteLocal);

            if (resource.Source == null)
            {
                resource.Source = source;
            }

            // Important: this has to happen last since it will trigger the
            // StateChanged event and that means the object must be completed
            resource.State = ResourceState.Available;
        }

        private bool GetLocalCopy(IStoredResource resource, string localFile, bool keepTracking)
        {
            IWritableFileInfo oldLocalFile;
            if (this.FileSystem.TryGetFile(localFile, out oldLocalFile))
            {
                try
                {
                    // try to delete the local file if it exists
                    oldLocalFile.Attributes = FileAttributes.Normal;
                    oldLocalFile.Delete();
                }
                catch (IOException)
                {
                }
            }

            lock (resource)
            {
                return this.resourceStore.GetLocalCopy(resource, localFile, keepTracking);
            }
        }

        private void ReturnLocalCopy(IStoredResource resource, IWritableFileInfo localFile)
        {
            lock (resource)
            {
                if (localFile.FullName.Equals(resource.StoreReference, StringComparison.InvariantCultureIgnoreCase))
                {
                    // ignore if we want to return the store reference
                    this.Logger.Debug("Returning the store reference of {0}:{1}", resource.Id, resource.StoreReference);
                    return;
                }

                if (resource.References.Find(
                    f => f.Equals(localFile.FullName, StringComparison.InvariantCultureIgnoreCase)) == null)
                {
                    // ignore if we don't know that file
                    this.Logger.Debug("Returning an unknown resource: {0}", resource.Id);
                    return;
                }

                // remove the file from the references if we had it referenced
                foreach (var reference in resource.References)
                {
                    if (reference.Equals(localFile.FullName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        resource.References.Remove(reference);
                        break;
                    }
                }

                this.resourceStore.ReturnLocalCopy(resource, localFile);

                this.resourceDataStore.Update(resource);
            }
        }

        private void RoutingTableOnUpdated(object sender, RouteUpdatesEventArgs e)
        {
            var updates = new List<RouteUpdate>(e.Updates);
            var resources = this.GetResourcesToResend(d => updates.Find(u => u.Added && u.Address.Equals(d)) != null);

            this.Logger.Debug("Found {0} resources to be resent through {1}", resources.Count, e.SessionId);
            this.ResendResources(resources);
        }

        private void ResendResources()
        {
            // resend resources to destinations that are already in the routing table
            var resources = this.GetResourcesToResend(d => this.Dispatcher.RoutingTable.GetSessionId(d) != null);

            this.ResendResources(resources);
        }

        private IDictionary<MediAddress, List<ResendResourceInfo>> GetResourcesToResend(Predicate<MediAddress> filter)
        {
            var resources = new Dictionary<MediAddress, List<ResendResourceInfo>>();
            lock (this.resourceDataStore)
            {
                foreach (var storedResource in this.resourceDataStore.GetAll())
                {
                    foreach (var transfer in storedResource.Transfers)
                    {
                        List<ResendResourceInfo> sendResources;
                        if (!resources.TryGetValue(transfer.Destination, out sendResources))
                        {
                            if (!filter(transfer.Destination))
                            {
                                continue;
                            }

                            sendResources = new List<ResendResourceInfo>();
                            resources.Add(transfer.Destination, sendResources);
                        }
                        else if (sendResources.Find(i => i.Info.Id.Equals(storedResource.Id)) != null)
                        {
                            // multiple entries for the same resource and destination
                            continue;
                        }

                        this.Logger.Trace(
                            "Found resource {0} to be resent to {1}", storedResource.Id, transfer.Destination);
                        sendResources.Add(
                            new ResendResourceInfo(
                                this.EndGetResourceInfo(this.BeginGetResourceInfo(storedResource.Id, true, null, null)),
                                transfer));
                    }
                }
            }

            return resources;
        }

        private void ResendResources(IDictionary<MediAddress, List<ResendResourceInfo>> resources)
        {
            if (resources.Count == 0)
            {
                return;
            }

            this.Logger.Debug("Resending {0} resources", resources.Count);
            var toRemove = new List<MediAddress>();
            foreach (var resourcePair in resources)
            {
                if (this.ResendResources(resourcePair.Key, resourcePair.Value))
                {
                    toRemove.Add(resourcePair.Key);
                }
            }

            foreach (var address in toRemove)
            {
                resources.Remove(address);
            }

            if (resources.Count == 0)
            {
                return;
            }

            var timer = TimerFactory.Current.CreateTimer("ResendResources");
            timer.AutoReset = false;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Elapsed += (s, e) => this.ResendResources(resources);
            timer.Enabled = true;
        }

        private bool ResendResources(MediAddress destination, IList<ResendResourceInfo> resources)
        {
            if (SessionIds.Local.Equals(this.Dispatcher.RoutingTable.GetSessionId(destination)))
            {
                this.Logger.Debug("Not sending resources to myself");
                foreach (var resource in resources)
                {
                    this.Logger.Trace(" - {0}", resource.Info.Id);
                }

                return true;
            }

            for (int i = resources.Count - 1; i >= 0; i--)
            {
                var resource = resources[i];
                if (!this.SendResource(
                        resource.Info,
                        resource.Transfer.Source,
                        destination,
                        resource.Transfer.IsTemporary,
                        resource.Transfer.OriginalName))
                {
                    return false;
                }

                resources.RemoveAt(i);
            }

            return true;
        }

        private class ResendResourceInfo
        {
            public ResendResourceInfo(ResourceInfo info, ResourceTransfer transfer)
            {
                this.Info = info;
                this.Transfer = transfer;
            }

            public ResourceInfo Info { get; private set; }

            public ResourceTransfer Transfer { get; private set; }
        }
    }
}