// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="Join"/>.
    /// </summary>
    public class JoinDataViewModel : TransformationDataViewModelBase<Join>
    {
        private string separator;

        /// <summary>
        /// Gets or sets the separator.
        /// </summary>
        public string Separator
        {
            get
            {
                return this.separator;
            }

            set
            {
                this.SetProperty(ref this.separator, value, () => this.Separator);
            }
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(Join config)
        {
            this.Separator = config.Separator;
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(Join config)
        {
            config.Separator = this.Separator;
        }
    }
}