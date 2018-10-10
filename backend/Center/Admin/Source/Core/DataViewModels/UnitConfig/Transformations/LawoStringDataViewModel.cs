// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LawoStringDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LawoStringDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="LawoString"/>.
    /// </summary>
    public class LawoStringDataViewModel : TransformationDataViewModelBase<LawoString>
    {
        private int codePage;

        /// <summary>
        /// Gets or sets the code page.
        /// </summary>
        public int CodePage
        {
            get
            {
                return this.codePage;
            }

            set
            {
                this.SetProperty(ref this.codePage, value, () => this.CodePage);
            }
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(LawoString config)
        {
            this.CodePage = config.CodePage;
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(LawoString config)
        {
            config.CodePage = this.CodePage;
        }
    }
}