// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportMenuItemAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using System;
    using System.ComponentModel.Composition;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Exports a menu item with a defined index for sorting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttribute]
    public sealed class ExportMenuItemAttribute : ExportAttribute, IMenuItemMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportMenuItemAttribute"/> class.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public ExportMenuItemAttribute(int index = 0)
            : base(typeof(MenuItemBase))
        {
            this.Index = index;
        }

        /// <summary>
        /// Gets the index in the displayed list of stages.
        /// </summary>
        public int Index { get; private set; }
    }
}
