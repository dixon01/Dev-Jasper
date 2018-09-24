// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Detour.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Detour type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;

    /// <summary>
    ///   Toggles Redirection/detour.
    /// </summary>
    internal class Detour : Command
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
            context.StatusHandler.DriveInfo.DetourActive = !context.StatusHandler.DriveInfo.DetourActive;
            if (sourceIsMenu)
            {
                context.ShowRootScreen();
            }
        }
    }
}
