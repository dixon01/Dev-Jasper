// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewMenuDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ViewMenuDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ViewMenuDialog.xaml
    /// </summary>
    public partial class ViewMenuDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMenuDialog"/> class.
        /// </summary>
        public ViewMenuDialog()
        {
            InitializeComponent();
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
