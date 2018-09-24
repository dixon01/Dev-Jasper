// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using System;

    using Gorba.Common.Medi.Core.Services;

    /// <summary>
    /// The resource service interface.
    /// Use <see cref="MessageDispatcher.GetService{T}"/> to get the current
    /// instance for this Medi node.
    /// </summary>
    public interface IResourceService : IService
    {
        /// <summary>
        /// Event that is fired whenever a file is received.
        /// This event is triggered by a remote node using
        /// <see cref="SendFile"/> (or its asynchronous counterpart).
        /// </summary>
        event EventHandler<FileReceivedEventArgs> FileReceived;

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
        ResourceId RegisterResource(string localFile, bool deleteLocal);

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
        IAsyncResult BeginRegisterResource(string localFile, bool deleteLocal, AsyncCallback callback, object state);

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
        ResourceId EndRegisterResource(IAsyncResult result);

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
        bool RemoveResource(ResourceId id);

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
        IAsyncResult BeginRemoveResource(ResourceId id, AsyncCallback callback, object state);

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
        bool EndRemoveResource(IAsyncResult result);

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
        ResourceInfo GetResource(ResourceId id);

        /// <summary>
        /// Updates the state of an existing resource.
        /// </summary>
        /// <param name="id">
        /// The resource Id.
        /// </param>
        void UpdateResource(ResourceId id);

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
        IAsyncResult BeginGetResource(ResourceId id, AsyncCallback callback, object state);

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
        ResourceInfo EndGetResource(IAsyncResult result);

        /// <summary>
        /// Sends a local resource to a different node in the Medi network.
        /// The node can be local or remote. This will make sure the resource
        /// is available at the destination node. Use this method before sending
        /// a <see cref="ResourceId"/> to another node.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginSendResource"/> instead.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource(ResourceId)"/>.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <returns>
        /// True if the resource was successfully sent out (it is not guaranteed
        /// the resource has already been delivered to the destination when
        /// this method returns.
        /// </returns>
        bool SendResource(ResourceInfo info, MediAddress destination);

        /// <summary>
        /// Begins sending a local resource to a different node in the Medi network.
        /// See <see cref="SendResource(ResourceInfo,MediAddress)"/> for more information.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource(ResourceId)"/>.
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
        IAsyncResult BeginSendResource(
            ResourceInfo info, MediAddress destination, AsyncCallback callback, object state);

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
        bool EndSendResource(IAsyncResult result);

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
        /// The resource information returned by <see cref="GetResource(ResourceId)"/>.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        bool CheckoutResource(ResourceInfo info, string localFile);

        /// <summary>
        /// Begins checking out the given resource.
        /// See <see cref="CheckoutResource"/> for more information.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource(ResourceId)"/>.
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
        IAsyncResult BeginCheckoutResource(ResourceInfo info, string localFile, AsyncCallback callback, object state);

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
        bool EndCheckoutResource(IAsyncResult result);

        /// <summary>
        /// Returns a file gotten by <see cref="CheckoutResource"/> to the resource service.
        /// </summary>
        /// <param name="localFile">
        /// The local file to return. The file will be deleted
        /// after it was returned to the service.
        /// </param>
        /// <seealso cref="CheckoutResource"/>
        void CheckinResource(string localFile);

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
        IAsyncResult BeginCheckinResource(string localFile, AsyncCallback callback, object state);

        /// <summary>
        /// Ends checking in a resource started with <see cref="BeginCheckinResource"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginCheckinResource"/>.
        /// </param>
        void EndCheckinResource(IAsyncResult result);

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
        /// The resource information returned by <see cref="GetResource(ResourceId)"/>.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        bool ExportResource(ResourceInfo info, string localFile);

        /// <summary>
        /// Begins exporting out the given resource.
        /// See <see cref="ExportResource"/> for more information.
        /// </summary>
        /// <param name="info">
        /// The resource information returned by <see cref="GetResource(ResourceId)"/>.
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
        IAsyncResult BeginExportResource(ResourceInfo info, string localFile, AsyncCallback callback, object state);

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
        bool EndExportResource(IAsyncResult result);

        /// <summary>
        /// Sends a local file to a different node in the Medi network.
        /// The file will only be kept in resource management until it is sent,
        /// this is useful to transfer temporary files (e.g. log files).
        /// The node can be local or remote. This will make sure the file
        /// is available at the destination node.
        /// On the <see cref="destination"/> node, the event
        /// <see cref="FileReceived"/> event is fired.
        /// This is a long-running method, consider using the asynchronous
        /// <see cref="BeginSendFile"/> instead.
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
        bool SendFile(string localFile, MediAddress destination);

        /// <summary>
        /// Begins sending a local file to a different node in the Medi network.
        /// See <see cref="SendFile(string,MediAddress)"/> for more information.
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
        /// An async result to be used with <see cref="EndSendFile"/>.
        /// </returns>
        IAsyncResult BeginSendFile(string localFile, MediAddress destination, AsyncCallback callback, object state);

        /// <summary>
        /// Ends sending a file started with <see cref="BeginSendFile"/>.
        /// This method blocks until the operation has completed.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginSendFile"/>.
        /// </param>
        /// <returns>
        /// True if the file was successfully sent out (it is not guaranteed
        /// the resource has already been delivered to the destination when
        /// this method returns.
        /// </returns>
        bool EndSendFile(IAsyncResult result);

        /// <summary>
        /// Requests a given remote file from a remote system.
        /// </summary>
        /// <param name="remoteFile">
        /// The remote file.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        void RequestFile(string remoteFile, MediAddress address);

        /// <summary>
        /// Indicates a new set of resources
        /// </summary>
        void BeginSet();

        /// <summary>
        /// Indicates the end of new set of resources
        /// </summary>
        void EndSet();
    }
}