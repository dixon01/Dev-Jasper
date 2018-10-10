// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectNodeInfoViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObjectMediTreeNodeViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.MediTree
{
    using System.Linq;

    using Gorba.Common.Medi.Core.Management.Remote;

    /// <summary>
    /// The view model representing object information in the Medi management tree.
    /// </summary>
    public class ObjectNodeInfoViewModel : NodeInfoViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectNodeInfoViewModel"/> class.
        /// </summary>
        /// <param name="provider">
        /// The object management provider represented by this view model.
        /// </param>
        public ObjectNodeInfoViewModel(IRemoteManagementObjectProvider provider)
        {
            this.Properties =
                new PropertiesHolder(provider.Properties.ToDictionary(p => p.Name, p => (object)p.StringValue));
        }

        /// <summary>
        /// Gets the object containing all properties of the represented object.
        /// </summary>
        public PropertiesHolder Properties { get; private set; }
    }
}