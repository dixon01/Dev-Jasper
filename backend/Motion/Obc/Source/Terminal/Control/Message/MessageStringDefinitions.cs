// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageStringDefinitions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageStringDefinitions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Message
{
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// The message string definitions.
    /// </summary>
    internal class MessageStringDefinitions
    {
        /// <summary>
        /// Converts the message ID of a predefined message to string (multi language)
        /// </summary>
        /// <param name="message">The predefined message name</param>
        /// <returns>
        /// The localized message.
        /// </returns>
        public static string GetMessageString(evMessage.Messages message)
        {
            switch (message)
            {
                case evMessage.Messages.AssureConnection:
                    return ml.ml_string(53, "Wait connection");
                case evMessage.Messages.EnterCarWash:
                    return ml.ml_string(54, "Got to car wash");
                case evMessage.Messages.VehicleArrives:
                    return ml.ml_string(55, "Vehicle arrives");
                case evMessage.Messages.FAK:
                    return ml.ml_string(56, "Razzia");
                case evMessage.Messages.WrongMessage:
                    return ml.ml_string(57, "Wrong message sent");
                case evMessage.Messages.MessageUnderstood:
                    return ml.ml_string(58, "Message understood");
                case evMessage.Messages.PoliceArrives:
                    return ml.ml_string(59, "Police arrives");
                case evMessage.Messages.MedicArrives:
                    return ml.ml_string(60, "Ambulance arrives");
                case evMessage.Messages.WashVehicle:
                    return ml.ml_string(61, "Wash vehicle");
                case evMessage.Messages.CallControlCenter:
                    return ml.ml_string(62, "Zentrale telefonieren");
                case evMessage.Messages.SlowDown:
                    return ml.ml_string(63, "Slow down");
                case evMessage.Messages.DutyOffRequested:
                    return ml.ml_string(64, "Forced end of duty");
                case evMessage.Messages.FailureTicketing_Krauth_1:
                    return ml.ml_string(257, "Failure ticketing 1");
                case evMessage.Messages.FailureTicketing_Krauth_2:
                    return ml.ml_string(66, "Failure ticketing 2");
                case evMessage.Messages.FailureTicketing_Atron_1:
                    return ml.ml_string(65, "Failure ticketing 1");
                case evMessage.Messages.FailureTicketing_Atron_2:
                    return ml.ml_string(258, "Failure ticketing 2");
                case evMessage.Messages.FailureTicketing_Atron_3:
                    return ml.ml_string(256, "Failure ticketing 3");
                case evMessage.Messages.FailureTicketCanceler1:
                    return ml.ml_string(67, "Failure ticket canceler 1");
                case evMessage.Messages.FailureTicketCanceler2:
                    return ml.ml_string(68, "Failure ticket canceler 2");
                case evMessage.Messages.FailureTicketCanceler3:
                    return ml.ml_string(69, "Failure ticket canceler 3");
                case evMessage.Messages.WarningCashBox1:
                    return ml.ml_string(70, "Warning cash box 1");
                case evMessage.Messages.WarningCashBox2:
                    return ml.ml_string(71, "Warning cash box 2");
                case evMessage.Messages.WarningPaper1:
                    return ml.ml_string(72, "Warning paper 1");
                case evMessage.Messages.WarningPaper2:
                    return ml.ml_string(73, "Warning paper 2");
                default:
                    return message.ToString();
            }
        }
    }
}