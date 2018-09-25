// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDefinitionExtended.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColumnDefinitionExtended type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Extensions
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Extension of <see cref="ColumnDefinition"/> that supports hiding a column.
    /// <seealso cref="http://www.codeproject.com/Articles/437237/WPF-Grid-Column-and-Row-Hiding"/>
    /// </summary>
    public class ColumnDefinitionExtended : ColumnDefinition
    {
        /// <summary>
        /// The visible property.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty;

        static ColumnDefinitionExtended()
        {
            VisibleProperty = DependencyProperty.Register(
                "Visible",
                typeof(bool),
                typeof(ColumnDefinitionExtended),
                new PropertyMetadata(true, OnVisibleChanged));

            WidthProperty.OverrideMetadata(
                typeof(ColumnDefinitionExtended),
                new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), null, CoerceWidth));

            MinWidthProperty.OverrideMetadata(
                typeof(ColumnDefinitionExtended),
                new FrameworkPropertyMetadata((double)0, null, CoerceMinWidth));
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column is visible.
        /// </summary>
        public bool Visible
        {
            get
            {
                return (bool)this.GetValue(VisibleProperty);
            }

            set
            {
                this.SetValue(VisibleProperty, value);
            }
        }

        private static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.CoerceValue(ColumnDefinition.WidthProperty);
            obj.CoerceValue(ColumnDefinition.MinWidthProperty);
        }

        private static object CoerceWidth(DependencyObject obj, object value)
        {
            return ((ColumnDefinitionExtended)obj).Visible ? value : new GridLength(0);
        }

        private static object CoerceMinWidth(DependencyObject obj, object value)
        {
            return ((ColumnDefinitionExtended)obj).Visible ? value : (double)0;
        }
    }
}
