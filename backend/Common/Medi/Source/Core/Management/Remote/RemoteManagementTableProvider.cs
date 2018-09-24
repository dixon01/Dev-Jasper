// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteManagementTableProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteManagementTableProvider type.
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
    /// <see cref="IManagementTableProvider"/>s at a different node.
    /// </summary>
    internal class RemoteManagementTableProvider : ModifiableManagementTableProvider,
                                                   IRemoteManagementTableProvider,
                                                   IRemoteManagementProviderImpl
    {
        private readonly RemoteManagementHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteManagementTableProvider"/> class.
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
        public RemoteManagementTableProvider(
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
        public override IEnumerable<List<ManagementProperty>> Rows
        {
            get
            {
                this.handler.Load(false);
                return base.Rows;
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
        /// Get a row by its index.
        /// </summary>
        /// <param name="index">
        /// The index from zero on which to find the row.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        public override List<ManagementProperty> GetRow(int index)
        {
            this.handler.Load(false);
            return base.GetRow(index);
        }

        /// <summary>
        /// Get a property by the row index and and the property name.
        /// </summary>
        /// <param name="rowIndex">
        /// The index from zero on which to find the row.
        /// </param>
        /// <param name="columnName">
        /// The name of the column to return the value in the row.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        public override ManagementProperty GetCell(int rowIndex, string columnName)
        {
            this.handler.Load(false);
            return base.GetCell(rowIndex, columnName);
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
        /// Asynchronously begins to fetch the rows from the remote node.
        /// </summary>
        /// <param name="callback">
        /// The callback called when the rows where fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetRows"/>.
        /// </returns>
        public IAsyncResult BeginGetRows(AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, null, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch the rows from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetRows"/>.
        /// </param>
        /// <returns>
        /// The list of all rows.
        /// </returns>
        public IEnumerable<List<ManagementProperty>> EndGetRows(IAsyncResult result)
        {
            this.handler.EndLoad(result);
            return base.Rows;
        }

        /// <summary>
        /// Asynchronously begins to fetch the row with the given index from the remote node.
        /// </summary>
        /// <param name="index">
        /// The index from zero on which to find the row.
        /// </param>
        /// <param name="callback">
        /// The callback called when the row was fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetRow"/>.
        /// </returns>
        public IAsyncResult BeginGetRow(int index, AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, index, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch a row from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetRow"/>.
        /// </param>
        /// <returns>
        /// The row with the index given to <see cref="BeginGetRow"/>
        /// or null if no row with the given index exists.
        /// </returns>
        public List<ManagementProperty> EndGetRow(IAsyncResult result)
        {
            var index = (int)this.handler.EndLoad(result);
            return base.GetRow(index);
        }

        /// <summary>
        /// Asynchronously begins to fetch the cell with the given row index and column name from the remote node.
        /// </summary>
        /// <param name="rowIndex">
        /// The index from zero on which to find the row.
        /// </param>
        /// <param name="columnName">
        /// The name of the column to return the value in the row.
        /// </param>
        /// <param name="callback">
        /// The callback called when the cell was fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetCell"/>.
        /// </returns>
        public IAsyncResult BeginGetCell(int rowIndex, string columnName, AsyncCallback callback, object state)
        {
            return this.handler.BeginLoad(false, new KeyValuePair<int, string>(rowIndex, columnName), callback, state);
        }

        /// <summary>
        /// Ends the asynchronous request to fetch a cell from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetCell"/>.
        /// </param>
        /// <returns>
        /// The cell with the row index and column name given to <see cref="BeginGetCell"/>
        /// or null if the cell was not found.
        /// </returns>
        public ManagementProperty EndGetCell(IAsyncResult result)
        {
            var kvp = (KeyValuePair<int, string>)this.handler.EndLoad(result);
            return base.GetCell(kvp.Key, kvp.Value);
        }

        private void HandlerReceivedResponse(object sender, MessageEventArgs<ManagementResponse> e)
        {
            this.Clear();

            if (e.Message.Rows != null)
            {
                foreach (var row in e.Message.Rows)
                {
                    // ReSharper disable RedundantTypeArgumentsOfMethod
                    this.AddRow(row.ConvertAll<ManagementProperty>(this.handler.CreateProperty));
                    // ReSharper restore RedundantTypeArgumentsOfMethod
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