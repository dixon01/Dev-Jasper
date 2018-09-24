// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediTreeNodeViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediTreeNodeViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.MediTree
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.Medi.Core.Management.Remote;

    /// <summary>
    /// The view model representing a node in the Medi management tree.
    /// </summary>
    public class MediTreeNodeViewModel : ViewModelBase
    {
        /// <summary>
        /// The dummy tree node that is not representing a real item.
        /// </summary>
        public static readonly MediTreeNodeViewModel Dummy = new MediTreeNodeViewModel();

        private bool isLoading;

        private NodeInfoViewModelBase info;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediTreeNodeViewModel"/> class.
        /// </summary>
        /// <param name="provider">
        /// The management provider represented by this view model.
        /// </param>
        public MediTreeNodeViewModel(IRemoteManagementProvider provider)
            : this()
        {
            this.Provider = provider;
            this.Name = provider.Name;

            if (provider is IRemoteManagementObjectProvider)
            {
                this.NodeType = NodeType.Object;
            }
            else if (provider is IRemoteManagementTableProvider)
            {
                this.NodeType = NodeType.Table;
            }
            else
            {
                this.NodeType = NodeType.Plain;
            }
        }

        private MediTreeNodeViewModel()
        {
            this.Children = new ObservableCollection<MediTreeNodeViewModel>();
        }

        /// <summary>
        /// Gets the management provider represented by this view model.
        /// </summary>
        public IRemoteManagementProvider Provider { get; private set; }

        /// <summary>
        /// Gets the name of this node.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the collection of children of this node.
        /// The children are filled asynchronously by the controller.
        /// </summary>
        public ObservableCollection<MediTreeNodeViewModel> Children { get; private set; }

        /// <summary>
        /// Gets or sets the additional node information.
        /// </summary>
        public NodeInfoViewModelBase Info
        {
            get
            {
                return this.info;
            }

            set
            {
                this.SetProperty(ref this.info, value, () => this.Info);
            }
        }

        /// <summary>
        /// Gets the node type.
        /// </summary>
        public NodeType NodeType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this node is currently loading.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.SetProperty(ref this.isLoading, value, () => this.IsLoading);
            }
        }
    }
}
