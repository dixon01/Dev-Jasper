// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemovableMediaViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemovableMediaViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Navigator
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The view model representing a removable media (USB stick).
    /// </summary>
    public class RemovableMediaViewModel : ViewModelBase
    {
        private bool hasFeedback;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovableMediaViewModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the media.
        /// </param>
        public RemovableMediaViewModel(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the media.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the media contains has feedback.
        /// </summary>
        public bool HasFeedback
        {
            get
            {
                return this.hasFeedback;
            }

            set
            {
                this.SetProperty(ref this.hasFeedback, value, () => this.HasFeedback);
            }
        }
    }
}