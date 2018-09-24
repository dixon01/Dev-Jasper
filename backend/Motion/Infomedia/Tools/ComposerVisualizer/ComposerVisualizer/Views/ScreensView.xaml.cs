// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreensView.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ScreensView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels;

    /// <summary>
    /// Interaction logic for ScreensView.xaml
    /// </summary>
    public partial class ScreensView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreensView"/> class.
        /// </summary>
        public ScreensView()
        {
            this.InitializeComponent();
        }

        private void TreeViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;
            if (treeViewItem != null)
            {
                var treeView = treeViewItem.Header as ConfigBaseDataViewModel;
                if (treeView != null)
                {
                    this.ItemPropertyGrid.SelectedObject = treeView;
                    treeViewItem.IsSelected = true;
                }
            }
        }
    }
}
