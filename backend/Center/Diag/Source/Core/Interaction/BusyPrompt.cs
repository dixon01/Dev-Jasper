// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusyPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The BusyPrompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Interaction
{
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// The BusyPrompt.
    /// </summary>
    public class BusyPrompt : PromptNotification
    {
        private bool isBusy;

        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.SetProperty(ref this.isBusy, value, () => this.IsBusy);
            }
        }
    }
}