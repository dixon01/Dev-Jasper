// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Remote
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management.Provider;
    using Gorba.Common.Medi.Core.Messages;

    /// <summary>
    /// Implementation of <see cref="IRemoteManagementProvider"/> to access
    /// <see cref="IManagementProvider"/>s at a different node.
    /// </summary>
    internal class RemoteManagementProvider : ModifiableManagementProvider,
                                              IRemoteManagementProviderImpl
    {
        private readonly RemoteManagementHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteManagementProvider"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher.
        /// </param>
        /// <param name="address">
        /// The remote address.
        /// </param>
        public RemoteManagementProvider(IMessageDispatcher messageDispatcher, MediAddress address)
            : base(address.ToString(), null)
        {
            this.handler = new RemoteManagementHandler(messageDispatcher, address);
            this.handler.ReceivedResponse += this.HandlerReceivedResponse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteManagementProvider"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message Dispatcher.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public RemoteManagementProvider(
            IMessageDispatcher messageDispatcher,
            MediAddress address,
            IRemoteManagementProviderImpl parent,
            string name)
            : base(name, parent)
        {
            var parentPath = parent.Path;
            var path = new List<string>(parentPath.Length + 1);
            path.AddRange(parentPath);
            path.Add(name);

            this.handler = new RemoteManagementHandler(messageDispatcher, address, path.ToArray());
            this.handler.ReceivedResponse += this.HandlerReceivedResponse;
        }

        /// <summary>
        /// Gets the path elements of this provider.
        /// </summary>
        public string[] Path
        {
            get
            {
                return this.handler.Path;
            }
        }

        /// <summary>
        /// Gets all children.
        /// </summary>
        public override IEnumerable<IManagementProvider> Children
        {
            get
            {
                this.handler.Load(false);
                return base.Children;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.handler.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Get a child by its name.
        /// </summary>
        /// <param name="name">
        /// The name of the child to be found.
        /// </param>
        /// <returns>
        /// the child if found, otherwise null.
        /// </returns>
        public override IManagementProvider GetChild(string name)
        {
            this.handler.Load(false);
            return base.GetChild(name);
        }

        /// <summary>
        /// Reloads the cached management information from the remote host.
        /// </summary>
        public void Reload()
        {
            this.handler.Load(true);
        }

        /// <summary>
        /// Asynchronously begins to reload the cached management information from the remote node.
        /// </summary>
        /// <param name="callback">
        /// The callback called when information was refreshed.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="IRemoteManagementProvider.EndReload"/>.
        /// </returns>
        public IAsyncResult BeginReload(AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(true, null, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to reload the cached management information from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="IRemoteManagementProvider.BeginReload"/>.
        /// </param>
        public void EndReload(IAsyncResult result)
        {
            this.handler.EndLoad(result);
        }

        /// <summary>
        /// Asynchronously begins to fetch the children from the remote node.
        /// </summary>
        /// <param name="callback">
        /// The callback called when the children where fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="IRemoteManagementProvider.EndGetChildren"/>.
        /// </returns>
        public IAsyncResult BeginGetChildren(AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, null, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch the children from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="IRemoteManagementProvider.BeginGetChildren"/>.
        /// </param>
        /// <returns>
        /// The list of all children.
        /// </returns>
        public IEnumerable<IManagementProvider> EndGetChildren(IAsyncResult result)
        {
            this.handler.EndLoad(result);
            return base.Children;
        }

        /// <summary>
        /// Asynchronously begins to fetch the child with the given name from the remote node.
        /// </summary>
        /// <param name="name">
        /// The name of the child.
        /// </param>
        /// <param name="callback">
        /// The callback called when the child was fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="IRemoteManagementProvider.EndGetChild"/>.
        /// </returns>
        public IAsyncResult BeginGetChild(string name, AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, name, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch a child from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="IRemoteManagementProvider.BeginGetChild"/>.
        /// </param>
        /// <returns>
        /// The child with the name given to <see cref="IRemoteManagementProvider.BeginGetChild"/>
        /// or null if no child with the given name exists.
        /// </returns>
        public IManagementProvider EndGetChild(IAsyncResult result)
        {
            var name = (string)this.handler.EndLoad(result);
            return base.GetChild(name);
        }

        private void HandlerReceivedResponse(object sender, MessageEventArgs<ManagementResponse> e)
        {
            this.Clear();

            foreach (var child in e.Message.ChildInfos)
            {
                var childProvider = this.handler.CreateChild(this.handler.Address, this, child);
                if (childProvider != null)
                {
                    this.AddChild(childProvider);
                }
            }
        }
    }
}
