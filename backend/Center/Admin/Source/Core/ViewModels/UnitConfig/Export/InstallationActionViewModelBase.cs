// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationActionViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationActionViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The installation action view model base.
    /// </summary>
    public abstract class InstallationActionViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationActionViewModelBase"/> class.
        /// </summary>
        protected InstallationActionViewModelBase()
        {
            this.Arguments = string.Empty;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets the executable path base.
        /// </summary>
        public virtual string ExecutablePathBase { get; set; }
    }
}