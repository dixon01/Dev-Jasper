// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingWindow.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TrackingWindow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views
{
    using System.Collections.ObjectModel;
    using System.Windows;

    /// <summary>
    /// Interaction logic for TrackingWindow
    /// </summary>
    public class TrackingWindow : Window
    {
        /// <summary>
        /// the is dirty property
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
            "IsDirty",
            typeof(bool),
            typeof(TrackingWindow),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// The list of items to be displayed in the menu
        /// </summary>
        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register(
            "MenuItems",
            typeof(Collection<UIElement>),
            typeof(TrackingWindow),
            new PropertyMetadata(new Collection<UIElement>()));

        /// <summary>
        /// The login information
        /// </summary>
        public static readonly DependencyProperty LoginInformationProperty = DependencyProperty.Register(
            "LoginInformation",
            typeof(UIElement),
            typeof(TrackingWindow),
            new PropertyMetadata(default(UIElement)));

        /// <summary>
        /// The property for the Show Shroud mechanic
        /// </summary>
        public static readonly DependencyProperty ShowShroudProperty = DependencyProperty.Register(
            "ShowShroud",
            typeof(bool),
            typeof(TrackingWindow),
            new PropertyMetadata(false));

        /// <summary>
        /// The property for the top most layer
        /// </summary>
        public static readonly DependencyProperty TopMostLayerProperty = DependencyProperty.Register(
            "TopMostLayer",
            typeof(UIElement),
            typeof(TrackingWindow),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether the shroud should be displayed
        /// </summary>
        public bool ShowShroud
        {
            get
            {
                return (bool)this.GetValue(ShowShroudProperty);
            }

            set
            {
                this.SetValue(ShowShroudProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the top most layer
        /// </summary>
        public UIElement TopMostLayer
        {
            get
            {
                return (UIElement)this.GetValue(TopMostLayerProperty);
            }

            set
            {
                this.SetValue(TopMostLayerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the list of items to be displayed in the menu
        /// </summary>
        public Collection<UIElement> MenuItems
        {
            get
            {
                return (Collection<UIElement>)this.GetValue(MenuItemsProperty);
            }

            set
            {
                this.SetValue(MenuItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the list of items to be displayed in the menu
        /// </summary>
        public UIElement LoginInformation
        {
            get
            {
                return (UIElement)this.GetValue(LoginInformationProperty);
            }

            set
            {
                this.SetValue(LoginInformationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window has changes
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return (bool)this.GetValue(TrackingWindow.IsDirtyProperty);
            }

            set
            {
                this.SetValue(TrackingWindow.IsDirtyProperty, value);
            }
        }
    }
}
