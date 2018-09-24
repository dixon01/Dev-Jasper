// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Razzia.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Toggle Razzia
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;

    /// <summary>
    ///   Toggle Razzia
    /// </summary>
    internal class Razzia : Command
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
            context.StatusHandler.DriveInfo.RazziaEnabled = !context.StatusHandler.DriveInfo.RazziaEnabled;
            if (sourceIsMenu)
            {
                context.ShowRootScreen();
            }
        }
    }
}