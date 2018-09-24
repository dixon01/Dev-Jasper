// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CyclePackageConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The cycle package config data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using System.Linq;

    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The cycle package config data view model.
    /// </summary>
    public partial class CyclePackageConfigDataViewModel : IReusableEntity
    {
        private bool isInEditMode;

        private int referencesCount;

        private bool isExpanded;

        private bool isChildItemSelected;

        /// <summary>
        /// Gets the references count.
        /// </summary>
        public int ReferencesCount
        {
            get
            {
                return this.referencesCount;
            }

            private set
            {
                this.SetProperty(ref this.referencesCount, value, () => this.ReferencesCount);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.SetProperty(ref this.isExpanded, value, () => this.IsExpanded);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether one of the child items is selected.
        /// </summary>
        public bool IsChildItemSelected
        {
            get
            {
                return this.isChildItemSelected;
            }

            set
            {
                this.SetProperty(ref this.isChildItemSelected, value, () => this.IsChildItemSelected);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return this.isInEditMode;
            }

            set
            {
                this.SetProperty(ref this.isInEditMode, value, () => this.IsInEditMode);
            }
        }

        /// <summary>
        /// Gets the is used tool tip.
        /// </summary>
        public string IsUsedToolTip { get; private set; }

        /// <summary>
        /// The get name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetName()
        {
            return this.Name.Value;
        }

        /// <summary>
        /// The set name.
        /// </summary>
        /// <param name="cyclePackageName">
        /// The name.
        /// </param>
        public void SetName(string cyclePackageName)
        {
            this.Name.Value = cyclePackageName;
        }

        /// <summary>
        /// Validates a value before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object value)
        {
            if (propertyName == "Name")
            {
                var valuestring = value as string;

                if (string.IsNullOrEmpty(valuestring))
                {
                    return MediaStrings.CycleDetailsNavigator_CyclePackageNameMissing;
                }

                var cyclePackages =
                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.CyclePackages.Where(
                        package => package.Name.Value == valuestring);
                if (cyclePackages.Count() > 1)
                {
                    return MediaStrings.CycleDetailsNavigator_CyclePackageNameDuplicate;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The list of error messages for the given properties. Empty enumeration if no error was found.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<string> Validate(string propertyName)
        {
            if (propertyName == "Name")
            {
                if (string.IsNullOrEmpty(this.Name.Value))
                {
                    return new[] { MediaStrings.CycleDetailsNavigator_CyclePackageNameMissing };
                }

                var cyclePackages =
                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.CyclePackages.Where(
                        package => package.Name.Value == this.Name.Value);
                if (cyclePackages.Count() > 1)
                {
                    return new[] { MediaStrings.CycleDetailsNavigator_CyclePackageNameDuplicate };
                }
            }

            return Enumerable.Empty<string>();
        }
    }
}
