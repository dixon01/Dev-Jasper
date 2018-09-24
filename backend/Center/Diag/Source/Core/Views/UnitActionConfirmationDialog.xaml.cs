// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitActionConfirmationDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UnitActionConfirmationDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// Interaction logic for UnitActionConfirmationDialog.xaml
    /// </summary>
    public partial class UnitActionConfirmationDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitActionConfirmationDialog" /> class.
        /// </summary>
        public UnitActionConfirmationDialog()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            ((PromptNotification)this.DataContext).Confirmed = true;
            this.Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
