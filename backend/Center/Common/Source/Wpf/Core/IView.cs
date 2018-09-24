// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IView.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core
{
    /// <summary>
    /// Defines a view with data binding possibilities.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        object DataContext { get; set; }
    }
}
