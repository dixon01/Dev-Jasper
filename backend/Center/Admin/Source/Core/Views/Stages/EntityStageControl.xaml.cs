// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityStageControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for EntityStageControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Stages
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Xml;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.Interaction;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.Stages;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;
    using Telerik.Windows.Data;

    using GridViewColumn = Telerik.Windows.Controls.GridViewColumn;

    /// <summary>
    /// Interaction logic for EntityStageControl.xaml
    /// </summary>
    public partial class EntityStageControl : UserControl
    {
        private bool resettingItemSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityStageControl"/> class.
        /// </summary>
        public EntityStageControl()
        {
            this.InitializeComponent();
        }

        private void GridViewOnAutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            if (e.ItemPropertyInfo.Name == "ClonedFrom" || e.ItemPropertyInfo.Name == "ReadableModel"
                || e.ItemPropertyInfo.Name == "DisplayText" || e.ItemPropertyInfo.Name == "Factory"
                || e.ItemPropertyInfo.Name == "Dispatcher")
            {
                // internal properties are never shown in the grid
                e.Cancel = true;
                return;
            }

            if (typeof(ICommand).IsAssignableFrom(e.ItemPropertyInfo.PropertyType))
            {
                // command properties are never shown in the form
                e.Cancel = true;
                return;
            }

            if (typeof(EntityCollectionBase).IsAssignableFrom(e.ItemPropertyInfo.PropertyType))
            {
                // collection properties are never shown in the grid
                e.Cancel = true;
                return;
            }

            if (typeof(ReadOnlyDataViewModelBase).IsAssignableFrom(e.ItemPropertyInfo.PropertyType))
            {
                // reference properties are represented by their xxxDisplayText property (see below)
                e.Cancel = true;
                return;
            }

            if (e.ItemPropertyInfo.Name == "Id")
            {
                // when grouped, show the number of entities behind the group name
                e.Column.AggregateFunctions.Add(
                    new CountFunction { ResultFormatString = AdminStrings.EntityList_CountFormat });
            }

            if (e.ItemPropertyInfo.Name.EndsWith("DisplayText"))
            {
                var propertyName = e.ItemPropertyInfo.Name.Remove(e.ItemPropertyInfo.Name.Length - 11);
                e.Column.Header = propertyName;

                // create a hyperlink data template to link to the right entity
                var stringReader = this.GetHyperlinkDataTemplateStringReader(propertyName);
                var xmlReader = XmlReader.Create(stringReader);
                e.Column.CellTemplate = XamlReader.Load(xmlReader) as DataTemplate;
            }

            if (typeof(DateTime).IsAssignableFrom(e.ItemPropertyInfo.PropertyType)
                || typeof(DateTime?).IsAssignableFrom(e.ItemPropertyInfo.PropertyType))
            {
                var propertyName = e.ItemPropertyInfo.Name;

                // create date time data template using the UtcToUiTimeConverter
                var stringReader = this.GetDateTimeDataTemplateStringReader(propertyName);
                var xmlReader = XmlReader.Create(stringReader);
                e.Column.CellTemplate = XamlReader.Load(xmlReader) as DataTemplate;
            }

            var viewModel = this.DataContext as EntityStageViewModelBase;
            if (viewModel == null)
            {
                return;
            }

            var parameters = new ColumnVisibilityParameters(viewModel.EntityName, e.ItemPropertyInfo.Name);
            viewModel.FilterEntityColumnCommand.Execute(parameters);
            if (parameters.Visibility == Visibility.Hidden)
            {
                // if the visibility is "hidden", we don't create a column
                e.Cancel = true;
                return;
            }

            // make the column visible only if it is to be shown
            e.Column.IsVisible = parameters.Visibility == Visibility.Visible;
            PropertyChangedEventManager.AddHandler(e.Column, this.ColumnVisibleChanged, "IsVisible");
        }

        private StringReader GetDateTimeDataTemplateStringReader(string propertyName)
        {
            var stringReader = new StringReader(@"
                    <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                    xmlns:converters7=""clr-namespace:Gorba.Center.Common.Wpf.Framework"
                                                + @".Converters;assembly=Gorba.Center.Common.Wpf.Framework""
                                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                        <DataTemplate.Resources>
                            <converters7:UtcToUiTimeConverter x:Key=""UtcToUiTimeConverter"" />
                        </DataTemplate.Resources>
                        <TextBlock Text=""{Binding " + propertyName
                                                + @", Converter={StaticResource UtcToUiTimeConverter}}""/>
                    </DataTemplate>");
            return stringReader;
        }

        private StringReader GetHyperlinkDataTemplateStringReader(string propertyName)
        {
            const string CommandPropertyName = "NavigateToEntityCommand";
            var stringReader = new StringReader(@"
                    <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                  xmlns:stages=""clr-namespace:" + this.GetType().Namespace + @";assembly="
                                                + this.GetType().Assembly.GetName().Name + @""">
                        <TextBlock>
                            <Hyperlink Command=""{Binding DataContext." + CommandPropertyName
                                                + @", RelativeSource={RelativeSource AncestorType=stages:"
                                                + this.GetType().Name
                                                + @"}}""
                                        CommandParameter=""{Binding " + propertyName + @"}"">
                                <TextBlock Text=""{Binding " + propertyName + @"}""/>
                            </Hyperlink>
                        </TextBlock>
                    </DataTemplate>");
            return stringReader;
        }

        private void ColumnVisibleChanged(object sender, PropertyChangedEventArgs e)
        {
            var column = sender as GridViewColumn;
            var viewModel = this.DataContext as EntityStageViewModelBase;
            if (column == null || viewModel == null)
            {
                return;
            }

            var columnFilter = new ColumnVisibilityParameters(viewModel.EntityName, column.UniqueName);
            columnFilter.Visibility = column.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            viewModel.UpdateColumnVisibilityCommand.Execute(columnFilter);
        }

        private void GridViewOnDataLoaded(object sender, EventArgs e)
        {
            var grid = sender as RadGridView;
            if (grid == null || this.resettingItemSource)
            {
                return;
            }

            var viewModel = this.DataContext as EntityStageViewModelBase;
            if (viewModel == null)
            {
                return;
            }

            // workaround for bug with Telerik: if an empty list of ICustomTypeDescriptor is changed to have one item,
            // the grid doesn't load the available columns, therefore we refresh manually (grid.Rebind() isn't enough).
            if (viewModel.Instances.Count == 1 && viewModel.Instances[0] is ReadOnlyDataViewModelWithUdpBase)
            {
                this.resettingItemSource = true;
                try
                {
                    if (grid.GroupDescriptors.Count < 1)
                    {
                        grid.ItemsSource = null;
                        grid.SetBinding(DataControl.ItemsSourceProperty, "Instances");
                    }
                }
                finally
                {
                    this.resettingItemSource = false;
                }
            }

            // workaround for bug with Telerik: when we get the DataLoaded event,
            // the data context hasn't completely changed yet, so let's invoke this a little later
            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                        {
                            grid.Focus();
                            grid.SelectedItem = viewModel.SelectedInstance;
                            if (grid.SelectedItem != null)
                            {
                                grid.ScrollIntoView(grid.SelectedItem);
                            }
                        }));
        }

        private void GridViewOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender == null)
            {
                return;
            }

            var row = originalSender.ParentOfType<GridViewRow>();
            if (row == null)
            {
                return;
            }

            var details = originalSender.ParentOfType<DetailsPresenter>();
            if (details != null)
            {
                // double-click on details doesn't open editor
                return;
            }

            var cell = originalSender.ParentOfType<GridViewCell>();
            if (cell != null && cell.Column is GridViewToggleRowDetailsColumn)
            {
                // double-click on details toggle button doesn't open editor
                return;
            }

            var viewModel = this.DataContext as EntityStageViewModelBase;
            if (viewModel == null)
            {
                return;
            }

            var command = viewModel.EditEntityCommand;
            if (command.CanExecute(row.DataContext))
            {
                command.Execute(row.DataContext);
            }
        }

        private void GridViewOnLoadingRowDetails(object sender, GridViewRowDetailsEventArgs e)
        {
            var viewModel = this.DataContext as EntityStageViewModelBase;
            if (viewModel == null)
            {
                return;
            }

            viewModel.LoadEntityDetailsCommand.Execute(e.Row.Item);
        }

        private void ExpandAllOnClick(object sender, RoutedEventArgs e)
        {
            this.GridView.ExpandAllGroups();
        }

        private void CollapseAllOnClick(object sender, RoutedEventArgs e)
        {
            this.GridView.CollapseAllGroups();
        }
    }
}
