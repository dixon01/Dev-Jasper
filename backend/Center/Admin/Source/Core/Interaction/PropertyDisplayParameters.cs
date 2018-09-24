// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDisplayParameters.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyDisplayParameters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Interaction
{
    using Gorba.Center.Admin.Core.DataViewModels;

    /// <summary>
    /// Parameters to the <see cref="CommandCompositionKeys.Shell.Editor.UpdatePropertyDisplay"/> command.
    /// </summary>
    public class PropertyDisplayParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDisplayParameters"/> class.
        /// </summary>
        /// <param name="editingEntity">
        /// The editing entity.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public PropertyDisplayParameters(DataViewModelBase editingEntity, string propertyName)
        {
            this.EditingEntity = editingEntity;
            this.PropertyName = propertyName;
            this.DisplayName = propertyName;
            this.IsVisible = true;
        }

        /// <summary>
        /// Gets the editing entity.
        /// </summary>
        public DataViewModelBase EditingEntity { get; private set; }

        /// <summary>
        /// Gets the original property name.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets or sets the order index.
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// Initially this is equal to the <see cref="PropertyName"/>, but it can be changed if needed.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this property is visible.
        /// This is true by default.
        /// </summary>
        public bool IsVisible { get; set; }
    }
}
