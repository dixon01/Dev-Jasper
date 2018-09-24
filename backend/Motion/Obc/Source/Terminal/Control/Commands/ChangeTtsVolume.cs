// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTtsVolume.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeTtsVolume type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Sets the Volume (TTS)
    /// </summary>
    internal class ChangeTtsVolume : MainFieldCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTtsVolume"/> class.
        /// </summary>
        public ChangeTtsVolume()
            : base(MainFieldKey.TtsVolume)
        {
        }
    }
}
