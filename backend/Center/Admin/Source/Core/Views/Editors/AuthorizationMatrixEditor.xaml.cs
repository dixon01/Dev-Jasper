// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationMatrixEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for AuthorizationMatrixEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Editors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.Data.PropertyGrid;

    /// <summary>
    /// Interaction logic for AuthorizationMatrixEditor.xaml
    /// </summary>
    public partial class AuthorizationMatrixEditor : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationMatrixEditor"/> class.
        /// </summary>
        public AuthorizationMatrixEditor()
        {
            this.InitializeComponent();
        }

        private void UserControlOnLoaded(object sender, RoutedEventArgs e)
        {
            var dropDownEditor = this.GetVisualParent<DropDownEditor>();
            if (dropDownEditor == null)
            {
                return;
            }

            var dropDownButton = dropDownEditor.FindChildByType<RadDropDownButton>();
            if (dropDownButton == null)
            {
                return;
            }

            dropDownButton.DropDownOpened -= this.DropDownButtonOnDropDownOpened;
            dropDownButton.DropDownOpened += this.DropDownButtonOnDropDownOpened;
        }

        private void DropDownButtonOnDropDownOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var radDropDownButton = sender as RadDropDownButton;
            if (radDropDownButton == null)
            {
                return;
            }

            radDropDownButton.DropDownWidth = Math.Max(radDropDownButton.DropDownWidth, this.ActualWidth + 4);
        }
    }
}
