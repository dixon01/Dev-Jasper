// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInputHandler.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IInputHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Inputs
{
    using System;

    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// The InputHandler interface.
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// Event that is fired if this handler created some data.
        /// </summary>
        event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the name of the input.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Starts this input handler.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this input handler.
        /// </summary>
        void Stop();
    }
}