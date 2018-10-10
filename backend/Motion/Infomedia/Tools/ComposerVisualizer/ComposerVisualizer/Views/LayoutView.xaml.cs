// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutView.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for LayoutView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for LayoutView.xaml
    /// </summary>
    public partial class LayoutView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutView"/> class.
        /// </summary>
        public LayoutView()
        {
            this.InitializeComponent();
        }

        private void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listItem = sender as ListViewItem;
            if (listItem != null)
            {
                var properties = listItem.Content;
                this.ItemPropertyGrid.SelectedObject = properties;
            }
        }
    }
}
