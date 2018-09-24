// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerNavigationEditorPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The trigger navigation editor prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The formula navigation editor prompt.
    /// </summary>
    public class TriggerNavigationEditorPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        private GenericTriggerConfigDataViewModel dataValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerNavigationEditorPrompt"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="dataValue">
        /// The data value.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public TriggerNavigationEditorPrompt(
            IMediaShell shell,
            GenericTriggerConfigDataViewModel dataValue,
            ICommandRegistry commandRegistry)
        {
            this.dataValue = dataValue;
            this.Shell = shell;
            this.commandRegistry = commandRegistry;
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the current DataValue
        /// </summary>
        public GenericTriggerConfigDataViewModel DataValue
        {
            get
            {
                return this.dataValue;
            }

            set
            {
                this.SetProperty(ref this.dataValue, value, () => this.DataValue);
            }
        }

        /// <summary>
        /// Gets the show dictionary selector command.
        /// </summary>
        public ICommand ShowDictionarySelectorCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowTriggerEditorDictionarySelector);
            }
        }

        /// <summary>
        /// Gets the dictionary selector interaction request.
        /// </summary>
        public IInteractionRequest DictionarySelectorInteractionRequest
        {
            get
            {
                return InteractionManager<DictionaryTriggerEditorSelectorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the media shell
        /// </summary>
        public IMediaShell Shell { get; private set; }

        private void Initialize()
        {
        }
    }
}