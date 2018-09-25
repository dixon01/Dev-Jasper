// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IndexEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The index event arguments.
    /// </summary>
    public class IndexEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexEventArgs"/> class.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public IndexEventArgs(int index)
        {
            this.Index = index;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index { get; private set; }
    }
}