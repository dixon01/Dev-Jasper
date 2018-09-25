// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementProviderFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management.Clr;
    using Gorba.Common.Medi.Core.Management.FileSystem;
    using Gorba.Common.Medi.Core.Management.Provider;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Medi.Core.Management.Wmi;
    using Gorba.Common.Medi.Core.Messages;

    /// <summary>
    /// Factory for local and remote management providers.
    /// </summary>
    internal class ManagementProviderFactory : IManagementProviderFactory
    {
        private readonly IMessageDispatcher messageDispatcher;

        private bool messageDispatchingInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementProviderFactory"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message Dispatcher.
        /// </param>
        public ManagementProviderFactory(IMessageDispatcher messageDispatcher)
        {
            this.messageDispatcher = messageDispatcher;
            var localRoot = new LocalManagementProvider(this);
            localRoot.AddChild(new ClrManagementProvider(localRoot));
            localRoot.AddChild(FileSystemManagementProvider.CreateRoot(localRoot));
            localRoot.AddChild(new WmiManagementProvider(localRoot));
            this.LocalRoot = localRoot;
        }

        /// <summary>
        /// Gets the local management root object.
        /// </summary>
        public IModifiableManagementProvider LocalRoot { get; private set; }

        /// <summary>
        /// Creates an <see cref="IRemoteManagementProvider"/> for the root
        /// management object at the given address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// A newly created <see cref="IRemoteManagementProvider"/> implementation.
        /// </returns>
        public IRemoteManagementProvider CreateRemoteProvider(MediAddress address)
        {
            return new RemoteManagementProvider(this.messageDispatcher, address);
        }

        /// <summary>
        /// Create a unique name for a child.
        /// </summary>
        /// <param name="parent">the parent within which the name has to be unique</param>
        /// <param name="baseName">the base used for the name. A number will be appended to it.</param>
        /// <returns>the unique name</returns>
        public string CreateUniqueName(IManagementProvider parent, string baseName)
        {
            for (int i = 0;; i++)
            {
                var name = baseName + i;
                if (parent.GetChild(name) == null)
                {
                    return name;
                }
            }
        }

        /// <summary>
        /// Creates a provider. If the object implements IManageable, it will be queried,
        /// if not, an empty ModifiableManagementProvider is created using the type name as its name.
        /// </summary>
        /// <param name="name">
        /// The name. Be careful to choose a unique name for the provider, if necessary, use
        /// <see cref="CreateUniqueName"/> to assure the name is unique within its children.
        /// </param>
        /// <param name="parent">
        /// the parent.
        /// </param>
        /// <param name="managed">
        /// the object to be described. Can be an IManageable or not.
        /// </param>
        /// <returns>
        /// The management provider.
        /// </returns>
        public IManagementProvider CreateManagementProvider(string name, IManagementProvider parent, object managed)
        {
            var manageableObject = managed as IManageableObject;
            if (manageableObject != null)
            {
                return new ManageableObjectManagementProvider(name, manageableObject, parent);
            }

            var manageableTable = managed as IManageableTable;
            if (manageableTable != null)
            {
                return new ManageableTableManagementProvider(name, manageableTable, parent);
            }

            var manageable = managed as IManageable;
            if (manageable != null)
            {
                return new ManageableManagementProvider(name, manageable, parent);
            }

            return new ModifiableManagementProvider(name, parent);
        }

        /// <summary>
        /// Initializes message dispatching for remote management.
        /// This method has only to be called by MessageDispatcher.
        /// This method is necessary, so we don't run into an infinite loop
        /// (<code>MessageDispatcher..ctor</code> registers to the management, but it is
        /// not yet ready to handle calls to <see cref="MessageDispatcher.Subscribe{T}"/>).
        /// </summary>
        internal void InitMessageDispatching()
        {
            if (this.messageDispatchingInitialized)
            {
                return;
            }

            this.messageDispatchingInitialized = true;
            this.messageDispatcher.Subscribe<ManagementRequest>(this.HandleManagementRequest);
        }

        private void HandleManagementRequest(object sender, MessageEventArgs<ManagementRequest> e)
        {
            var response = new ManagementResponse { RequestId = e.Message.Id };
            var provider = this.LocalRoot.GetDescendant(false, e.Message.Path);
            if (provider != null)
            {
                var objProvider = provider as IManagementObjectProvider;
                if (objProvider != null)
                {
                    response.Properties = new List<ManagementResponse.Property>();
                    foreach (var property in objProvider.Properties)
                    {
                        response.Properties.Add(
                            new ManagementResponse.Property
                                {
                                    Name = property.Name, Value = property.Value, ReadOnly = property.ReadOnly
                                });
                    }
                }

                var tableProvider = provider as IManagementTableProvider;
                if (tableProvider != null)
                {
                    response.Rows = new List<List<ManagementResponse.Property>>();
                    foreach (var row in tableProvider.Rows)
                    {
                        var properties = new List<ManagementResponse.Property>();
                        foreach (var property in row)
                        {
                            properties.Add(
                                new ManagementResponse.Property
                                    {
                                        Name = property.Name, Value = property.Value, ReadOnly = property.ReadOnly
                                    });
                        }

                        response.Rows.Add(properties);
                    }
                }

                foreach (var child in provider.Children)
                {
                    var childInfo = new ManagementResponse.ChildInfo { Name = child.Name };
                    if (child is IManagementObjectProvider)
                    {
                        childInfo.Type = ManagementResponse.ChildType.ManagementObjectProvider;
                    }
                    else if (child is IManagementTableProvider)
                    {
                        childInfo.Type = ManagementResponse.ChildType.ManagementTableProvider;
                    }
                    else
                    {
                        childInfo.Type = ManagementResponse.ChildType.ManagementProvider;
                    }

                    response.ChildInfos.Add(childInfo);
                }
            }

            this.messageDispatcher.Send(e.Source, response);
        }

        private class LocalManagementProvider : ModifiableManagementProvider
        {
            private readonly IManagementProviderFactory factory;

            public LocalManagementProvider(IManagementProviderFactory factory)
                : base("Local", null)
            {
                this.factory = factory;
            }

            public override IManagementProviderFactory Factory
            {
                get
                {
                    return this.factory;
                }
            }
        }
    }
}
