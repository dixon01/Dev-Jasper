// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTab.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitTab type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// A tab showing information about a unit.
    /// </summary>
    public class UnitTab : UnitTabBase
    {
        private readonly ICommandRegistry commandRegistry;

        private InfoSectionViewModelBase selectedInfoSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTab"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        /// <param name="unit">
        /// The unit view model.
        /// </param>
        /// <param name="commandRegistry">the command registry</param>
        public UnitTab(IDiagShell shell, UnitViewModelBase unit, ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.Unit = unit;
            this.Shell = shell;

            this.Unit.PropertyChanged += this.UnitOnPropertyChanged;

            this.Name = this.Unit.DisplayName;

            this.InfoSections = new ObservableCollection<InfoSectionViewModelBase>
                                {
                                    new SystemInfoSectionViewModel(unit),
                                    new FileSystemInfoSectionViewModel(unit, commandRegistry),
                                    new RemoteViewerSectionViewModel(unit),
                                    new UnitIoInfoSectionViewModel(unit)
                                };
        }

        /// <summary>
        /// Gets the shell
        /// </summary>
        public IDiagShell Shell { get; private set; }

        /// <summary>
        /// Gets the info sections to be shown for this unit.
        /// </summary>
        public ObservableCollection<InfoSectionViewModelBase> InfoSections { get; private set; }

        /// <summary>
        /// Gets or sets the selected info section.
        /// </summary>
        public InfoSectionViewModelBase SelectedInfoSection
        {
            get
            {
                return this.selectedInfoSection;
            }

            set
            {
                if (!this.SetProperty(ref this.selectedInfoSection, value, () => this.SelectedInfoSection))
                {
                    return;
                }

                var appSection = value as ApplicationInfoSectionViewModel;
                this.SelectedApplication = appSection == null ? null : appSection.Application;
            }
        }

        /// <summary>
        /// Gets the unit view model.
        /// </summary>
        public UnitViewModelBase Unit { get; private set; }

        /// <summary>
        /// Gets the launch application command.
        /// </summary>
        public ICommand LaunchApplicationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitApplication.Launch);
            }
        }

        /// <summary>
        /// Gets the re-launch application command.
        /// </summary>
        public ICommand RelaunchApplicationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitApplication.Relaunch);
            }
        }

        /// <summary>
        /// Gets the end application command.
        /// </summary>
        public ICommand EndApplicationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitApplication.End);
            }
        }

        private void UnitOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DisplayName")
            {
                this.Name = this.Unit.DisplayName;
            }
        }
    }
}