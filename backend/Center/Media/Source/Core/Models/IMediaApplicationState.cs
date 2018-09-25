// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediaApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMediaApplicationState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Controllers.ProjectStates;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Defines the state of the Media application.
    /// </summary>
    public interface IMediaApplicationState : IConnectedApplicationState
    {
        /// <summary>
        /// Gets the default master layout.
        /// </summary>
        MasterLayout DefaultMasterLayout { get; }

        /// <summary>
        /// Gets or sets the current project state.
        /// </summary>
        ProjectStates CurrentProjectState { get; set; }

        /// <summary>
        /// Gets or sets the current project.
        /// </summary>
        /// <value>
        /// The current project.
        /// </value>
        MediaProjectDataViewModel CurrentProject { get; set; }

        /// <summary>
        /// Gets or sets the current project.
        /// </summary>
        /// <value>
        /// The current project.
        /// </value>
        IProjectManager ProjectManager { get; set; }

        /// <summary>
        /// Gets or sets the list of recently used projects.
        /// </summary>
        /// <value>
        /// The list of recent projects.
        /// </value>
        ExtendedObservableCollection<RecentProjectDataViewModel> RecentProjects { get; set; }

        /// <summary>
        /// Gets or sets the existing projects of the current tenant.
        /// </summary>
        ObservableCollection<MediaConfigurationDataViewModel> ExistingProjects { get; set; }

        /// <summary>
        /// Gets all existing projects of all authorized tenants.
        /// </summary>
        Dictionary<int, ObservableCollection<MediaConfigurationDataViewModel>> AllExistingProjects { get; }

        /// <summary>
        /// Gets or sets a value indicating whether is existing projects loaded.
        /// </summary>
        bool IsExistingProjectsLoaded { get; set; }

        /// <summary>
        /// Gets or sets the list of dictionary values.
        /// </summary>
        ExtendedObservableCollection<DictionaryValueDataViewModel> RecentDictionaryValues { get; set; }

        /// <summary>
        /// Gets or sets the currently selected Physical Screen
        /// </summary>
        PhysicalScreenConfigDataViewModel CurrentPhysicalScreen { get; set; }

        /// <summary>
        /// Gets or sets the currently selected virtual display
        /// </summary>
        VirtualDisplayConfigDataViewModel CurrentVirtualDisplay { get; set; }

        /// <summary>
        /// Gets or sets the current virtual display references.
        /// </summary>
        ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel> CurrentVirtualDisplayReferences { get; set; }

        /// <summary>
        /// Gets or sets the currently selected cycle package
        /// </summary>
        CyclePackageConfigDataViewModel CurrentCyclePackage { get; set; }

        /// <summary>
        /// Gets or sets the currently selected cycle
        /// </summary>
        CycleConfigDataViewModelBase CurrentCycle { get; set; }

        /// <summary>
        /// Gets or sets the currently selected cycle item
        /// </summary>
        SectionConfigDataViewModelBase CurrentSection { get; set; }

        /// <summary>
        /// Gets or sets the currently selected Layout
        /// </summary>
        LayoutConfigDataViewModelBase CurrentLayout { get; set; }

        /// <summary>
        /// Gets or sets the current media configuration.
        /// </summary>
        MediaConfigurationDataViewModel CurrentMediaConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the recent media resources.
        /// </summary>
        /// <value>
        /// The recent media resources.
        /// </value>
        [DataMember(Name = "RecentMediaResources")]
        Dictionary<Guid, ExtendedObservableCollection<ResourceInfoDataViewModel>> RecentMediaResources { get; set; }

        /// <summary>
        /// Gets or sets the last used file dialog directories for projects, images and videos.
        /// </summary>
        [DataMember(Name = "LastUsedDirectories")]
        Dictionary<DialogDirectoryType, string> LastUsedDirectories { get; set; }

        /// <summary>
        /// Gets or sets the consistency messages.
        /// </summary>
        ExtendedObservableCollection<ConsistencyMessageDataViewModel> ConsistencyMessages { get; set; }

        /// <summary>
        /// Gets or sets the compatibility messages.
        /// </summary>
        ExtendedObservableCollection<ConsistencyMessageDataViewModel> CompatibilityMessages { get; set; }

        /// <summary>
        /// Gets the system fonts.
        /// </summary>
        Dictionary<string, List<string>> SystemFonts { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the edge snap mode is enabled
        /// </summary>
        bool UseEdgeSnap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is currently checking in a project as new one.
        /// </summary>
        bool IsCheckinAs { get; set; }

        /// <summary>
        /// Initializes the state.
        /// </summary>
        /// <param name="shell">The shell.</param>
        void Initialize(IMediaShell shell);
    }
}