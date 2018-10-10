// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LayoutConfigDataViewModel
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using System.Collections.Specialized;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The LayoutConfigDataViewModel.
    /// </summary>
    public partial class LayoutConfigDataViewModel : IReusableEntity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool isInEditMode;

        private int referencesCount;

        private string isUsedToolTip;

        private ExtendedObservableCollection<LayoutCycleSectionRefDataViewModel> cycleSectionReferences;

        /// <summary>
        /// Gets or sets a value indicating whether the layout is in edit mode.
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
        /// Gets the tool tip shown for the IsUsed flag.
        /// </summary>
        public string IsUsedToolTip
        {
            get
            {
                return this.isUsedToolTip;
            }

            private set
            {
                this.SetProperty(ref this.isUsedToolTip, value, () => this.IsUsedToolTip);
            }
        }

        /// <summary>
        /// Gets the resolutions index by width and height
        /// </summary>
        public LayoutResolutionCollection IndexedResolutions
        {
            get
            {
                return new LayoutResolutionCollection(this.Resolutions);
            }
        }

        /// <summary>
        /// Gets the number of sections where this layout is used.
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
                this.IsUsedToolTip = string.Format(
                    MediaStrings.LayoutNavigationDialog_UsedLayoutTooltip, this.referencesCount);
            }
        }

        /// <summary>
        /// Gets the references of cycles and sections where this layout is used.
        /// </summary>
        public ExtendedObservableCollection<LayoutCycleSectionRefDataViewModel> CycleSectionReferences
        {
            get
            {
                return this.cycleSectionReferences;
            }
        }

        /// <summary>
        /// Sets the name of the Layout.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void SetName(string name)
        {
            this.Name.Value = name;
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        /// <returns>the name</returns>
        public string GetName()
        {
            return this.Name.Value;
        }

        /// <summary>
        /// The get cycle section reference.
        /// </summary>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <param name="parentCycle">
        /// The parent cycle.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutCycleSectionRefDataViewModel"/>.
        /// </returns>
        public LayoutCycleSectionRefDataViewModel GetCycleSectionReference(
            SectionConfigDataViewModelBase section,
            CycleConfigDataViewModelBase parentCycle)
        {
            return this.CycleSectionReferences.FirstOrDefault(
                r => r.CycleReference == parentCycle && r.SectionReference == section);
        }

        /// <summary>
        /// Adds the resolution of the current virtual display if it doesn't exist.
        /// </summary>
        /// <returns>
        /// The <see cref="ResolutionConfigDataViewModel"/> if it has been added, NULL otherwise.
        /// </returns>
        public ResolutionConfigDataViewModel AddCurrentResolution()
        {
            var virtualDisplay = this.mediaShell.MediaApplicationState.CurrentVirtualDisplay;
            var existingResolution = this.IndexedResolutions[virtualDisplay.Width.Value, virtualDisplay.Height.Value];
            if (existingResolution == null)
            {
                var resolution = new ResolutionConfigDataViewModel(this.mediaShell)
                {
                    Width = { Value = virtualDisplay.Width.Value },
                    Height = { Value = virtualDisplay.Height.Value }
                };

                this.Resolutions.Add(resolution);
                return resolution;
            }

            return null;
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
                    return new[] { MediaStrings.LayoutNavigationDialog_LayoutNameMissing };
                }

                var layouts =
                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.Where(
                        layout => layout.Name.Value == this.Name.Value);
                if (layouts.Count() > 1)
                {
                    return new[] { MediaStrings.LayoutNavigationDialog_LayoutNameDuplicate };
                }
            }

            return Enumerable.Empty<string>();
        }

        partial void Initialize(Models.Presentation.LayoutConfigDataModel dataModel)
        {
            this.cycleSectionReferences = new ExtendedObservableCollection<LayoutCycleSectionRefDataViewModel>();
            this.cycleSectionReferences.CollectionChanged += this.CycleSectionReferencesChanged;

            this.DisplayText = this.Name.Value;
        }

        partial void Initialize(LayoutConfigDataViewModel dataViewModel)
        {
            this.cycleSectionReferences = new ExtendedObservableCollection<LayoutCycleSectionRefDataViewModel>();
            this.cycleSectionReferences.CollectionChanged += this.CycleSectionReferencesChanged;
        }

        private void CycleSectionReferencesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ReferencesCount = this.cycleSectionReferences.Count;
        }

        private LayoutConfigDataViewModel FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            foreach (var layoutConfigDataViewModel in applicationState.CurrentProject.InfomediaConfig.Layouts)
            {
                if (layoutConfigDataViewModel.Name.Value == this.baselayoutnameReferenceName)
                {
                    return layoutConfigDataViewModel;
                }
            }

            Logger.Trace("Layout reference with name {0} not found in Layouts.", this.BaseLayoutNameName);
            return null;
        }
    }
}