// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation of <see cref="ILayoutController"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System.Linq;
    using System.Text;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="ILayoutController"/>.
    /// </summary>
    public class LayoutController : ILayoutController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutController"/> class.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public LayoutController(
            IMediaShellController parentController,
            IMediaShell shell,
            ICommandRegistry commandRegistry)
        {
            this.ParentController = parentController;
            this.commandRegistry = commandRegistry;
            this.MediaShell = shell;
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.CreateTftLayout,
                new RelayCommand(this.CreateTftLayout));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.CreateLedLayout,
                new RelayCommand(this.CreateLedLayout));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.CreateAudioLayout,
                new RelayCommand(this.CreateAudioLayout));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.DeleteLayout,
                new RelayCommand<LayoutConfigDataViewModel>(this.DeleteLayout));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.CloneLayout,
                new RelayCommand<LayoutConfigDataViewModel>(this.CloneLayout));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.Rename,
                new RelayCommand<RenameReusableEntityParameters>(this.RenameLayout));
        }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        public IMediaShellController ParentController { get; set; }

        /// <summary>
        /// Gets the media shell.
        /// </summary>
        public IMediaShell MediaShell { get; private set; }

        private static void ShowLayoutCycleSectionsMessageBox(LayoutConfigDataViewModel layout)
        {
            var references =
                layout.CycleSectionReferences.OrderBy(csr => csr.CycleReference.Name.Value).Take(10).ToList();
            var sb = new StringBuilder();
            sb.AppendFormat(MediaStrings.LayoutNavigationDialog_RemoveLayoutMessage, layout.Name.Value);
            sb.AppendLine();
            sb.AppendLine();
            var cycleName = string.Empty;
            var cycleFormat = MediaStrings.LayoutNavigationDialog_RemoveLayoutMessage_Cycle;
            var sectionFormat = MediaStrings.LayoutNavigationDialog_RemoveLayoutMessage_Section;
            foreach (var reference in references)
            {
                var sectionName =
                    reference.SectionReference.GetType().Name.Replace("ConfigDataViewModel", string.Empty);
                if (reference.CycleReference.Name.Value != cycleName)
                {
                    cycleName = reference.CycleReference.Name.Value;
                    sb.AppendFormat(cycleFormat, cycleName);
                    sb.AppendLine();
                }

                sb.AppendFormat(sectionFormat, sectionName);
                sb.AppendLine();
            }

            Logger.Warn("Tried to delete layout '{0}' which is used.", layout.Name.Value);
            MessageBox.Show(
                sb.ToString(),
                MediaStrings.LayoutNavigationDialog_RemoveLayoutTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void RenameLayout(RenameReusableEntityParameters parameters)
        {
            var layout = parameters.Entity as LayoutConfigDataViewModel;
            if (layout != null && parameters.NewName != layout.Name.Value)
            {
                Logger.Debug("Rename layout from '{0}' to '{1}'", layout.Name.Value, parameters.NewName);
                var historyEntry = new RenameLayoutHistoryEntry(
                    layout,
                    parameters.NewName,
                    MediaStrings.MediaShellController_RenameLayoutHistoryEntry);
                this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
        }

        private void CreateTftLayout()
        {
            Logger.Info("Create new TFT layout");
            var newLayout = this.CreateBasicLayout(PhysicalScreenType.TFT);

            var historyEntry = new CreateLayoutHistoryEntry(
                newLayout,
                this.MediaShell.MediaApplicationState,
                this.commandRegistry,
                MediaStrings.MediaShellController_CreateLayoutHistoryEntry);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void CreateLedLayout()
        {
            Logger.Info("Create new LED layout.");
            var newLayout = this.CreateBasicLayout(PhysicalScreenType.LED);

            var historyEntry = new CreateLayoutHistoryEntry(
                newLayout,
                this.MediaShell.MediaApplicationState,
                this.commandRegistry,
                MediaStrings.MediaShellController_CreateLayoutHistoryEntry);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void CreateAudioLayout()
        {
            Logger.Info("Create new audio layout.");
            var newLayout = this.CreateBasicLayout(PhysicalScreenType.Audio);
            newLayout.Resolutions.First().Elements.Add(new AudioOutputElementDataViewModel(this.MediaShell)
                {
                    Volume = { Value = 80 },
                    ElementName = { Value = MediaStrings.ElementName_AudioOutput }
                });

            var historyEntry = new CreateLayoutHistoryEntry(
                newLayout,
                this.MediaShell.MediaApplicationState,
                this.commandRegistry,
                MediaStrings.MediaShellController_CreateLayoutHistoryEntry);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private LayoutConfigDataViewModel CreateBasicLayout(PhysicalScreenType layoutNamePrefix)
        {
            const int Index = 0;
            var resolutionWidth = this.MediaShell.MediaApplicationState.CurrentVirtualDisplay.Width.Value;
            var resolutionHeight = this.MediaShell.MediaApplicationState.CurrentVirtualDisplay.Height.Value;

            var newName = this.ParentController.PhysicalScreenController.GenerateLayoutName(
                resolutionWidth,
                resolutionHeight,
                Index,
                layoutNamePrefix);
            var newLayout = new LayoutConfigDataViewModel(this.MediaShell)
            {
                Name = new DataValue<string>(newName),
                DisplayText = newName,
                IsInEditMode = true
            };

            var resolutionConfig = this.ParentController.ProjectController.CreateDefaultResolutionConfigDataViewModel(
                resolutionWidth, resolutionHeight);

            newLayout.Resolutions = new ExtendedObservableCollection<ResolutionConfigDataViewModel>
                {
                    resolutionConfig
                };

            return newLayout;
        }

        private void CloneLayout(LayoutConfigDataViewModel layout)
        {
            Logger.Info("Clone layout '{0}'", layout.Name.Value);

            var historyEntry = new CloneLayoutHistoryEntry(
                layout,
                this.MediaShell.MediaApplicationState,
                this.commandRegistry,
                MediaStrings.MediaShellController_CloneLayoutHistoryEntry);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void DeleteLayout(LayoutConfigDataViewModel layout)
        {
            if (layout.ReferencesCount > 0)
            {
                ShowLayoutCycleSectionsMessageBox(layout);
                return;
            }

            var state = this.MediaShell.MediaApplicationState;
            Logger.Info("Layout {0} removed from project {1}", layout.Name, state.CurrentProject.FileName);

            var historyEntry = new DeleteLayoutHistoryEntry(
                layout,
                state,
                this.commandRegistry,
                MediaStrings.MediaShellController_DeleteLayoutHistoryEntry);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }
    }
}
