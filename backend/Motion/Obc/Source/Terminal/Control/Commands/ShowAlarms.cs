// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowAlarms.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShowAlarms type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using System;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    /// Shows the alarms list.
    /// </summary>
    internal class ShowAlarms : Command
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
            if (context.AlarmHandler == null)
            {
                throw new NotSupportedException(ml.ml_string(129, "Alarm is not activated"));
            }

            if (context.AlarmHandler.IsAlarmActive() == false)
            {
                context.ShowMainField(MainFieldKey.Alarm);
            }
        }
    }
}
