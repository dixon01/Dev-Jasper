// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeechGsm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpeechGsm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using System;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Show the Speech screen (GSM/Radio)
    /// </summary>
    internal class SpeechGsm : Command
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
            if (context.ConfigHandler.GetConfig().SpeechType.Value != SpeechType.Gsm)
            {
                throw new NotSupportedException(ml.ml_string(127, "GSM is not activated"));
            }

            if (context.WanManager == null)
            {
                throw new NotSupportedException(
                    ml.ml_string(84, "There's a configuration fault. Can not handle GSM speech functionality."));
            }

            if (sourceIsMenu)
            {
                // user comes from menu. enter in the speech gsm screen
                context.ShowMainField(MainFieldKey.SpeechGsm);
            }
            else if (context.WanManager.IsSpeechConnected())
            {
                // In a call
                context.WanManager.HangUp();
            }
            else if (context.WanManager.IsBuildingCall())
            {
                // building a call (in or out call)
                context.WanManager.AnswerCall();
            }
            else
            {
                context.ShowMainField(MainFieldKey.SpeechGsm);
            }
        }
    }
}