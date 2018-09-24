// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISimulator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISimulator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation
{
    using System;

    /// <summary>
    /// Interface to be implemented by every simulator that can show the contents
    /// of an icenter.media layout in a separate window.
    /// </summary>
    public interface ISimulator
    {
        /// <summary>
        /// Event that is fired when this simulator is stopped, either by calling
        /// <see cref="Stop"/> or if the simulator stops by itself (e.g. trough user interaction).
        /// </summary>
        event EventHandler Stopped;

        /// <summary>
        /// Gets the width of the area that is simulated.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the area that is simulated.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Configures this simulator with the given size.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        void Configure(int width, int height);

        /// <summary>
        /// Starts this simulator by showing the UI.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this simulator by hiding the UI.
        /// </summary>
        void Stop();
    }
}