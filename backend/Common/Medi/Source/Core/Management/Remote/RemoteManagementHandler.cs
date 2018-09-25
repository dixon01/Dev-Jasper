// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteManagementHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteManagementHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Remote
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Utility.Core.Async;

    /// <summary>
    /// Helper class for <see cref="IRemoteManagementProviderImpl"/> implementations.
    /// </summary>
    internal class RemoteManagementHandler : IDisposable
    {
        private const int LoadTimeout = 10000;

        private readonly IMessageDispatcher messageDispatcher;

        private readonly object locker = new object();
        private readonly List<AsyncResult> requests = new List<AsyncResult>();

        private readonly List<string> path;

        private LoadState loadState = LoadState.NotLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteManagementHandler"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher.
        /// </param>
        /// <param name="address">
        /// The remote address.
        /// </param>
        /// <param name="path">
        /// The path that is represented by this handler.
        /// </param>
        public RemoteManagementHandler(IMessageDispatcher messageDispatcher, MediAddress address, params string[] path)
        {
            this.messageDispatcher = messageDispatcher;
            this.path = new List<string>(path);
            this.Address = address;

            this.messageDispatcher.Subscribe<ManagementResponse>(this.HandleManagementResponse);
        }

        /// <summary>
        /// Event that is fired when a response is received for this handler.
        /// </summary>
        public event EventHandler<MessageEventArgs<ManagementResponse>> ReceivedResponse;

        private enum LoadState
        {
            NotLoaded,
            Loading,
            Loaded,
            Reloading,
        }

        /// <summary>
        /// Gets the address of the remote host.
        /// </summary>
        public MediAddress Address { get; private set; }

        /// <summary>
        /// Gets the path elements of this handler.
        /// </summary>
        public string[] Path
        {
            get
            {
                return this.path.ToArray();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.messageDispatcher.Unsubscribe<ManagementResponse>(this.HandleManagementResponse);
        }

        /// <summary>
        /// Reloads the cached management information from the remote host.
        /// </summary>
        public void Reload()
        {
            this.Load(true);
        }

        /// <summary>
        /// Remotely loads the data if needed.
        /// </summary>
        /// <param name="force">
        /// If set to true any cached data will be ignored.
        /// </param>
        /// <exception cref="TimeoutException">
        /// if no response is received within 10 seconds.
        /// </exception>
        public void Load(bool force)
        {
            var result = this.BeginLoad(force, null, null, null);
            if (!result.AsyncWaitHandle.WaitOne(LoadTimeout, false))
            {
                throw new TimeoutException("No response received");
            }

            this.EndLoad(result);
        }

        /// <summary>
        /// Begins to remotely load the data if needed.
        /// </summary>
        /// <param name="force">
        /// If set to true any cached data will be ignored.
        /// </param>
        /// <param name="returnValue">
        /// The value to be returned by <see cref="EndLoad"/>
        /// </param>
        /// <param name="callback">
        /// The async callback that is called when loading has completed.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The async result to be used with <see cref="EndLoad"/>.
        /// </returns>
        public IAsyncResult BeginLoad(bool force, object returnValue, AsyncCallback callback, object state)
        {
            bool sendRequest;

            var result = new AsyncResult(returnValue, callback, state); 
            lock (this.locker)
            {
                switch (this.loadState)
                {
                    case LoadState.Loading:
                        sendRequest = false;
                        break;
                    case LoadState.Loaded:
                        if (!force)
                        {
                            result.CompleteSynchronously();
                            return result;
                        }

                        this.loadState = LoadState.Reloading;
                        sendRequest = true;
                        break;
                    case LoadState.Reloading:
                        result.CompleteSynchronously();
                        return result;
                    case LoadState.NotLoaded:
                        this.loadState = LoadState.Loading;
                        sendRequest = true;
                        break;
                    default:
                        result.CompleteSynchronously();
                        return result;
                }

                this.requests.Add(result);
            }

            if (sendRequest)
            {
                this.messageDispatcher.Send(
                    this.Address, new ManagementRequest { Id = this.GetHashCode(), Path = this.path.ToArray() });
            }

            return result;
        }

        /// <summary>
        /// Ends the asynchronous request to load the data.
        /// </summary>
        /// <param name="result">
        /// The result returned from <see cref="BeginLoad"/>.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> given to <see cref="BeginLoad"/>.
        /// </returns>
        public object EndLoad(IAsyncResult result)
        {
            var ar = result as AsyncResult;
            if (ar == null)
            {
                throw new ArgumentException("Result must come from BeginLoad()");
            }

            ar.WaitForCompletionAndVerify();
            return ar.ReturnValue;
        }

        /// <summary>
        /// Helper method to create a property from a message.
        /// </summary>
        /// <param name="property">
        /// The message property.
        /// </param>
        /// <returns>
        /// a management property.
        /// </returns>
        public ManagementProperty CreateProperty(ManagementResponse.Property property)
        {
            if (property.Value == null)
            {
                return new ManagementProperty<string>(property.Name, string.Empty, true);
            }

            var valueType = property.Value.GetType();
            var propType = typeof(ManagementProperty<>).MakeGenericType(valueType);
            var ctor = propType.GetConstructor(new[] { typeof(string), valueType, typeof(bool) });
            if (ctor == null)
            {
                throw new TypeLoadException("Couldn't find constructor");
            }

            return (ManagementProperty)ctor.Invoke(new[] { property.Name, property.Value, property.ReadOnly });
        }

        /// <summary>
        /// Helper method to create a child from a message.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <returns>
        /// a new <see cref="IRemoteManagementProviderImpl"/> implementation for the
        /// given arguments or null if the <see cref="ManagementResponse.ChildInfo.Type"/>
        /// is not supported.
        /// </returns>
        public IRemoteManagementProviderImpl CreateChild(
            MediAddress address, IRemoteManagementProviderImpl parent, ManagementResponse.ChildInfo child)
        {
            switch (child.Type)
            {
                case ManagementResponse.ChildType.ManagementProvider:
                    return new RemoteManagementProvider(this.messageDispatcher, address, parent, child.Name);
                case ManagementResponse.ChildType.ManagementObjectProvider:
                    return new RemoteManagementObjectProvider(this.messageDispatcher, address, parent, child.Name);
                case ManagementResponse.ChildType.ManagementTableProvider:
                    return new RemoteManagementTableProvider(this.messageDispatcher, address, parent, child.Name);
                default:
                    return null;
            }
        }

        private void HandleManagementResponse(object sender, MessageEventArgs<ManagementResponse> e)
        {
            if (e.Message.RequestId != this.GetHashCode())
            {
                return;
            }

            this.RaiseReceivedResponse(e);

            AsyncResult[] results;
            lock (this.locker)
            {
                this.loadState = LoadState.Loaded;
                if (this.requests.Count == 0)
                {
                    return;
                }

                results = this.requests.ToArray();
                this.requests.Clear();
            }

            foreach (var result in results)
            {
                result.CompleteAsynchronously();
            }
        }

        private void RaiseReceivedResponse(MessageEventArgs<ManagementResponse> e)
        {
            var handler = this.ReceivedResponse;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private class AsyncResult : AsyncResultBase
        {
            public AsyncResult(object returnValue, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.ReturnValue = returnValue;
            }

            public object ReturnValue { get; private set; }

            public void CompleteSynchronously()
            {
                this.Complete(true);
            }

            public void CompleteAsynchronously()
            {
                this.Complete(false);
            }
        }
    }
}