// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridGroupBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridGroupBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The property grid group base.
    /// </summary>
    public class PropertyGridGroupBase : UserControl
    {
        /// <summary>
        /// The header toggle button template property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderTemplateProperty =
            DependencyProperty.Register(
                "GroupHeaderTemplate",
                typeof(ControlTemplate),
                typeof(PropertyGridGroupBase),
                new PropertyMetadata(default(ControlTemplate)));

        /// <summary>
        /// Gets or sets the header toggle button template.
        /// </summary>
        public ControlTemplate GroupHeaderTemplate
        {
            get
            {
                return (ControlTemplate)this.GetValue(GroupHeaderTemplateProperty);
            }

            set
            {
                this.SetValue(GroupHeaderTemplateProperty, value);
            }
        }
    }
}
