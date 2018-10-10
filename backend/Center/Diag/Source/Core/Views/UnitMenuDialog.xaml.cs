// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitMenuDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UnitMenuDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for UnitMenuDialog.xaml
    /// </summary>
    public partial class UnitMenuDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitMenuDialog"/> class.
        /// </summary>
        public UnitMenuDialog()
        {
            InitializeComponent();
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
