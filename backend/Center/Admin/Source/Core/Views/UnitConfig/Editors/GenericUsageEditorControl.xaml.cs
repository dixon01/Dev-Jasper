// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericUsageEditorControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for GenericUsageEditorControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.UnitConfig.Editors
{
    using System.Windows.Controls;

    using Gorba.Common.Protocols.Ximple.Generic;

    using Telerik.Windows;
    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for GenericUsageEditorControl.xaml
    /// </summary>
    public partial class GenericUsageEditorControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsageEditorControl"/> class.
        /// </summary>
        public GenericUsageEditorControl()
        {
            this.InitializeComponent();
        }

        private void RadTreeViewOnItemClick(object sender, RadRoutedEventArgs e)
        {
            var item = e.OriginalSource as RadTreeViewItem;
            if (item != null && item.DataContext is Column)
            {
                this.DropDownButton.IsOpen = false;
            }
        }
    }
}
