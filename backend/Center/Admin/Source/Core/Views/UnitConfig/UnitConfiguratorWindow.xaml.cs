// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfiguratorWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for UnitConfiguratorWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.UnitConfig
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Framework.Views;
    using Gorba.Center.Common.Wpf.Views.Components;

    using Telerik.Windows.Controls;
    using Telerik.Windows.DragDrop;

    using DragEventArgs = Telerik.Windows.DragDrop.DragEventArgs;

    /// <summary>
    /// Interaction logic for UnitConfiguratorWindow.xaml
    /// </summary>
    public partial class UnitConfiguratorWindow : IWindowView
    {
        private bool versionSelectionLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitConfiguratorWindow"/> class.
        /// </summary>
        public UnitConfiguratorWindow()
        {
            this.InitializeComponent();

            DragDropManager.AddDropHandler(this.PanelBar, this.OnDrop);
            DragDropManager.AddDragOverHandler(this.PanelBar, this.OnDragOver);
        }

        private static TransformationPartViewModel GetTransformationPart(RoutedEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;
            if (source == null)
            {
                return null;
            }

            var text = source.GetVisualParent<TextBlockWithChangeIndicator>();
            if (text == null)
            {
                return null;
            }

            var transformation = text.DataContext as TransformationPartViewModel;
            return transformation;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            var transformation = GetTransformationPart(e);
            e.Effects = transformation == null ? DragDropEffects.None : DragDropEffects.Copy;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            // always return "None" to prevent moving away an item (only copy is allowed)
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            var transformation = GetTransformationPart(e);
            if (transformation == null)
            {
                return;
            }

            var items =
                DragDropPayloadManager.GetDataFromObject(
                    e.Data, typeof(TransformationDataViewModelBase)) as IEnumerable;
            if (items == null)
            {
                return;
            }

            foreach (var item in items.OfType<TransformationDataViewModelBase>())
            {
                transformation.Editor.Transformations.Add((TransformationDataViewModelBase)item.Clone());
            }
        }

        private void OnClickRestore(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void OnClickMinimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnClickMaximize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void OnClickClose(object sender, RoutedEventArgs e)
        {
            var viewModel = (UnitConfiguratorViewModel)DataContext;
            viewModel.CancelUnitConfigurationChanges.Execute(null);
        }

        private void VersionSelectionGridViewOnDataLoaded(object sender, EventArgs e)
        {
            var viewModel = (UnitConfiguratorViewModel)DataContext;
            var grid = sender as RadGridView;

            // workaround for bug with Telerik: when we get the DataLoaded event,
            // the data context hasn't completely changed yet, so let's invoke this a little later
            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        grid.Focus();
                        grid.SelectedItem = viewModel.CurrentVersion;
                        if (grid.SelectedItem != null)
                        {
                            grid.ScrollIntoView(grid.SelectedItem);
                        }

                        this.versionSelectionLoaded = true;
                    }));
        }

        private void VersionSelectionGridView_OnSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var grid = (RadGridView)sender;

            // workaround for bug with Telerik: when the selected item changes from code behind, the grid within
            // the dropdown content is not refreshed. We need to refresh it manually
            grid.Rebind();
            if (this.versionSelectionLoaded)
            {
                this.VersionDropDownButton.IsOpen = false;
            }
        }

        private void VersionSelectionGridView_OnSelectionChanging(object sender, SelectionChangingEventArgs e)
        {
            if (!this.versionSelectionLoaded)
            {
                return;
            }

            this.VersionDropDownButton.IsOpen = false;
        }
    }
}
