// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionRequestTrigger.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteractionRequestTrigger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System.Windows.Interactivity;

    /// <summary>
    /// Defines a trigger used for interactions.
    /// </summary>
    public class InteractionRequestTrigger : EventTrigger
    {
        /// <summary>
        /// Specifies the name of the Event this EventTriggerBase is listening for.
        /// </summary>
        /// <returns>
        /// This implementation always returns the Raised event name for ease of connection with interaction requests.
        /// </returns>
        protected override string GetEventName()
        {
            return "Raised";
        }
    }
}