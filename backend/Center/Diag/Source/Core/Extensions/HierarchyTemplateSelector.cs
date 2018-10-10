// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HierarchyTemplateSelector.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The HierarchyTemplateSelector.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Extensions
{
    using System.Windows;
    using System.Windows.Controls;

    using Gorba.Center.Diag.Core.ViewModels.FileSystem;

    /// <summary>
    /// The hierarchy template selector.
    /// </summary>
    public class HierarchyTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the folder template.
        /// </summary>
        public DataTemplate FolderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the file template.
        /// </summary>
        public DataTemplate FileTemplate { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="DataTemplate"/> based on custom logic.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is FolderViewModel)
            {
                return this.FolderTemplate;
            }

            if (item is FileViewModel)
            {
                return this.FileTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
