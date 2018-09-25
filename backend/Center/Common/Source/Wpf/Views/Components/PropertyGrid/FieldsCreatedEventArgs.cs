// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldsCreatedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FieldsCreatedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The fields created event args.
    /// </summary>
    [Serializable]
    public class FieldsCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsCreatedEventArgs"/> class.
        /// </summary>
        /// <param name="elements">
        /// The elements used to generate the grid fields.
        /// </param>
        /// <param name="fields">
        /// The fields of the grid.
        /// </param>
        public FieldsCreatedEventArgs(IEnumerable elements, IList<GridFieldData> fields)
        {
            this.Elements = elements;
            this.Fields = fields;
        }

        /// <summary>
        /// Gets the elements used to generate the grid fields.
        /// </summary>
        public IEnumerable Elements { get; private set; }

        /// <summary>
        /// Gets the fields of the grid.
        /// </summary>
        public IList<GridFieldData> Fields { get; private set; }
    }
}