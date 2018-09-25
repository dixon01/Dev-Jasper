// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageOptionGroupViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The group view model for language options
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.Options
{
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Model.Options;

    /// <summary>
    /// The group view model for language options
    /// </summary>
    public class LanguageOptionGroupViewModel : OptionGroupViewModelBase
    {
        private CultureInfo selectedLanguage;

        private Collection<CultureInfo> languages;

        private string restartInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageOptionGroupViewModel"/> class.
        /// </summary>
        public LanguageOptionGroupViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageOptionGroupViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public LanguageOptionGroupViewModel(LanguageOptionGroup model)
        {
            this.languages = new Collection<CultureInfo>
                                 {
                                     new CultureInfo("en-US"),
                                     new CultureInfo("de-DE"),
                                     new CultureInfo("fr-FR")
                                 };

            if (model == null)
            {
                this.SelectedLanguage = this.languages.First();
                return;
            }

            this.SelectedLanguage = new CultureInfo(model.Language);
        }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        public CultureInfo SelectedLanguage
        {
            get
            {
                return this.selectedLanguage;
            }

            set
            {
                this.SetProperty(ref this.selectedLanguage, value, () => this.SelectedLanguage);
            }
        }

        /// <summary>
        /// Gets or sets the selected language string. This value is used for serialization.
        /// </summary>
        public string SelectedLanguageString
        {
            get
            {
                return this.selectedLanguage.Name;
            }

            set
            {
                var newValue = CultureInfo.GetCultureInfo(value);
                this.SetProperty(ref this.selectedLanguage, newValue, () => this.SelectedLanguageString);
            }
        }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        public Collection<CultureInfo> Languages
        {
            get
            {
                return this.languages;
            }

            set
            {
                this.SetProperty(ref this.languages, value, () => this.Languages);
            }
        }

        /// <summary>
        /// Gets or sets the restart information.
        /// </summary>
        public string RestartInformation
        {
            get
            {
                return this.restartInformation;
            }

            set
            {
                this.SetProperty(ref this.restartInformation, value, () => this.RestartInformation);
            }
        }

        /// <summary>
        /// Creates a model from this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="LanguageOptionGroup"/> model.
        /// </returns>
        public override OptionGroupBase CreateModel()
        {
            return new LanguageOptionGroup
                       {
                           Language = this.selectedLanguage.Name
                       };
        }
    }
}
