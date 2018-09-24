// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridGroup.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridGroup.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Interaction logic for PropertyGridGroup
    /// </summary>
    [ContentProperty("Items")]
    public partial class PropertyGridGroup
    {
        /// <summary>
        /// The name property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(string), typeof(PropertyGridGroup), new PropertyMetadata(default(string)));

        /// <summary>
        /// The items property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(ObservableCollection<PropertyGridItemBase>),
            typeof(PropertyGridGroup),
            new PropertyMetadata(default(ObservableCollection<PropertyGridItemBase>)));

        /// <summary>
        /// The is expanded property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof(bool), typeof(PropertyGridGroup), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The header tool tip property.
        /// </summary>
        public static readonly DependencyProperty HeaderToolTipProperty = DependencyProperty.Register(
            "HeaderToolTip", typeof(string), typeof(PropertyGridGroup), new PropertyMetadata(default(string)));

        /// <summary>
        /// The content margin property.
        /// </summary>
        public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register(
            "ContentMargin",
            typeof(Thickness),
            typeof(PropertyGridGroup),
            new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// The before search expander state.
        /// </summary>
        private bool beforeSearchIsExpandedState;

        /// <summary>
        /// The last search text.
        /// </summary>
        private string lastSearchText;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridGroup"/> class.
        /// </summary>
        public PropertyGridGroup()
        {
            this.InitializeComponent();

            this.Items = new ObservableCollection<PropertyGridItemBase>();
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Header
        {
            get
            {
                return (string)this.GetValue(HeaderProperty);
            }

            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public ObservableCollection<PropertyGridItemBase> Items
        {
            get
            {
                return (ObservableCollection<PropertyGridItemBase>)this.GetValue(ItemsProperty);
            }

            set
            {
                this.SetValue(ItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(IsExpandedProperty);
            }

            set
            {
                this.SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header tool tip.
        /// </summary>
        public string HeaderToolTip
        {
            get
            {
                return (string)this.GetValue(HeaderToolTipProperty);
            }

            set
            {
                this.SetValue(HeaderToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content margin.
        /// </summary>
        public Thickness ContentMargin
        {
            get
            {
                return (Thickness)this.GetValue(ContentMarginProperty);
            }

            set
            {
                this.SetValue(ContentMarginProperty, value);
            }
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        public void Search(string searchText)
        {
            if (this.lastSearchText == searchText)
            {
                return;
            }

            var hasFoundSome = false;

            foreach (var item in this.Items)
            {
                if (item.Search(searchText))
                {
                    hasFoundSome = true;
                }
            }

            if (hasFoundSome)
            {
                this.Visibility = Visibility.Visible;
                this.beforeSearchIsExpandedState = this.GroupExpander.IsExpanded;
                this.GroupExpander.IsExpanded = true;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }

            this.lastSearchText = searchText;
        }

        /// <summary>
        /// The clear search.
        /// </summary>
        public void ClearSearch()
        {
            if (!string.IsNullOrWhiteSpace(this.lastSearchText))
            {
                foreach (var item in this.Items)
                {
                    item.ClearSearch();
                }

                this.Visibility = Visibility.Visible;
                this.GroupExpander.IsExpanded = this.beforeSearchIsExpandedState;

                this.lastSearchText = null;
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
            this.beforeSearchIsExpandedState = this.GroupExpander.IsExpanded;
        }
    }
}
