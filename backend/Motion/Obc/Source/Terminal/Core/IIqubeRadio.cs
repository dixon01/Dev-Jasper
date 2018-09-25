// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIqubeRadio.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IIqubeRadio type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The iqube radio field interface.
    /// </summary>
    public interface IIqubeRadio : IMainField
    {
        /// <summary>
        ///   Confirmed Event
        /// </summary>
        event EventHandler<IqubeRadioEventArgs> InputDone;

        /// <summary>
        ///   Initialization from the iqube.radio screen
        /// </summary>
        /// <param name = "mainCaption">
        /// The caption.
        /// </param>
        void Init(string mainCaption);
    }
}