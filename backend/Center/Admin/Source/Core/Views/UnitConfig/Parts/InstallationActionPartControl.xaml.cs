// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationActionPartControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for InstallationActionPartControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.UnitConfig.Parts
{
    using System.Windows.Input;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;

    /// <summary>
    /// Interaction logic for InstallationActionPartControl.xaml
    /// </summary>
    public partial class InstallationActionPartControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationActionPartControl"/> class.
        /// </summary>
        public InstallationActionPartControl()
        {
            this.InitializeComponent();
            RowReorderBehavior.SetIsEnabled(this.ActionsGridView, true);
        }

        private void ActionsGridView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var grid = sender as RadGridView;
                if (grid == null)
                {
                    return;
                }

                if (grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    var row = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as GridViewRow;
                    if (row == null)
                    {
                        return;
                    }

                    if (!row.IsMouseOver)
                    {
                        (row as GridViewRow).IsSelected = false;
                    }

                    grid.CommitEdit();
                }
            }
        }
    }
}
