// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataItemValueProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDataItemValueProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;

    /// <summary>
    /// Interface to be implemented by classes that want to provide
    /// values for IsiPut responses to the remote board computer.
    /// </summary>
    public interface IDataItemValueProvider
    {
        /// <summary>
        /// This event is fired every time the <see cref="Value"/> changes.
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// Gets the value from this provider.
        /// </summary>
        string Value { get; }
    }
}