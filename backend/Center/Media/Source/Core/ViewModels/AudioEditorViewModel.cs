// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioEditor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.Interaction;

    /// <summary>
    /// the audio editor view model
    /// </summary>
    public class AudioEditorViewModel : EditorViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioEditorViewModel"/> class.
        /// </summary>
        public AudioEditorViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioEditorViewModel"/> class
        /// </summary>
        /// <param name="parent">the parent view model</param>
        /// <param name="commandRegistry">the Command Registry</param>
        public AudioEditorViewModel(IMediaShell parent, ICommandRegistry commandRegistry)
            : base(parent, commandRegistry)
        {
        }

        /// <summary>
        /// Gets the Dynamic TTS Interaction Request.
        /// </summary>
        public IInteractionRequest EditDynamicTtsInteractionRequest
        {
            get
            {
                return InteractionManager<AudioDictionarySelectorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the audio file selection Interaction Request.
        /// </summary>
        public IInteractionRequest SelectAudioFileInteractionRequest
        {
            get
            {
                return InteractionManager<AudioFileSelectionPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the CreateLayoutElement Command.
        /// </summary>
        public override ICommand CreateLayoutElementCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.Audio.CreateElement);
            }
        }

        /// <summary>
        /// Gets the ShowElementEditPopup command.
        /// </summary>
        public override ICommand ShowElementEditPopupCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.Audio.ShowLayoutEditPopup);
            }
        }

        /// <summary>
        /// Gets the snap configuration.
        /// </summary>
        /// <remarks>Edge snap is disabled for the audio editor.</remarks>
        public override SnapConfiguration SnapConfiguration
        {
            get
            {
                return SnapConfiguration.Disabled;
            }
        }

        /// <summary>
        /// Gets the paste configuration.
        /// </summary>
        public override PasteConfiguration PasteConfiguration
        {
            get
            {
                return PasteConfiguration.Audio;
            }
        }
    }
}