// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReusableEntity.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IReusableEntity.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    /// <summary>
    /// The Interface for reusable entities
    /// </summary>
    public interface IReusableEntity
    {
        /// <summary>
        /// Gets the number of sections where this layout is used.
        /// </summary>
        int ReferencesCount { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is in edit mode.
        /// </summary>
        bool IsInEditMode { get; set; }

        /// <summary>
        /// Gets the tool tip shown for the IsUsed flag.
        /// </summary>
        string IsUsedToolTip { get; }

        /// <summary>
        /// Gets or sets the display text
        /// </summary>
        string DisplayText { get; set; }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValid();

        /// <summary>
        /// Gets the name
        /// </summary>
        /// <returns>the name</returns>
        string GetName();

        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        void SetName(string name);
    }
}