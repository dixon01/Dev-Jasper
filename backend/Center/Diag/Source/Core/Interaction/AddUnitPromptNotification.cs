// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddUnitPromptNotification.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AddUnitPromptNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Interaction
{
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// The prompt notification to add a new unit.
    /// </summary>
    public class AddUnitPromptNotification : PromptNotification
    {
        private string unitAddress;

        /// <summary>
        /// Gets or sets the unit address (IP address for now).
        /// </summary>
        public string UnitAddress
        {
            get
            {
                return this.unitAddress;
            }

            set
            {
                this.SetProperty(ref this.unitAddress, value, () => this.UnitAddress);
            }
        }
    }
}