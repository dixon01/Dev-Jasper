// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INumberInputBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the INumberInputBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The base interface for number input.
    /// </summary>
    public interface INumberInputBase : IMainField
    {
        /// <summary>
        /// The input done event.
        /// </summary>
        event EventHandler<NumberInputEventArgs> InputDone;

        /// <summary>
        /// The input update event.
        /// The index value will be the value of this input.
        /// </summary>
        event EventHandler<IndexEventArgs> InputUpdate;

        /// <summary>
        /// Gets or sets the hint text.
        /// </summary>
        string HintText { get; set; }
    }
}