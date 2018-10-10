// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridSingleSelectEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridSingleSelectEditor.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The property grid text editor.
    /// </summary>
    public partial class PropertyGridSingleSelectEditor
    {
        /// <summary>
        /// The action callback
        /// </summary>
        public static readonly DependencyProperty ActionCallbackProperty = DependencyProperty.Register(
            "ActionCallback",
            typeof(Action<PropertyGridItem, PropertyGridItemDataSource>),
            typeof(PropertyGridSingleSelectEditor),
            new PropertyMetadata(default(Action<PropertyGridItem, PropertyGridItemDataSource>)));

        /// <summary>
        /// the property grid item property
        /// </summary>
        public static readonly DependencyProperty PropertyGridItemProperty =
            DependencyProperty.Register(
                "PropertyGridItem",
                typeof(PropertyGridItem),
                typeof(PropertyGridSingleSelectEditor),
                new PropertyMetadata(default(PropertyGridItem)));

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridSingleSelectEditor"/> class.
        /// </summary>
        public PropertyGridSingleSelectEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the action callback
        /// </summary>
        public Action<PropertyGridItem, PropertyGridItemDataSource> ActionCallback
        {
            get
            {
                return (Action<PropertyGridItem, PropertyGridItemDataSource>)GetValue(ActionCallbackProperty);
            }

            set
            {
                SetValue(ActionCallbackProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the property grid item
        /// </summary>
        public PropertyGridItem PropertyGridItem
        {
            get
            {
                return (PropertyGridItem)GetValue(PropertyGridItemProperty);
            }

            set
            {
                SetValue(PropertyGridItemProperty, value);
            }
        }

        private void OnNavigateButtonClicked(object sender, RoutedEventArgs e)
        {
            var context = (PropertyGridItemDataSource)this.DataContext;
            if (context.NavigateButtonCommand != null)
            {
                context.NavigateButtonCommand.Execute(null);
            }

            ValueComboBox.IsDropDownOpen = false;
        }
    }
}