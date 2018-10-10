// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigatorViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NavigatorViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Navigator
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The view model base class for items in the navigator.
    /// </summary>
    public class NavigatorViewModelBase : ViewModelBase
    {
        private bool isExpanded;

        private string displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorViewModelBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of this item.
        /// </param>
        public NavigatorViewModelBase(string name)
        {
            this.Name = name;
            this.DisplayName = name;
            this.IsExpanded = true;
        }

        /// <summary>
        /// Gets the name of this view model.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.SetProperty(ref this.isExpanded, value, () => this.IsExpanded);
            }
        }

        /// <summary>
        /// Gets or sets the display name shown for entities.
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
    }
}