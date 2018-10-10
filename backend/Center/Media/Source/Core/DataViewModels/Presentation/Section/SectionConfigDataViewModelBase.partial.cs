// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionConfigDataViewModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Section
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The SectionConfigDataViewModelBase.
    /// </summary>
    public partial class SectionConfigDataViewModelBase : IReusableEntity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool isInEditMode;

        private string name;

        private bool isExpanded;

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
        /// Gets or sets a value indicating whether the is in edit mode
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
        /// Gets or sets the name of the Section
        /// </summary>
        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(this.name))
                {
                    return this.name;
                }

                return MediaStrings.ResourceManager.GetString("SectionConfigNameFallback_" + this.GetType().Name);
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets the references count
        /// </summary>
        public int ReferencesCount { get; private set; }

        /// <summary>
        /// Gets the is used tooltip
        /// </summary>
        public string IsUsedToolTip { get; private set; }

        /// <summary>
        /// Gets the name
        /// </summary>
        /// <returns>the name</returns>
        public string GetName()
        {
            return this.Name;
        }

        /// <summary>
        /// Sets the name of the section.
        /// </summary>
        /// <param name="sectionName">
        /// The name.
        /// </param>
        public void SetName(string sectionName)
        {
            this.Name = sectionName;
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

        partial void Initialize(SectionConfigDataViewModelBase dataViewModel)
        {
            if (dataViewModel != null)
            {
                this.Name = dataViewModel.Name;
            }

            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void Initialize(Models.Presentation.Section.SectionConfigDataModelBase dataModel)
        {
            if (dataModel != null && !string.IsNullOrEmpty(dataModel.Name))
            {
                this.Name = dataModel.Name;
            }

            this.PropertyChanged += this.OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Enabled" || e.PropertyName == "Layout")
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

        partial void ConvertNotGeneratedToDataModel(
            ref Models.Presentation.Section.SectionConfigDataModelBase dataModel)
        {
            if (dataModel != null)
            {
                dataModel.Name = this.name;
            }
        }

        private LayoutConfigDataViewModelBase FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            foreach (var layout in applicationState.CurrentProject.InfomediaConfig.Layouts)
            {
                if (layout.Name.Value == this.LayoutName)
                {
                    return layout;
                }
            }

            Logger.Trace("Layout reference with name {0} not found in Layouts.", this.LayoutName);
            return null;
        }

        private CycleConfigDataViewModelBase SearchParentCycle()
        {
            if (this.mediaShell == null
                || this.mediaShell.MediaApplicationState == null
                || this.mediaShell.MediaApplicationState.CurrentProject == null
                || this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            var cycles = this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Cycles;
            if (cycles != null)
            {
                foreach (var cycle in cycles.StandardCycles)
                {
                    if (cycle.Sections.Any(containedSection => object.ReferenceEquals(containedSection, this)))
                    {
                        return cycle;
                    }
                }

                foreach (var cycle in cycles.EventCycles)
                {
                    if (cycle.Sections.Any(containedSection => object.ReferenceEquals(containedSection, this)))
                    {
                        return cycle;
                    }
                }
            }

            return null;
        }

        private void PostActionSetLayout_DoNotUseException()
        {
            throw new NotSupportedException("Default setter is not to be used. Use SetLayout() instead.");
        }
    }
}
