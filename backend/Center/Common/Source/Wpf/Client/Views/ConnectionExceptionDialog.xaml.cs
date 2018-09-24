// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionExceptionDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ConnectionExceptionDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ConnectionExceptionDialog.xaml
    /// </summary>
    public partial class ConnectionExceptionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionExceptionDialog"/> class.
        /// </summary>
        public ConnectionExceptionDialog()
        {
            InitializeComponent();
        }

        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
