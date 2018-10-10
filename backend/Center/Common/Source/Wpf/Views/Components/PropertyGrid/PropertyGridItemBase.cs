// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridItemBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The property grid item base.
    /// </summary>
    public class PropertyGridItemBase : UserControl
    {
        /// <summary>
        /// The context button template property.
        /// </summary>
        public static readonly DependencyProperty ContextButtonTemplateProperty =
            DependencyProperty.Register(
                "ContextButtonTemplate",
                typeof(ControlTemplate),
                typeof(PropertyGridItemBase),
                new PropertyMetadata(default(ControlTemplate)));

        /// <summary>
        /// The menu property.
        /// </summary>
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu", typeof(ContextMenu), typeof(PropertyGridItemBase), new PropertyMetadata(default(ContextMenu)));

        /// <summary>
        /// The editor template property.
        /// </summary>
        public static readonly DependencyProperty EditorTemplateProperty = DependencyProperty.Register(
            "EditorTemplate",
            typeof(DataTemplate),
            typeof(PropertyGridItemBase),
            new PropertyMetadata(default(ControlTemplate)));

        /// <summary>
        /// The header property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(string), typeof(PropertyGridItemBase), new PropertyMetadata(default(string)));

        /// <summary>
        /// The header tool tip property.
        /// </summary>
        public static readonly DependencyProperty HeaderToolTipProperty = DependencyProperty.Register(
            "HeaderToolTip", typeof(string), typeof(PropertyGridItemBase), new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets the context button template.
        /// </summary>
        public ControlTemplate ContextButtonTemplate
        {
            get
            {
                return (ControlTemplate)this.GetValue(ContextButtonTemplateProperty);
            }

            set
            {
                this.SetValue(ContextButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        public ContextMenu Menu
        {
            get
            {
                return (ContextMenu)this.GetValue(MenuProperty);
            }

            set
            {
                this.SetValue(MenuProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the editor template.
        /// </summary>
        public DataTemplate EditorTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(EditorTemplateProperty);
            }

            set
            {
                this.SetValue(EditorTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header.
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
        /// The search.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool Search(string searchText)
        {
            return false;
        }

        /// <summary>
        /// The clear search.
        /// </summary>
        public virtual void ClearSearch()
        {
            this.Visibility = Visibility.Visible;
        }
    }
}
