// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeechIra.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Iqube Radio (only for VB Biel)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Iqube Radio (only for VB Biel)
    /// </summary>
    internal class SpeechIra : Command
    {
        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="sourceIsMenu">
        /// A flag indicating if the source is the menu.
        /// </param>
        public override void Execute(IContext context, bool sourceIsMenu)
        {
            if (sourceIsMenu || !context.IraHandler.SendDisconnect())
            {
                // user comes from menu or we are not connected. enter in the IRA screen
                context.ShowMainField(MainFieldKey.IqubeRadio);
            }
        }
    }
}
