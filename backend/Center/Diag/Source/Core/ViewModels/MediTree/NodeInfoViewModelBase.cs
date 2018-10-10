// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeInfoViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NodeInfoViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.MediTree
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.Medi.Core.Management.Remote;

    /// <summary>
    /// View model base class for additional node information.
    /// </summary>
    public abstract class NodeInfoViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Creates a new <see cref="NodeInfoViewModelBase"/> from the given provider.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// Either an <see cref="ObjectNodeInfoViewModel"/>, a <see cref="TableNodeInfoViewModel"/>
        /// or null.
        /// </returns>
        public static NodeInfoViewModelBase Create(IRemoteManagementProvider provider)
        {
            var obj = provider as IRemoteManagementObjectProvider;
            if (obj != null)
            {
                return new ObjectNodeInfoViewModel(obj);
            }

            var table = provider as IRemoteManagementTableProvider;
            if (table != null)
            {
                return new TableNodeInfoViewModel(table);
            }

            return null;
        }
    }
}
