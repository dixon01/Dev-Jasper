// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerEditorParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TriggerEditorParameters.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;

    /// <summary>
    /// The trigger editor parameters
    /// </summary>
    public class TriggerEditorParameters
    {
        /// <summary>
        /// Gets or sets the callback
        /// </summary>
        public PropertyGridItemDataSource DataSource { get; set; }

        /// <summary>
        /// Gets or sets the item
        /// </summary>
        public PropertyGridItem Item { get; set; }
    }
}