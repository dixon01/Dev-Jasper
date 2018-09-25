// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageVersionEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for PackageVersionEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Editors
{
    using System.Windows.Controls;

    using Telerik.Windows;
    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for PackageVersionEditor.xaml
    /// </summary>
    public partial class PackageVersionEditor : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageVersionEditor"/> class.
        /// </summary>
        public PackageVersionEditor()
        {
            this.InitializeComponent();
        }

        private void RadTreeViewOnItemDoubleClick(object sender, RadRoutedEventArgs e)
        {
            var item = e.OriginalSource as RadTreeViewItem;
            if (item == null)
            {
                return;
            }

            item.BeginEdit();
        }
    }
}
