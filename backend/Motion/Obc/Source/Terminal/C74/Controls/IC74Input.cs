// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IC74Input.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IC74Input type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System.Windows.Forms;

    /// <summary>
    /// Interface implemented by <see cref="Control"/> subclasses that
    /// require selection and key handling inside a <see cref="MainField"/> of C74.
    /// </summary>
    public interface IC74Input
    {
        /// <summary>
        /// Gets or sets a value indicating whether this control is selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Processes the given key.
        /// </summary>
        /// <param name="key">
        /// The key. This is never <see cref="C74Keys.None"/>.
        /// </param>
        /// <returns>
        /// True if the key was handled otherwise false.
        /// </returns>
        bool ProcessKey(C74Keys key);
    }
}