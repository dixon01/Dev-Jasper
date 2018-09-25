// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexDividerDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegexDividerDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using System;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="RegexDivider"/>.
    /// </summary>
    public class RegexDividerDataViewModel : TransformationDataViewModelBase<RegexDivider>
    {
        private string regex;

        private RegexOptions options;

        /// <summary>
        /// Gets or sets the regex.
        /// </summary>
        public string Regex
        {
            get
            {
                return this.regex;
            }

            set
            {
                if (!this.SetProperty(ref this.regex, value, () => this.Regex))
                {
                    return;
                }

                try
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new Regex(value, this.Options);
                    this.ClearErrors("Regex");
                }
                catch (ArgumentException ex)
                {
                    this.AddError("Regex", ex.Message);
                }
            }
        }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public RegexOptions Options
        {
            get
            {
                return this.options;
            }

            set
            {
                this.SetProperty(ref this.options, value, () => this.Options);
            }
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(RegexDivider config)
        {
            this.Regex = config.Regex;
            this.Options = config.Options;
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(RegexDivider config)
        {
            config.Regex = this.Regex;
            config.Options = this.Options;
        }
    }
}