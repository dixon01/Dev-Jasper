// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeBrightness.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeBrightness type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Sets the display brightness
    /// </summary>
    internal class ChangeBrightness : MainFieldCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeBrightness"/> class.
        /// </summary>
        public ChangeBrightness()
            : base(MainFieldKey.Brightness)
        {
        }
    }
}
