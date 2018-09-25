// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryAttribute.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CategoryAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace System.ComponentModel
{
    /// <summary>
    /// Property category attribute which is not available in Compact Framework.
    /// </summary>
    public partial class CategoryAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryAttribute"/> class.
        /// </summary>
        public CategoryAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryAttribute"/> class.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public CategoryAttribute(string category)
        {
            this.Category = category;
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public string Category { get; set; }
    }
}

// ReSharper restore CheckNamespace
