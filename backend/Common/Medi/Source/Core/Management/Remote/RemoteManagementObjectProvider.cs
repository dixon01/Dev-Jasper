// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteManagementObjectProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteManagementObjectProvider type.
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
    /// <see cref="IManagementObjectProvider"/>s at a different node.
    /// </summary>
    internal class RemoteManagementObjectProvider : ModifiableManagementObjectProvider,
                                                    IRemoteManagementObjectProvider,
                                                    IRemoteManagementProviderImpl
    {
        private readonly RemoteManagementHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteManagementObjectProvider"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher.
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
        public RemoteManagementObjectProvider(
            IMessageDispatcher messageDispatcher,
            MediAddress address,
            IRemoteManagementProviderImpl parent,
            string name)
            : base(name, parent)
        {
            var path = new List<string>(parent.Path.Length + 1);
            path.AddRange(parent.Path);
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
        /// Gets all <see cref="ManagementProperty"/> objects for this node.
        /// </summary>
        public override IEnumerable<ManagementProperty> Properties
        {
            get
            {
                this.handler.Load(false);
                return base.Properties;
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
        /// Get a property by its name.
        /// </summary>
        /// <param name="name">
        /// The name of the property to be found.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        public override ManagementProperty GetProperty(string name)
        {
            this.handler.Load(false);
            return base.GetProperty(name);
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
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndReload"/>.
        /// </returns>
        public IAsyncResult BeginReload(AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(true, null, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to reload the cached management information from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginReload"/>.
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
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetChildren"/>.
        /// </returns>
        public IAsyncResult BeginGetChildren(AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, null, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch the children from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetChildren"/>.
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
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetChild"/>.
        /// </returns>
        public IAsyncResult BeginGetChild(string name, AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, name, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch a child from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetChild"/>.
        /// </param>
        /// <returns>
        /// The child with the name given to <see cref="BeginGetChild"/>
        /// or null if no child with the given name exists.
        /// </returns>
        public IManagementProvider EndGetChild(IAsyncResult result)
        {
            var name = (string)this.handler.EndLoad(result);
            return base.GetChild(name);
        }

        /// <summary>
        /// Asynchronously begins to fetch the properties from the remote node.
        /// </summary>
        /// <param name="callback">
        /// The callback called when the properties where fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetProperties"/>.
        /// </returns>
        public IAsyncResult BeginGetProperties(AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, null, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch the properties from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetProperties"/>.
        /// </param>
        /// <returns>
        /// The list of all properties.
        /// </returns>
        public IEnumerable<ManagementProperty> EndGetProperties(IAsyncResult result)
        {
            this.handler.EndLoad(result);
            return base.Properties;
        }

        /// <summary>
        /// Asynchronously begins to fetch the property with the given name from the remote node.
        /// </summary>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <param name="callback">
        /// The callback called when the property was fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetProperty"/>.
        /// </returns>
        public IAsyncResult BeginGetProperty(string name, AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, name, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch a property from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetProperty"/>.
        /// </param>
        /// <returns>
        /// The property with the name given to <see cref="BeginGetProperty"/>
        /// or null if no property with the given name exists.
        /// </returns>
        public ManagementProperty EndGetProperty(IAsyncResult result)
        {
            var name = (string)this.handler.EndLoad(result);
            return base.GetProperty(name);
        }

        private void HandlerReceivedResponse(object sender, MessageEventArgs<ManagementResponse> e)
        {
            this.Clear();

            if (e.Message.Properties != null)
            {
                foreach (var property in e.Message.Properties)
                {
                    this.AddProperty(this.handler.CreateProperty(property));
                }
            }

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