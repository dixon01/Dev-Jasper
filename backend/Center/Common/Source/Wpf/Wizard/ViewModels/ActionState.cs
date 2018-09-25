// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard.ViewModels
{
    using System;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the state of one action while deleting a tenant
    /// </summary>
    public class ActionState : ViewModelBase
    {
        private string description;

        private States state;

        /// <summary>
        /// Gets or sets the description of the action.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }

        /// <summary>
        /// Gets or sets the state of the action.
        /// </summary>
        public States State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.SetProperty(ref this.state, value, () => this.State);
            }
        }

        /// <summary>
        /// Gets the image url depending on the <see cref="ActionState.State"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the state is not "Success", "Failed", or "In Progress...".
        /// </exception>
        public string StatusImageUrl
        {
            get
            {
                switch (this.state)
                {
                    case States.None:
                        break;
                    case States.Waiting:
                        break;
                    case States.InProgress:
                        return "/Icons/loading.gif";
                    case States.Done:
                        return "/Icons/success.png";
                    case States.Error:
                        return "/Icons/failed.png";
                    case States.Canceled:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return string.Empty;
            }
        }
    }
}