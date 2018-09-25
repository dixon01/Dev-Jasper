// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManagementProviderFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IManagementProviderFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using Gorba.Common.Medi.Core.Management.Remote;

    /// <summary>
    /// Interface for a factory of <see cref="IManagementProvider"/>s.
    /// </summary>
    public interface IManagementProviderFactory
    {
        /// <summary>
        /// Gets the local management root object.
        /// </summary>
        IModifiableManagementProvider LocalRoot { get; }

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
        IRemoteManagementProvider CreateRemoteProvider(MediAddress address);

        /// <summary>
        /// Create a unique name for a child.
        /// </summary>
        /// <param name="parent">the parent within which the name has to be unique</param>
        /// <param name="baseName">the base used for the name. A number will be appended to it.</param>
        /// <returns>the unique name</returns>
        string CreateUniqueName(IManagementProvider parent, string baseName);

        /// <summary>
        /// Creates a provider. If the object implements IManageable, it will be queried, 
        /// if not, an empty ModifiableManagementProvider is created using the type name as its name.
        /// </summary>
        /// <param name="name">
        /// The name. Be careful to choose a unique name for the provider, if necessary, use
        /// <see cref="ManagementProviderFactory.CreateUniqueName"/> to assure the name is unique within its children.
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
        IManagementProvider CreateManagementProvider(string name, IManagementProvider parent, object managed);
    }
}