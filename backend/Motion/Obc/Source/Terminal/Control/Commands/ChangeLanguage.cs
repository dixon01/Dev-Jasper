// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeLanguage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeLanguage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Select the Language
    /// </summary>
    internal class ChangeLanguage : MainFieldCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeLanguage"/> class.
        /// </summary>
        public ChangeLanguage()
            : base(MainFieldKey.Language)
        {
        }
    }
}
