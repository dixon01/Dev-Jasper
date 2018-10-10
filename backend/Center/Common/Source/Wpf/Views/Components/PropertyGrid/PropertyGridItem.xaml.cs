// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridItem.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridItem.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;

    using NLog;

    /// <summary>
    /// Interaction logic for PropertyGridItem
    /// </summary>
    [ContentProperty("EditorTemplate")]
    public partial class PropertyGridItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The data converter property.
        /// </summary>
        public static readonly DependencyProperty DataConverterProperty = DependencyProperty.Register(
            "DataConverter",
            typeof(IValueConverter),
            typeof(PropertyGridItem),
            new PropertyMetadata(default(IValueConverter)));

        /// <summary>
        /// The data source property.
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
            "DataSource",
            typeof(object),
            typeof(PropertyGridItem),
            new FrameworkPropertyMetadata(default(object), OnDataSourceChanged) { BindsTwoWayByDefault = true });

        /// <summary>
        /// The data source property.
        /// </summary>
        public static readonly DependencyProperty ParallelDataProperty = DependencyProperty.Register(
            "ParallelData",
            typeof(object),
            typeof(PropertyGridItem),
            new FrameworkPropertyMetadata(default(object), OnParallelDataChanged) { BindsTwoWayByDefault = true });

        /// <summary>
        /// The wrapped data source property.
        /// </summary>
        public static readonly DependencyProperty WrappedDataSourceProperty =
            DependencyProperty.Register(
                "WrappedDataSource",
                typeof(PropertyGridItemDataSource),
                typeof(PropertyGridItem),
                new PropertyMetadata(default(PropertyGridItemDataSource)));

        /// <summary>
        /// The has context button property.
        /// </summary>
        public static readonly DependencyProperty HasContextButtonProperty =
            DependencyProperty.Register(
                "HasContextButton", typeof(bool), typeof(PropertyGridItem), new PropertyMetadata(true));

        /// <summary>
        /// The label column width property.
        /// </summary>
        public static readonly DependencyProperty LabelColumnWidthProperty =
            DependencyProperty.Register(
                "LabelColumnWidth",
                typeof(GridLength),
                typeof(PropertyGridItem),
                new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));

        /// <summary>
        /// The context button column width property.
        /// </summary>
        public static readonly DependencyProperty ContextButtonColumnWidthProperty =
            DependencyProperty.Register(
                "ContextButtonColumnWidth",
                typeof(GridLength),
                typeof(PropertyGridItem),
                new PropertyMetadata(new GridLength(16)));

        /// <summary>
        /// The domain object property.
        /// </summary>
        public static readonly DependencyProperty DomainObjectProperty = DependencyProperty.Register(
            "DomainObject", typeof(object), typeof(PropertyGridItem), new PropertyMetadata(default(object)));

        /// <summary>
        /// The action callback
        /// </summary>
        public static readonly DependencyProperty ActionCallbackProperty = DependencyProperty.Register(
            "ActionCallback",
            typeof(Action<PropertyGridItem, PropertyGridItemDataSource>),
            typeof(PropertyGridItem),
            new PropertyMetadata(default(Action<PropertyGridItem, PropertyGridItemDataSource>)));

        /// <summary>
        /// The data is same for all property
        /// </summary>
        public static readonly DependencyProperty HasMultipleDifferingDataSourcesProperty = DependencyProperty.Register(
            "HasMultipleDifferingDataSources",
            typeof(bool),
            typeof(PropertyGridItem),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// The is multi select property.
        /// </summary>
        public static readonly DependencyProperty IsMultiSelectProperty = DependencyProperty.Register(
            "IsMultiSelect",
            typeof(bool),
            typeof(PropertyGridItem),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// The navigate button action property.
        /// </summary>
        public static readonly DependencyProperty NavigateButtonActionProperty =
            DependencyProperty.Register(
                "NavigateButtonAction",
                typeof(Action),
                typeof(PropertyGridItem),
                new PropertyMetadata(default(Action)));

        /// <summary>
        /// The navigate button text property.
        /// </summary>
        public static readonly DependencyProperty NavigateButtonTextProperty =
            DependencyProperty.Register(
                "NavigateButtonText",
                typeof(string),
                typeof(PropertyGridItem),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// The format string for parsing.
        /// </summary>
        public static readonly DependencyProperty FormatStringProperty =
            DependencyProperty.Register(
                "FormatString",
                typeof(string),
                typeof(PropertyGridItem),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The m_ data type.
        /// </summary>
        private Type dataType;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridItem"/> class.
        /// </summary>
        public PropertyGridItem()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// The data source changed event
        /// </summary>
        public event Action<PropertyGridItem, object> DataSourceChanged;

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the domain object.
        /// </summary>
        public object DomainObject
        {
            get
            {
                return this.GetValue(DomainObjectProperty);
            }

            set
            {
                this.SetValue(DomainObjectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is multi select.
        /// </summary>
        public bool IsMultiSelect
        {
            get
            {
                return (bool)this.GetValue(IsMultiSelectProperty);
            }

            set
            {
                this.SetValue(IsMultiSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the data converter.
        /// </summary>
        public IValueConverter DataConverter
        {
            get
            {
                return (IValueConverter)this.GetValue(DataConverterProperty);
            }

            set
            {
                this.SetValue(DataConverterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public object DataSource
        {
            get
            {
                return this.GetValue(DataSourceProperty);
            }

            set
            {
                this.SetValue(DataSourceProperty, value);

                if (this.HasMultipleDifferingDataSources)
                {
                    this.HasMultipleDifferingDataSources = false;
                }

                this.OnDataSourceChanged();
            }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public object ParallelData
        {
            get
            {
                return this.GetValue(ParallelDataProperty);
            }

            set
            {
                this.SetValue(ParallelDataProperty, value);

                this.OnDataSourceChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether has context button.
        /// </summary>
        public bool HasContextButton
        {
            get
            {
                return (bool)this.GetValue(HasContextButtonProperty);
            }

            set
            {
                this.SetValue(HasContextButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label column width.
        /// </summary>
        public GridLength LabelColumnWidth
        {
            get
            {
                return (GridLength)this.GetValue(LabelColumnWidthProperty);
            }

            set
            {
                this.SetValue(LabelColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the context button column width.
        /// </summary>
        public GridLength ContextButtonColumnWidth
        {
            get
            {
                return (GridLength)this.GetValue(ContextButtonColumnWidthProperty);
            }

            set
            {
                this.SetValue(ContextButtonColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public PropertyGridItemDataSource WrappedDataSource
        {
            get
            {
                return (PropertyGridItemDataSource)this.GetValue(WrappedDataSourceProperty);
            }

            set
            {
                this.SetValue(WrappedDataSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the action callback
        /// </summary>
        public Action<PropertyGridItem, PropertyGridItemDataSource> ActionCallback
        {
            get
            {
                return (Action<PropertyGridItem, PropertyGridItemDataSource>)this.GetValue(ActionCallbackProperty);
            }

            set
            {
                this.SetValue(ActionCallbackProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data is same for all
        /// </summary>
        public bool HasMultipleDifferingDataSources
        {
            get
            {
                return (bool)this.GetValue(HasMultipleDifferingDataSourcesProperty);
            }

            set
            {
                this.SetValue(HasMultipleDifferingDataSourcesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the navigate button text.
        /// </summary>
        public string NavigateButtonText
        {
            get
            {
                return (string)this.GetValue(NavigateButtonTextProperty);
            }

            set
            {
                this.SetValue(NavigateButtonTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the navigate button text.
        /// </summary>
        public string FormatString
        {
            get
            {
                return (string)this.GetValue(FormatStringProperty);
            }

            set
            {
                this.SetValue(FormatStringProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the navigate button action.
        /// </summary>
        public Action NavigateButtonAction
        {
            get
            {
                return (Action)this.GetValue(NavigateButtonActionProperty);
            }

            set
            {
                this.SetValue(NavigateButtonActionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        public object ParentObject { get; set; }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Search(string searchText)
        {
            var result = false;

            var headerText = this.Header != null ? this.Header.ToLower() : string.Empty;
            var headerTooltipText = this.HeaderToolTip != null ? this.HeaderToolTip.ToLower() : string.Empty;

            if (headerText.Contains(searchText.ToLower()) || headerTooltipText.Contains(searchText.ToLower()))
            {
                result = true;
                this.Visibility = Visibility.Visible;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }

            return result;
        }

        /// <summary>
        /// handles data source changes (used if initial data source is null)
        /// </summary>
        public void HandleDataSourceChange()
        {
            if (this.WrappedDataSource == null)
            {
                this.WrappedDataSource = new PropertyGridItemDataSource(this);
            }
            else
            {
                this.InvalidateProperty(WrappedDataSourceProperty);
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The on default data source changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// The dependency property changed event args.
        /// </param>
        private static void OnDefaultDataSourceChanged(
            DependencyObject sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((PropertyGridItem)sender).OnPropertyChanged("HasDefaultValue");
        }

        /// <summary>
        /// The property changed callback.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnDataSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var item = (PropertyGridItem)dependencyObject;
            item.HandleDataSourceChange();
        }

        /// <summary>
        /// The property changed callback.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnParallelDataChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var item = (PropertyGridItem)dependencyObject;
            item.HandleDataSourceChange();
        }

        /// <summary>
        /// The on context button clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnContextButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.Menu != null)
            {
                this.Menu.PlacementTarget = this;
                this.Menu.IsOpen = false;
                this.Menu.IsOpen = true;
            }
        }

        /// <summary>
        /// The on data source changed.
        /// </summary>
        private void OnDataSourceChanged()
        {
            this.FireDataSourceChanged(this.DataSource);
        }

        private void FireDataSourceChanged(object dataSource)
        {
            if (this.DataSourceChanged != null)
            {
                this.DataSourceChanged(this, dataSource);
            }
        }

        /// <summary>
        /// The on loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="routedEventArgs">
        /// The routed event args.
        /// </param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.DataSource != null)
            {
                this.dataType = this.DataSource.GetType();
            }
            else
            {
                Log.Error("OnLoaded()", "No DataSource found.");
            }

            if (this.ContextButtonTemplate == null)
            {
                this.ContextButtonTemplate = (ControlTemplate)this.FindResource("ContextButtonTemplate");
            }
        }
    }
}