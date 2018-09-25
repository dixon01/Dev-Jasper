// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileMenuDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for FileMenuDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Menu
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for FileMenuDialog.xaml
    /// </summary>
    public partial class FileMenuDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMenuDialog"/> class.
        /// </summary>
        public FileMenuDialog()
        {
            InitializeComponent();
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
