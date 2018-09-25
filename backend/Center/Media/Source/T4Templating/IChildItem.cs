// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChildItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IChildItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    /// <summary>
    /// Defines an item with a parent.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent.</typeparam>
    public interface IChildItem<TParent> where TParent : class
    {
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        TParent Parent { get; set; }
    }
}