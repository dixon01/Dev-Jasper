// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigatorControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for NavigatorControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views
{
    using Gorba.Center.Admin.Core.ViewModels.Navigator;

    using Telerik.Windows;

    /// <summary>
    /// Interaction logic for NavigatorControl.xaml
    /// </summary>
    public partial class NavigatorControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorControl"/> class.
        /// </summary>
        public NavigatorControl()
        {
            this.InitializeComponent();
        }

        private void RadTreeView_OnPreviewSelected(object sender, RadRoutedEventArgs e)
        {
            e.Handled = !(e.OriginalSource is NavigatorEntityViewModel);
        }
    }
}
