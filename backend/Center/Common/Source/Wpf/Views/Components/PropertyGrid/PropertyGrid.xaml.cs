// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGrid.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGrid.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// Interaction logic for PropertyGrid
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules",
        "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
    [ContentProperty("Groups")]
    public partial class PropertyGrid
    {
        /// <summary>
        /// The actual label column width property.
        /// </summary>
        public static readonly DependencyProperty ActualLabelColumnWidthProperty =
            DependencyProperty.Register(
                "ActualLabelColumnWidth",
                typeof(double),
                typeof(PropertyGrid),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The groups property.
        /// </summary>
        public static readonly DependencyProperty GroupsProperty = DependencyProperty.Register(
            "Groups",
            typeof(ObservableCollection<PropertyGridGroup>),
            typeof(PropertyGrid),
            new PropertyMetadata(default(ObservableCollection<PropertyGridGroup>)));

        /// <summary>
        /// The search box help text property.
        /// </summary>
        public static readonly DependencyProperty SearchBoxHelpTextProperty = DependencyProperty.Register(
            "SearchBoxHelpText", typeof(string), typeof(PropertyGrid), new PropertyMetadata(default(string)));

        /// <summary>
        /// The has search box property.
        /// </summary>
        public static readonly DependencyProperty HasSearchBoxProperty =
            DependencyProperty.Register("HasSearchBox", typeof(bool), typeof(PropertyGrid), new PropertyMetadata(true));

        /// <summary>
        /// The dynamic data converter.
        /// </summary>
        public static readonly DependencyProperty DynamicDataConverterProperty =
            DependencyProperty.Register(
                "DynamicDataConverter",
                typeof(IValueConverter),
                typeof(PropertyGrid),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// the domain value collection
        /// </summary>
        public static readonly DependencyProperty DomainValueCollectionProperty = DependencyProperty.Register(
            "DomainValueCollection",
            typeof(Dictionary<string, ICollection>),
            typeof(PropertyGrid),
            new PropertyMetadata(default(Dictionary<string, ICollection>)));

        /// <summary>
        /// The editor callbacks
        /// </summary>
        public static readonly DependencyProperty EditorCallbacksProperty =
            DependencyProperty.Register(
                "EditorCallbacks",
                typeof(Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>),
                typeof(PropertyGrid),
                new PropertyMetadata(default(Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>)));

        /// <summary>
        /// The navigation callbacks property.
        /// </summary>
        public static readonly DependencyProperty NavigationCallbacksProperty =
            DependencyProperty.Register(
                "NavigationCallbacks",
                typeof(Dictionary<string, KeyValuePair<string, Action>>),
                typeof(PropertyGrid),
                new PropertyMetadata(default(Dictionary<string, KeyValuePair<string, Action>>)));

        private readonly Dictionary<string, PropertyGridItem> propertyItemCache =
            new Dictionary<string, PropertyGridItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGrid"/> class.
        /// </summary>
        public PropertyGrid()
        {
            this.InitializeComponent();

            this.Groups = new ObservableCollection<PropertyGridGroup>();

            this.DomainValueCollection = new Dictionary<string, ICollection>();

            this.EditorCallbacks = new Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>();
        }

        /// <summary>
        /// Event raised after creation of grid fields.
        /// </summary>
        public event EventHandler<FieldsCreatedEventArgs> FieldsCreated;

        /// <summary>
        /// Gets or sets the navigation callbacks.
        /// </summary>
        public Dictionary<string, KeyValuePair<string, Action>> NavigationCallbacks
        {
            get
            {
                return (Dictionary<string, KeyValuePair<string, Action>>)this.GetValue(NavigationCallbacksProperty);
            }

            set
            {
                this.SetValue(NavigationCallbacksProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        public ObservableCollection<PropertyGridGroup> Groups
        {
            get
            {
                return (ObservableCollection<PropertyGridGroup>)this.GetValue(GroupsProperty);
            }

            set
            {
                this.SetValue(GroupsProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual label column width.
        /// </summary>
        public double ActualLabelColumnWidth
        {
            get
            {
                return (double)this.GetValue(ActualLabelColumnWidthProperty);
            }

            internal set
            {
                this.SetValue(ActualLabelColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the search box help text.
        /// </summary>
        public string SearchBoxHelpText
        {
            get
            {
                return (string)this.GetValue(SearchBoxHelpTextProperty);
            }

            set
            {
                this.SetValue(SearchBoxHelpTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether has search box.
        /// </summary>
        public bool HasSearchBox
        {
            get
            {
                return (bool)this.GetValue(HasSearchBoxProperty);
            }

            set
            {
                this.SetValue(HasSearchBoxProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic data converter.
        /// </summary>
        public IValueConverter DynamicDataConverter
        {
            get
            {
                return (IValueConverter)this.GetValue(DynamicDataConverterProperty);
            }

            set
            {
                this.SetValue(DynamicDataConverterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item context menu.
        /// </summary>
        public ContextMenu ItemContextMenu
        {
            get
            {
                return (ContextMenu)this.GetValue(ContextMenuProperty);
            }

            set
            {
                this.SetValue(ContextMenuProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the domain value collection
        /// </summary>
        public Dictionary<string, ICollection> DomainValueCollection
        {
            get
            {
                return (Dictionary<string, ICollection>)this.GetValue(DomainValueCollectionProperty);
            }

            set
            {
                this.SetValue(DomainValueCollectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the editor callbacks
        /// </summary>
        public Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>> EditorCallbacks
        {
            get
            {
                return
                    (Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>)
                    this.GetValue(EditorCallbacksProperty);
            }

            set
            {
                this.SetValue(EditorCallbacksProperty, value);
            }
        }

        /// <summary>
        /// a helper method to convert a value including parsing
        /// </summary>
        /// <param name="concreteValue">the concrete value</param>
        /// <param name="datasource">the data source</param>
        /// <param name="property">the property</param>
        /// <param name="element">the element</param>
        public static void SetConvertedValue(
            object concreteValue, object datasource, PropertyInfo property, object element)
        {
            var dataValue = concreteValue as IDataValue;
            if (dataValue != null)
            {
                if (dataValue.ValueObject is TimeSpan)
                {
                    dataValue.ValueObject = TimeSpan.Parse(datasource.ToString());
                }
                else
                {
                    dataValue.ValueObject = datasource;
                }
            }

            property.SetValue(element, concreteValue, new object[0]);
        }

        /// <summary>
        /// Compare function for IDataValue and other objects
        /// </summary>
        /// <param name="data">the data</param>
        /// <param name="otherData">the other data</param>
        /// <returns>a value indicating whether the objects are equal</returns>
        public static bool CompareValue(object data, object otherData)
        {
            bool result;

            if (data is IDynamicDataValue && otherData is IDynamicDataValue)
            {
                if (((IDynamicDataValue)otherData).Formula == null && ((IDynamicDataValue)data).Formula == null)
                {
                    result = CompareDataValue(data, otherData);
                }
                else if (((IDynamicDataValue)otherData).Formula == null || ((IDynamicDataValue)data).Formula == null)
                {
                    result = false;
                }
                else
                {
                    result = ((IDynamicDataValue)otherData).Formula.Equals(((IDynamicDataValue)data).Formula);
                }
            }
            else if (data is IDataValue && otherData is IDataValue)
            {
                result = CompareDataValue(data, otherData);
            }
            else if (otherData == null && data == null)
            {
                result = true;
            }
            else if (otherData == null || data == null)
            {
                result = false;
            }
            else
            {
                result = otherData.Equals(data);
            }

            return result;
        }

        /// <summary>
        /// Compare function for IDataValue and other objects
        /// </summary>
        /// <param name="data">the data</param>
        /// <param name="otherData">the other data</param>
        /// <returns>a value indicating whether the objects are equal</returns>
        public static bool CompareDataValue(object data, object otherData)
        {
            bool result;
            if (((IDataValue)otherData).ValueObject == null && ((IDataValue)data).ValueObject == null)
            {
                result = true;
            }
            else if (((IDataValue)otherData).ValueObject == null || ((IDataValue)data).ValueObject == null)
            {
                result = false;
            }
            else
            {
                result = ((IDataValue)otherData).ValueObject.Equals(((IDataValue)data).ValueObject);
            }

            return result;
        }

        /// <summary>
        /// The add field.
        /// </summary>
        /// <param name="fieldname">
        /// The fieldname.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="itemPropertyChanged">
        /// The item property changed.
        /// </param>
        public void AddField(
            string fieldname,
            PropertyGridItem item,
            Action<PropertyGridItem, object> itemPropertyChanged)
        {
            this.propertyItemCache.Add(fieldname, item);
            item.DataSourceChanged += itemPropertyChanged;
        }

        /// <summary>
        /// Adds the collection to the Domain value collection
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="collection">the collection to be added</param>
        public void AddDomainValue(string name, ICollection collection)
        {
            if (!this.DomainValueCollection.ContainsKey(name))
            {
                this.DomainValueCollection.Add(name, null);
            }

            this.DomainValueCollection[name] = collection;
        }

        /// <summary>
        /// The remove field.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="itemPropertyChanged">
        /// The item property changed.
        /// </param>
        public void RemoveField(
            PropertyGridItem item,
            Action<PropertyGridItem, object> itemPropertyChanged)
        {
            item.DataSourceChanged -= itemPropertyChanged;
        }

        /// <summary>
        /// Clear a dynamic added fields from the grid.
        /// </summary>
        /// <param name="itemPropertyChanged">
        /// The item Property Changed.
        /// </param>
        public void Clear(Action<PropertyGridItem, object> itemPropertyChanged)
        {
            this.propertyItemCache.Clear();

            foreach (var gridGroup in this.Groups)
            {
                foreach (var item in gridGroup.Items)
                {
                    var gridItem = item as PropertyGridItem;
                    if (gridItem != null)
                    {
                        this.RemoveField(gridItem, itemPropertyChanged);
                    }
                }
            }

            this.Groups.Clear();
        }

        /// <summary>
        /// True if dynamic property with name key is present.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasDynamicProperty(string key)
        {
            return this.propertyItemCache.ContainsKey(key);
        }

        /// <summary>
        /// The set wrapped data source.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetWrappedDataSource(string key, object value)
        {
            this.propertyItemCache[key].WrappedDataSource.Value = value;
        }

        /// <summary>
        /// The update content.
        /// </summary>
        /// <param name="elementCollection">
        /// The elements.
        /// </param>
        /// <param name="itemPropertyChanged">
        /// The item Property Changed.
        /// </param>
        /// <param name="translate">
        /// The translate function
        /// </param>
        /// <param name="getTooltip">
        /// A function providing a tooltip or null if none can be given.
        /// </param>
        /// <param name="multiSelectFields">
        /// The multi selection fields.
        /// </param>
        /// <param name="filter">
        /// The filter to hide specific properties
        /// </param>
        /// <typeparam name="T">
        /// the type of the contained elements.
        /// </typeparam>
        public void UpdateContent<T>(
            ObservableCollection<T> elementCollection,
            Action<PropertyGridItem, object> itemPropertyChanged,
            Func<string, string> translate,
            Func<string, string> getTooltip,
            IEnumerable<string> multiSelectFields = null,
            string filter = null)
            where T : class
        {
            this.Clear(itemPropertyChanged);

            var elementList = elementCollection.Where(e => e != null).ToList();
            var elementCount = elementList.Count;

            var fields = from element in elementList
                         from propertyInfo in element.GetType().GetProperties()
                         let uservisibleAttribute = GetUserVisiblePropertyAttribute(propertyInfo)
                         where uservisibleAttribute != null
                         where MatchFilter(filter, uservisibleAttribute.Filter)
                         where AllHaveProperty(propertyInfo.Name, elementList)
                         select
                             new GridFieldData(
                             element,
                             propertyInfo,
                             uservisibleAttribute.GroupName,
                             translate(uservisibleAttribute.FieldName ?? propertyInfo.Name),
                             getTooltip(uservisibleAttribute.FieldName ?? propertyInfo.Name),
                             uservisibleAttribute.OrderIndex,
                             uservisibleAttribute.GroupOrderIndex,
                             multiSelectFields,
                             uservisibleAttribute.FormatString);

            var fieldsList = fields.Distinct(new GridFieldDataComparer()).ToList();
            this.RaiseFieldsCreated(elementList, fieldsList);
            var groupedFields = fieldsList.GroupBy(f => new Tuple<string, int>(f.GroupName, f.GroupOrderIndex));
            var orderedFieldGroups = groupedFields.OrderBy(fg => fg.Key.Item2).ThenBy(fg => fg.Key.Item1);
            this.Clear(itemPropertyChanged);
            foreach (var fieldGroup in orderedFieldGroups)
            {
                var orderedFieldGroup = fieldGroup.OrderBy(f => f.OrderIndex).ThenBy(f => f.FieldName);
                var gridGroup = new PropertyGridGroup
                                {
                                    Header = translate(fieldGroup.Key.Item1),
                                    ToolTip = getTooltip(fieldGroup.Key.Item1),
                                };

                foreach (var item in orderedFieldGroup)
                {
                    var field = item;
                    var dataSource = field.Data;

                    // multiple items selected, use one DataValue for all
                    var dataIsSameForAll = this.DataIsSameForAll(dataSource, field, elementList);
                    var hasMultipleDifferingDataSources = !dataIsSameForAll && elementList.Count > 1;
                    if (hasMultipleDifferingDataSources)
                    {
                        dataSource = SetupMultipleDifferingDataSources(field, dataSource);
                    }

                    PropertyGridItem propertyGridItem;
                    if (dataSource == null)
                    {
                        propertyGridItem = this.CreateDefaultPropertyGridItem(
                            field, dataSource, hasMultipleDifferingDataSources, elementCount);
                    }
                    else
                    {
                        propertyGridItem = this.CreateDataValuePropertyGridItem(
                            dataSource,
                            field,
                            hasMultipleDifferingDataSources,
                            elementCount,
                            elementList.FirstOrDefault());

                        if (dataSource.ValueObject is Enum)
                        {
                            propertyGridItem.DomainObject = Enum.GetValues(dataSource.ValueObject.GetType());
                        }
                    }

                    this.AddField(field.FieldName, propertyGridItem, itemPropertyChanged);
                    gridGroup.Items.Add(propertyGridItem);
                }

                this.Groups.Add(gridGroup);
            }
        }

        /// <summary>
        /// The focus search box.
        /// </summary>
        public void FocusSearchBox()
        {
            this.searchBox.Focus();
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        public void Search(string searchText)
        {
            foreach (var subGroup in this.Groups)
            {
                subGroup.Search(searchText);
            }
        }

        /// <summary>
        /// The clear search.
        /// </summary>
        public void ClearSearch()
        {
            foreach (var subGroup in this.Groups)
            {
                subGroup.ClearSearch();
            }
        }

        /// <summary>
        /// The data is same for all.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <typeparam name="T"> the items
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DataIsSameForAll<T>(object data, GridFieldData field, IEnumerable<T> elements)
        {
            return
                elements.Select(field.GetValueForProperty)
                        .All(otherData => CompareValue(data, otherData));
        }

        /// <summary>
        /// The data is same for all.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <typeparam name="T"> the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DataIsSameForAll<T>(object data, PropertyInfo property, IEnumerable<T> elements)
        {
            return elements.Select(p => property.GetValue(p, null)).All(otherData => otherData.Equals(data));
        }

        /// <summary>
        /// The on mouse right button up.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            // workaround: the property grid shows a invalid context menu on rigth click (even on the background)
            // only allow a rigth click context menu for TextBox
            // also TextBoxView is internal and no TextBox
            if (e.OriginalSource.GetType().FullName == "System.Windows.Controls.TextBoxView")
            {
                base.OnMouseRightButtonUp(e);
                return;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Raises the <see cref="FieldsCreated"/> event.
        /// </summary>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        protected virtual void RaiseFieldsCreated(IEnumerable elements, IList<GridFieldData> fields)
        {
            this.RaiseFieldsCreated(new FieldsCreatedEventArgs(elements, fields));
        }

        private static UserVisiblePropertyAttribute GetUserVisiblePropertyAttribute(PropertyInfo propertyInfo)
        {
            return (UserVisiblePropertyAttribute)
                   Attribute.GetCustomAttribute(propertyInfo, typeof(UserVisiblePropertyAttribute));
        }

        private static IDataValue SetupMultipleDifferingDataSources(GridFieldData field, IDataValue dataSource)
        {
            if (field.Data.ValueObject is bool)
            {
                dataSource = (IDataValue)Activator.CreateInstance(field.Data.GetType());
                dataSource.ValueObject = false;
            }
            else if (field.Data.ValueObject is int)
            {
                dataSource = null;
            }
            else if (field.Data.ValueObject is Enum)
            {
                dataSource = (IDataValue)Activator.CreateInstance(field.Data.GetType());
                var currentenum = dataSource.ValueObject as Enum;
                if (currentenum != null)
                {
                    var defaultEnum = Activator.CreateInstance(currentenum.GetType());
                    dataSource.ValueObject = defaultEnum;
                }
                else
                {
                    dataSource.ValueObject = null;
                }
            }
            else
            {
                dataSource = (IDataValue)Activator.CreateInstance(field.Data.GetType());
            }

            return dataSource;
        }

        private static bool AllHaveProperty<T>(string name, IEnumerable<T> elements)
        {
            return elements
                .Select(element => element.GetType().GetProperties())
                .All(properties => properties.Select(p => p.Name)
                                             .Contains(name));
        }

        private static bool MatchFilter(string filter, string userVisibileAttributeFilter)
        {
            if (string.IsNullOrEmpty(userVisibileAttributeFilter) || string.IsNullOrEmpty(filter))
            {
                return true;
            }

            var chunks = userVisibileAttributeFilter.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return
                chunks.Any(
                    chunk => filter.Equals(chunk, StringComparison.InvariantCultureIgnoreCase));
        }

        private PropertyGridItem CreateDefaultPropertyGridItem(
            GridFieldData field,
            IDataValue dataSource,
            bool hasMultipleDifferingDataSources,
            int elementCount)
        {
            var dynamicDataSource = dataSource as IDynamicDataValue;
            var hasContextButton = dynamicDataSource != null && elementCount <= 1;
            var propertyGridItem = new PropertyGridItem
                                                    {
                                                        Header = field.Header,
                                                        ToolTip = field.Tooltip,
                                                        Name = field.FieldName,
                                                        DataSource = field.Reference,
                                                        HasContextButton = hasContextButton,
                                                        Tag = dataSource,
                                                        IsMultiSelect = field.IsMultiSelect,
                                                        DomainObject = this.GetDomainValueForFieldName(field.FieldName),
                                                        ActionCallback = this.GetActionCallbackForField(field),
                                                        HasMultipleDifferingDataSources =
                                                          hasMultipleDifferingDataSources,
                                                    };

            // DataSource change Event was not called due to no change in data
            if (propertyGridItem.DataSource == null)
            {
                propertyGridItem.HandleDataSourceChange();
            }

            return propertyGridItem;
        }

        private PropertyGridItem CreateDataValuePropertyGridItem(
            IDataValue dataSource,
            GridFieldData field,
            bool hasMultipleDifferingDataSources,
            int elementCount,
            object parentElement)
        {
            var dynamicDataSource = dataSource as IDynamicDataValue;
            var hasContextButton = dynamicDataSource != null && elementCount <= 1;
            var animatedDynamicDataSource = dataSource as IAnimatedDataValue;

            object parallelData = null;
            IValueConverter dataConverter = null;

            object propertyValue;
            if (dynamicDataSource != null && dynamicDataSource.Formula != null)
            {
                propertyValue = dynamicDataSource.Formula;
                dataConverter = this.DynamicDataConverter;
            }
            else
            {
                propertyValue = dataSource.ValueObject;
            }

            if (animatedDynamicDataSource != null && animatedDynamicDataSource.Animation != null)
            {
                parallelData = animatedDynamicDataSource.Animation;
            }

            var actionCallback = this.GetActionCallbackForField(field);
            string navigationText;
            var navigationCallback = this.GetNavigationCallbackForField(field, out navigationText);
            var propertyGridItem = new PropertyGridItem
                                                    {
                                                        Header = field.Header,
                                                        ToolTip = field.Tooltip,
                                                        Name = field.FieldName,
                                                        DataSource = propertyValue,
                                                        ParallelData = parallelData,
                                                        DataConverter = dataConverter,
                                                        HasContextButton = hasContextButton,
                                                        Menu = this.ItemContextMenu,
                                                        Tag = dataSource,
                                                        ActionCallback = actionCallback,
                                                        IsMultiSelect = field.IsMultiSelect,
                                                        DomainObject = this.GetDomainValueForFieldName(field.FieldName),
                                                        HasMultipleDifferingDataSources =
                                                            hasMultipleDifferingDataSources,
                                                            ParentObject = parentElement,
                                                            NavigateButtonAction = navigationCallback,
                                                            NavigateButtonText = navigationText,
                                                            FormatString = field.Formatstring
                                                    };
            return propertyGridItem;
        }

        private ICollection GetDomainValueForFieldName(string name)
        {
            ICollection result = null;

            if (this.DomainValueCollection.ContainsKey(name))
            {
                result = this.DomainValueCollection[name];
            }

            return result;
        }

        /// <summary>
        /// The property grid search box_ on searched.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SearchBoxOnSearched(object sender, SearchBox.PropertyGridSearchEventArgs e)
        {
            var searchText = e.Text;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                this.Search(searchText);
            }
            else
            {
                this.ClearSearch();
            }
        }

        /// <summary>
        /// The search box clear search.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SearchBoxClearSearch(object sender, EventArgs e)
        {
            this.ClearSearch();
        }

        private void RaiseFieldsCreated(FieldsCreatedEventArgs e)
        {
            var handler = this.FieldsCreated;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        private Action<PropertyGridItem, PropertyGridItemDataSource> GetActionCallbackForField(GridFieldData field)
        {
            Action<PropertyGridItem, PropertyGridItemDataSource> result = null;

            Type referenceType = null;
            if (field.Reference != null)
            {
                referenceType = field.Reference.GetType();
            }

            if (referenceType != null && this.EditorCallbacks.ContainsKey(referenceType))
            {
                result = this.EditorCallbacks[referenceType];
            }

            return result;
        }

        private Action GetNavigationCallbackForField(GridFieldData field, out string navigationText)
        {
            Action result = null;
            navigationText = string.Empty;
            if (this.NavigationCallbacks != null && this.NavigationCallbacks.ContainsKey(field.FieldName))
            {
                result = this.NavigationCallbacks[field.FieldName].Value;
                navigationText = this.NavigationCallbacks[field.FieldName].Key;
            }

            return result;
        }

        private class GridFieldDataComparer : IEqualityComparer<GridFieldData>
        {
            public bool Equals(GridFieldData x, GridFieldData y)
            {
                return x.FieldName.Equals(y.FieldName);
            }

            public int GetHashCode(GridFieldData obj)
            {
                return obj.FieldName.GetHashCode();
            }
        }
    }
}
