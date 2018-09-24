// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleConfigDataViewModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The cycle config data view model base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Presentation.Cycle;
    using Gorba.Center.Media.Core.Resources;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The cycle config data view model base.
    /// </summary>
    public partial class CycleConfigDataViewModelBase
    {
        private ExtendedObservableCollection<CyclePackageConfigDataViewModel> cyclePackageReferences;

        /// <summary>
        /// Gets the cycle package references.
        /// </summary>
        public ExtendedObservableCollection<CyclePackageConfigDataViewModel> CyclePackageReferences
        {
            get
            {
                return this.cyclePackageReferences;
            }
        }

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsValid()
        {
            return base.IsValid() && this.IsNameUnique();
        }

        /// <summary>
        /// The validate as reference.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<string> ValidateAsReference(string propertyName)
        {
            return this.Validate(propertyName);
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        protected override IEnumerable<string> Validate(string propertyName)
        {
            if (propertyName == "Name")
            {
                if (string.IsNullOrEmpty(this.Name.Value))
                {
                    return new[] { MediaStrings.LayoutNavigationDialog_CycleNameMissing };
                }

                if (!this.IsNameUnique())
                {
                    return new[] { MediaStrings.LayoutNavigationDialog_CycleNameDuplicate };
                }
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Checks if a compatibility conversion for CSV mapping is required
        /// </summary>
        /// <param name="exportParameters">
        /// The export parameters.
        /// </param>
        /// <returns>
        /// True if is required.
        /// </returns>
        protected bool CsvMappingCompatibilityRequired(object exportParameters)
        {
            var parameters = exportParameters as ExportCompatibilityParameters;
            return parameters != null && parameters.CsvMappingCompatibilityRequired;
        }

        // ReSharper disable once UnusedParameter.Local
        partial void Initialize(CycleConfigDataViewModelBase dataViewModel)
        {
            this.cyclePackageReferences = new ExtendedObservableCollection<CyclePackageConfigDataViewModel>();
            this.cyclePackageReferences.CollectionChanged += this.CyclePackageReferencesChanged;
            this.PropertyChanged += this.OnPropertyChanged;
        }

        // ReSharper disable once UnusedParameter.Local
        partial void Initialize(CycleConfigDataModelBase dataModel)
        {
            this.cyclePackageReferences = new ExtendedObservableCollection<CyclePackageConfigDataViewModel>();
            this.cyclePackageReferences.CollectionChanged += this.CyclePackageReferencesChanged;
            this.PropertyChanged += this.OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Enabled")
            {
                if (this.mediaShell.MediaApplicationState.CurrentProject != null)
                {
                    foreach (var resource in this.mediaShell.MediaApplicationState.CurrentProject.Resources)
                    {
                        resource.UpdateIsUsedVisible();
                    }
                }
            }
        }

        private bool IsNameUnique()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return false;
            }

            var nameCount =
                applicationState.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Count(
                    cycle => cycle.Name.Value == this.Name.Value)
                + applicationState.CurrentProject.InfomediaConfig.Cycles.EventCycles.Count(
                    eventCycle => eventCycle.Name.Value == this.Name.Value);

            return nameCount <= 1;
        }

        private void CyclePackageReferencesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.CyclePackageReferences);
        }
    }
}
