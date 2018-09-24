// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationMenuDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ApplicationMenuDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ApplicationMenuDialog.xaml
    /// </summary>
    public partial class ApplicationMenuDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMenuDialog"/> class.
        /// </summary>
        public ApplicationMenuDialog()
        {
            InitializeComponent();
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
