// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridItemTemplateSelector.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridItemTemplateSelector.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The property grid item template selector.
    /// </summary>
    public class PropertyGridItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The select template.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="DataTemplate"/>.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate result = null;

            var frameworkElement = container as FrameworkElement;

            if (frameworkElement != null)
            {
                if (item != null)
                {
                    var customTemplate = frameworkElement.Tag as DataTemplate;

                    if (customTemplate != null)
                    {
                        result = customTemplate;
                    }
                    else
                    {
                        var dataSource = item as PropertyGridItemDataSource;
                        if (dataSource != null)
                        {
                            if (dataSource.IsMultiSelect)
                            {
                                result = frameworkElement.FindResource("PropertyGridMultiSelectEditor") as DataTemplate;
                            }
                            else if (dataSource.Value is Enum || dataSource.DomainObject != null)
                            {
                                result =
                                    frameworkElement.FindResource("PropertyGridSingleSelectEditor") as DataTemplate;
                            }
                            else if (dataSource.Value is string)
                            {
                                result = frameworkElement.FindResource("PropertyGridTextEditor") as DataTemplate;
                            }
                            else if (dataSource.Value is bool)
                            {
                                result = frameworkElement.FindResource("PropertyGridBoolEditor") as DataTemplate;
                            }
                            else if (dataSource.Value is int)
                            {
                                result = frameworkElement.FindResource("PropertyGridIntEditor") as DataTemplate;
                            }
                            else if (dataSource.Value is DateTime)
                            {
                                result = frameworkElement.FindResource("PropertyGridDateEditor") as DataTemplate;
                            }
                            else if (dataSource.Value is IList || dataSource.Value is ITriggerProperty)
                            {
                                result = frameworkElement.FindResource("PropertyGridActionEditor") as DataTemplate;
                            }
                            else
                            {
                                result = frameworkElement.FindResource("PropertyGridTextEditor") as DataTemplate;
                            }
                        }
                        else
                        {
                            result = frameworkElement.FindResource("PropertyGridTextEditor") as DataTemplate;
                        }
                    }
                }
                else
                {
                    result = frameworkElement.FindResource("PropertyGridTextEditor") as DataTemplate;
                }
            }

            return result;
        }
    }
}
