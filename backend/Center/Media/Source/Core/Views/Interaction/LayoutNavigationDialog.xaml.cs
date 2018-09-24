// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutNavigationDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LayoutNavigationDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System;

    /// <summary>
    /// Interaction logic for LayoutNavigationDialog.xaml
    /// </summary>
    public partial class LayoutNavigationDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutNavigationDialog"/> class.
        /// </summary>
        public LayoutNavigationDialog()
        {
            this.InitializeComponent();
        }

        private void LayoutSelectorOnButtonDoubleClicked(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
