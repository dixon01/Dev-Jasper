// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InformationMessages.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Shows the message list
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Shows the message list
    /// </summary>
    internal class InformationMessages : MainFieldCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformationMessages"/> class.
        /// </summary>
        public InformationMessages()
            : base(MainFieldKey.InMessages)
        {
        }
    }
}
