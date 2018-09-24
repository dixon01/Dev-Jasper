// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDirty.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDirty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework
{
    using System.ComponentModel;

    /// <summary>
    /// Defines an object exposing its <c>dirty</c> status.
    /// An object is <c>dirty</c> when any of its properties changed in the current <c>session</c>, typically between
    /// two saves.
    /// </summary>
    public interface IDirty : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating whether this instance has changes, making it <c>dirty</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        bool IsDirty { get; }

        /// <summary>
        /// Sets the <see cref="IsDirty"/> flag. The default behavior only sets the flag on the current object.
        /// </summary>
        void MakeDirty();

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag. The default behavior clears the flag on the current object and all
        /// its children.
        /// </summary>
        void ClearDirty();
    }
}