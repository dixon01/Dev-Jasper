// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The property grid base.
    /// </summary>
    public class PropertyGridBase : UserControl
    {
        /// <summary>
        /// The search box style property.
        /// </summary>
        public static readonly DependencyProperty SearchBoxStyleProperty = DependencyProperty.Register(
            "SearchBoxStyle", typeof(Style), typeof(PropertyGridBase), new PropertyMetadata(default(Style)));

        /// <summary>
        /// Gets or sets the search box style.
        /// </summary>
        public Style SearchBoxStyle
        {
            get
            {
                return (Style)this.GetValue(SearchBoxStyleProperty);
            }

            set
            {
                this.SetValue(SearchBoxStyleProperty, value);
            }
        }
    }
}
