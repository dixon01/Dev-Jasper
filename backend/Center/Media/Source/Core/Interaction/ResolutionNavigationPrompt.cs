// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionNavigationPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ResolutionNavigationPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// the Resolution Navigation Prompt
    /// </summary>
    public class ResolutionNavigationPrompt : PromptNotification
    {
        private readonly Lazy<ExtendedObservableCollection<string>> lazyAvailableResolutions;

        private readonly Lazy<ExtendedObservableCollection<MasterLayout>> lazyMasterLayoutTemplates;

        private readonly ICommandRegistry commandRegistry;

        private readonly IMediaShell mediaShell;

        private PhysicalScreenConfigDataViewModel highlightedPhysicalScreen;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionNavigationPrompt"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ResolutionNavigationPrompt(IMediaShell mediaShell, ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.mediaShell = mediaShell;
            this.HighlightedPhysicalScreen = this.CurrentPhysicalScreen;
            this.lazyAvailableResolutions =
                new Lazy<ExtendedObservableCollection<string>>(this.GetAvailableResolutions);
            this.lazyMasterLayoutTemplates =
                new Lazy<ExtendedObservableCollection<MasterLayout>>(this.GetMasterLayoutTemplates);
            this.mediaShell.MediaApplicationState.PropertyChanged += this.CurrentPhysicalScreenChanged;
        }

        /// <summary>
        /// Gets the current physical screen.
        /// </summary>
        public PhysicalScreenConfigDataViewModel CurrentPhysicalScreen
        {
            get
            {
                return this.Parent.MediaApplicationState.CurrentPhysicalScreen;
            }
        }

        /// <summary>
        /// Gets the physical screens.
        /// </summary>
        public ExtendedObservableCollection<PhysicalScreenConfigDataViewModel> PhysicalScreens
        {
            get
            {
                return this.Parent.MediaApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens;
            }
        }

        /// <summary>
        /// Gets the parent view model.
        /// </summary>
        public IMediaShell Parent
        {
            get
            {
                return this.mediaShell;
            }
        }

        /// <summary>
        /// Gets the show formula editor command.
        /// </summary>
        public ICommand ShowFormulaEditorCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowResolutionFormulaEditor);
            }
        }

        /// <summary>
        /// Gets the RemoveFormula command.
        /// </summary>
        public ICommand RemoveFormulaCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.RemoveFormula);
            }
        }

        /// <summary>
        /// Gets the choose physical screen command.
        /// </summary>
        public ICommand ChoosePhysicalScreenCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.Choose);
            }
        }

        /// <summary>
        /// Gets the show create physical screen command.
        /// </summary>
        public ICommand ShowCreatePhysicalScreenCommand
        {
            get
            {
                return
                    this.commandRegistry.GetCommand(
                        CommandCompositionKeys.Shell.PhysicalScreen.ShowCreatePhysicalScreenPopup);
            }
        }

        /// <summary>
        /// Gets the delete physical screen command.
        /// </summary>
        public ICommand DeletePhysicalScreenCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.Delete);
            }
        }

        /// <summary>
        /// Gets the clone physical screen command.
        /// </summary>
        public ICommand ClonePhysicalScreenCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.Clone);
            }
        }

        /// <summary>
        /// Gets the rename physical screen command.
        /// </summary>
        public ICommand RenamePhysicalScreenCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.Rename);
            }
        }

        /// <summary>
        /// Gets the FormulaEditor Interaction Request for the resolution navigation.
        /// </summary>
        public IInteractionRequest FormulaResolutionEditorInteractionRequest
        {
            get
            {
                return InteractionManager<FormulaResolutionEditorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the CreatePhysicalScreen Interaction Request for the resolution navigation.
        /// </summary>
        public IInteractionRequest CreatePhysicalScreenInteractionRequest
        {
            get
            {
                return InteractionManager<CreatePhysicalScreenPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets or sets the highlighted physical screen.
        /// </summary>
        public PhysicalScreenConfigDataViewModel HighlightedPhysicalScreen
        {
            get
            {
                return this.highlightedPhysicalScreen;
            }

            set
            {
                this.SetProperty(ref this.highlightedPhysicalScreen, value, () => this.HighlightedPhysicalScreen);
            }
        }

        /// <summary>
        /// Gets the available resolutions
        /// </summary>
        public ExtendedObservableCollection<string> AvailableResolutions
        {
            get
            {
                return this.lazyAvailableResolutions.Value;
            }
        }

        /// <summary>
        /// Gets the master layout templates.
        /// </summary>
        public ExtendedObservableCollection<MasterLayout> MasterLayoutTemplates
        {
            get
            {
                return this.lazyMasterLayoutTemplates.Value;
            }
        }

        private void CurrentPhysicalScreenChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPhysicalScreen")
            {
                this.RaisePropertyChanged(() => this.CurrentPhysicalScreen);
            }
        }

        private ExtendedObservableCollection<MasterLayout> GetMasterLayoutTemplates()
        {
            var configuration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
            var masterLayouts = new ExtendedObservableCollection<MasterLayout>();
            foreach (
                var layout in
                    configuration.PhysicalScreenSettings.PhysicalScreenTypes[0].AvailableResolutions[1].MasterLayouts)
            {
                masterLayouts.Add(layout);
            }

            return masterLayouts;
        }

        private ExtendedObservableCollection<string> GetAvailableResolutions()
        {
            var configuration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
            var resolutions = new ExtendedObservableCollection<string>();
            foreach (
                var availableResolution in
                    configuration.PhysicalScreenSettings.PhysicalScreenTypes[0].AvailableResolutions)
            {
                resolutions.Add(string.Format("{0}x{1}", availableResolution.Width, availableResolution.Height));
            }

            return resolutions;
        }
    }
}