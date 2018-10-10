// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigTreeNodeViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigTreeNodeViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig
{
    using System.Collections.Generic;

    /// <summary>
    /// View model representing a node in the unit configurator navigation tree.
    /// </summary>
    public abstract class UnitConfigTreeNodeViewModelBase : DirtyViewModelBase
    {
        private string displayName;

        private string description;

        /// <summary>
        /// Gets or sets the name displayed in the tree and in the title of this node.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                this.SetProperty(ref this.displayName, value, () => this.DisplayName);
            }
        }

        /// <summary>
        /// Gets or sets the description shown below the editor.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }

        /// <summary>
        /// Gets the error state of this node.
        /// </summary>
        public abstract ErrorState ErrorState { get; }

        /// <summary>
        /// Gets all errors of this node.
        /// </summary>
        public abstract ICollection<ErrorItem> Errors { get; }
    }
}