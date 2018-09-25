// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderBarBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the base header bar elements.
    /// </summary>
    public abstract class HeaderBarBase : ViewModelBase
    {
        private readonly ObservableCollection<string> tenants;

        private readonly ObservableCollection<string> users;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderBarBase"/> class.
        /// </summary>
        protected HeaderBarBase()
        {
            this.tenants = new ObservableCollection<string>();
            this.users = new ObservableCollection<string>();
        }

        /// <summary>
        /// Gets or sets the application header image.
        /// </summary>
        public BitmapImage ApplicationNameImage { get; set; }

        /// <summary>
        /// Gets or sets the selected user.
        /// </summary>
        public string SelectedUser { get; set; }

        /// <summary>
        /// Gets or sets the selected tenant.
        /// </summary>
        public string SelectedTenant { get; set; }

        /// <summary>
        /// Gets the tenants collection.
        /// </summary>
        public ObservableCollection<string> Tenants
        {
            get
            {
                return this.tenants;
            }
        }

        /// <summary>
        /// Gets the users collection.
        /// </summary>
        public ObservableCollection<string> Users
        {
            get
            {
                return this.users;
            }
        }
    }
}
