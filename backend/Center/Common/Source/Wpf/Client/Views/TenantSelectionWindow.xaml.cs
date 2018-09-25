// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantSelectionWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TenantSelectionWindowBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Views
{
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Client.ViewModels;

    /// <summary>
    /// Interaction logic for TenantSelectionWindowBase.xaml
    /// </summary>
    public partial class TenantSelectionWindow : ITenantSelectionWindowView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSelectionWindow"/> class.
        /// </summary>
        public TenantSelectionWindow()
        {
            InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var context = (TenantSelectionViewModel)this.DataContext;
            var isSameUser = context.ApplicationState.CurrentUser.Id.Equals(context.ApplicationState.LastUserId);
            var lastTenant = context.Tenants.FirstOrDefault(t => t.Id == context.ApplicationState.LastTenantId);

            if (isSameUser && lastTenant != null)
            {
                this.TenantComboBox.SelectedItem = lastTenant;
            }
            else
            {
                this.TenantComboBox.SelectedItem = context.Tenants.FirstOrDefault();
            }
        }
    }
}
