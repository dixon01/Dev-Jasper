// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediTreeControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The MediTreeControl.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls
{
    using System.Windows;

    using Gorba.Center.Diag.Core.ViewModels.App;
    using Gorba.Center.Diag.Core.ViewModels.MediTree;

    using Telerik.Windows;
    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for MediTreeControl.xaml
    /// </summary>
    public partial class MediTreeControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediTreeControl" /> class.
        /// </summary>
        public MediTreeControl()
        {
            InitializeComponent();
        }

        private void LoadNode(RoutedEventArgs e)
        {
            var itemContainer = e.OriginalSource as RadTreeViewItem;
            if (itemContainer == null)
            {
                return;
            }

            var item = itemContainer.Item;
            var node = item as MediTreeNodeViewModel;
            var context = (MediTreeInfoPartViewModel)this.DataContext;

            context.LoadFileSystemFolderCommand.Execute(node);
        }

        private void TreeViewLoadOnDemand(object sender, RadRoutedEventArgs e)
        {
            this.LoadNode(e);
        }

        private void OnNodeSelected(object sender, RadRoutedEventArgs e)
        {
            this.LoadNode(e);
        }
    }
}
