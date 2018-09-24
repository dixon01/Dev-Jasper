// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridLowPrioritySection.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridLowPrioritySection.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Interaction logic for PropertyGridLowPrioritySection
    /// </summary>
    [ContentProperty("Items")]
    public partial class PropertyGridLowPrioritySection
    {
        /// <summary>
        /// The items property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(ObservableCollection<PropertyGridItemBase>),
            typeof(PropertyGridLowPrioritySection),
            new PropertyMetadata(default(ObservableCollection<PropertyGridItemBase>)));

        /// <summary>
        /// The is expanded property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof(bool), typeof(PropertyGridLowPrioritySection), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The before search expander state.
        /// </summary>
        private bool beforeSearchIsExpandedState;

        /// <summary>
        /// The last search text.
        /// </summary>
        private string lastSearchText;

        /// <summary>
        /// The last search result.
        /// </summary>
        private bool? lastSearchResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridLowPrioritySection"/> class.
        /// </summary>
        public PropertyGridLowPrioritySection()
        {
            this.InitializeComponent();

            this.Items = new ObservableCollection<PropertyGridItemBase>();
            this.Loaded += this.OnLoaded;
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
            if (this.lastSearchResult != null && this.lastSearchText == searchText)
            {
                return this.lastSearchResult.Value;
            }

            var hasFoundSome = false;

            var headerText = this.Header != null ? this.Header.ToLower() : string.Empty;
            var headerTooltipText = this.HeaderToolTip != null ? this.HeaderToolTip.ToLower() : string.Empty;

            if (headerText.Contains(searchText.ToLower()) ||
                headerTooltipText.Contains(searchText.ToLower()))
            {
                hasFoundSome = true;
            }

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
            this.lastSearchResult = hasFoundSome;

            return hasFoundSome;
        }

        /// <summary>
        /// The clear search.
        /// </summary>
        public override void ClearSearch()
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
                this.lastSearchResult = null;
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
