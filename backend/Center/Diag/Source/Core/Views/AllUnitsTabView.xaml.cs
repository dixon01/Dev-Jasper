// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllUnitsTabView.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The AllUnitsTabView.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Center.Diag.Core.Views.Controls;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;

    /// <summary>
    /// Interaction logic for AllUnitsTabView.xaml
    /// </summary>
    public partial class AllUnitsTabView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllUnitsTabView" /> class.
        /// </summary>
        public AllUnitsTabView()
        {
            InitializeComponent();
            this.Loaded += (sender, args) =>
                {
                    var context = (AllUnitsTab)DataContext;
                    if (context != null)
                    {
                        context.Shell.AllUnits.ItemPropertyChanged += this.OnUnitChanged;
                    }
                };
        }

        private void OnUnitChanged(object sender, ItemPropertyChangedEventArgs<UnitViewModelBase> e)
        {
            var source = this.MainGrid.TryFindResource("GroupedItems") as CollectionViewSource;
            if (source != null && source.View != null)
            {
                source.View.Refresh();
            }
        }

        private void OnUnitTileMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var context = (AllUnitsTab)this.DataContext;
            var unitTile = sender as UnitTileControl;
            if (unitTile != null)
            {
                var unit = unitTile.DataContext as UnitViewModelBase;
                if (unit != null)
                {
                    context.Shell.ConnectUnitCommand.Execute(unit);
                }
            }
        }

        private void OnUnitGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    var context = (AllUnitsTab)this.DataContext;
                    var unit = row.DataContext;
                    context.Shell.ConnectUnitCommand.Execute(unit);
                }
            }
        }
    }
}
