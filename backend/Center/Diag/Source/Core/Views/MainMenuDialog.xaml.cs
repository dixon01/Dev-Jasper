// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainMenuDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The MainMenuDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainMenuDialog.xaml
    /// </summary>
    public partial class MainMenuDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuDialog"/> class.
        /// </summary>
        public MainMenuDialog()
        {
            InitializeComponent();
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
