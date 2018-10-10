// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioEditorController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILayoutEditorController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.EditorControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The graphical editor controller base.
    /// </summary>
    public class AudioEditorController : EditorControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioEditorController"/> class.
        /// </summary>
        /// <param name="shellController">
        /// The shell controller.
        /// </param>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public AudioEditorController(
            IMediaShellController shellController,
            IMediaShell shell,
            ICommandRegistry commandRegistry)
            : base(shellController, shell, commandRegistry)
        {
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.Audio.CreateElement,
                new RelayCommand<CreateElementParameters>(this.CreateElement));

            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Layout.Audio.ShowLayoutEditPopup,
               new RelayCommand<LayoutElementDataViewModelBase>(this.ShowEditPopup));
        }

        /// <summary>
        /// The get progressive number.
        /// </summary>
        /// <param name="elementTypeName">
        /// The element type name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected override int GetProgressiveNumber(string elementTypeName)
        {
           var editor = this.Shell.Editors[PhysicalScreenType.Audio];
           var existingElements =
                    editor.CurrentAudioOutputElement.Elements.Where(e => e.GetType().Name.Contains(elementTypeName));

           return this.CreateNextProgressiveNumber(existingElements);
        }

        /// <summary>
        /// The show edit popup.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        protected override void ShowEditPopup(LayoutElementDataViewModelBase parameter)
        {
            var selectedElements = ((MediaShell)this.Shell).Editor.SelectedElements;
            if (parameter == null && selectedElements.Count == 1)
            {
                parameter = selectedElements.First();
            }

            if (parameter is AudioFileElementDataViewModel)
            {
                this.ShowEditAudioPopup(parameter as AudioFileElementDataViewModel);
                return;
            }

            if (parameter is DynamicTtsElementDataViewModel)
            {
                this.ShowEditTtsPopup(parameter as DynamicTtsElementDataViewModel);
                return;
            }

            Logger.Debug("Show edit popup not handled");
        }

        private void ShowEditAudioPopup(AudioFileElementDataViewModel audioElement)
        {
            Logger.Debug("Request to show the Edit audio popup.");
            var placementTarget = new CustomPlacementTarget
            {
                UseMousePosition = new DataValue<bool>(true),
            };

            var prompt = new AudioFileSelectionPrompt(
                audioElement,
                this.Shell,
                this.CommandRegistry,
                this.RecentMediaResources) { PlacementTarget = placementTarget };
            InteractionManager<AudioFileSelectionPrompt>.Current.Raise(prompt);
        }

        private void ShowEditTtsPopup(DynamicTtsElementDataViewModel dynamicTtsElement)
        {
            Logger.Debug("Request to show the DictionarySelector Popup for a Dynamic TTS text.");
            var dataValue = dynamicTtsElement.Value;
            var evaluation = string.Empty;
            if (dataValue.Formula == null)
            {
                if (dataValue.Value != null && dataValue.Value.StartsWith("$"))
                {
                    evaluation = dataValue.Value;
                }
            }
            else
            {
                evaluation = ((EvalDataViewModelBase)dataValue.Formula).HumanReadable();
            }

            Action<DictionarySelectorPrompt> action = prompt =>
            {
                var oldElement = (LayoutElementDataViewModelBase)dynamicTtsElement.Clone();
                var generic = new GenericEvalDataViewModel(this.Shell);
                if (prompt.SelectedDictionaryValue.Column != null)
                {
                    generic.Column.Value = prompt.SelectedDictionaryValue.Column.Index;
                }

                if (prompt.SelectedDictionaryValue.Table != null)
                {
                    generic.Table.Value = prompt.SelectedDictionaryValue.Table.Index;
                }

                if (prompt.SelectedDictionaryValue.Language != null)
                {
                    generic.Language.Value = prompt.SelectedDictionaryValue.Language.Index;
                }

                generic.Row.Value = prompt.SelectedDictionaryValue.Row;
                dynamicTtsElement.Value.Formula = generic;

                var newElement = (LayoutElementDataViewModelBase)dynamicTtsElement.Clone();

                var editor = this.Shell.Editors[PhysicalScreenType.Audio] as AudioEditorViewModel;
                if (editor != null)
                {
                    var parameters = new UpdateEntityParameters(
                        new List<DataViewModelBase> { oldElement },
                        new List<DataViewModelBase> { newElement },
                        editor.CurrentAudioOutputElement.Elements);
                    editor.UpdateElementCommand.Execute(parameters);
                }
                else
                {
                    Logger.Error("AudioEditorController could not get AudioEditorViewModel.");
                }

                InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
            };

            var placementTarget = new CustomPlacementTarget
            {
                UseMousePosition = new DataValue<bool>(true),
                Height = new DataValue<int>(280),
                Width = new DataValue<int>(360)
            };

            var dictionaryPrompt =
                new AudioDictionarySelectorPrompt(evaluation, this.Shell, this.RecentDictionaryValues)
                {
                    IsOpen = true,
                    PlacementTarget = placementTarget,
                };
            InteractionManager<AudioDictionarySelectorPrompt>.Current.Raise(dictionaryPrompt, action);
        }

        private void CreateElement(CreateElementParameters createParams)
        {
            if (createParams == null)
            {
                throw new ArgumentNullException("createParams");
            }

            var editor = this.Shell.Editor;
            Logger.Info("Create new layout element: {0}", createParams);
            if (editor == null)
            {
                Logger.Error("The LayoutEditor can't be null.");
                return;
            }

            LayoutElementDataViewModelBase element;
            string displayText;
            switch (createParams.Type)
            {
                case LayoutElementType.AudioFile:
                    element = this.CreateAudioFileElement(createParams, out displayText);
                    break;
                case LayoutElementType.AudioPause:
                    element = this.CreateAudioPauseElement(createParams, out displayText);
                    break;
                case LayoutElementType.TextToSpeech:
                    element = this.CreateStaticTtsElement(createParams, out displayText);
                    break;
                case LayoutElementType.DynamicTts:
                    element = this.CreateDynamicTtsElement(createParams, out displayText);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (element == null)
            {
                return;
            }

            var historyEntry = new CreateLayoutElementHistoryEntry(
                element, editor, this.CommandRegistry, this.Shell.MediaApplicationState, displayText);

            this.ShellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private AudioFileElementDataViewModel CreateAudioFileElement(
            CreateElementParameters createParams,
            out string displayText)
        {
            var element = new AudioFileElementDataViewModel(this.Shell);
            this.FillCommonProperties(element, createParams, PhysicalScreenType.Audio);
            element.ListIndex.Value = createParams.InsertIndex;
            element.IsItemSelected = true;
            displayText = MediaStrings.AudioEditorController_CreateAudioFileElementHistoryEntryLabel;
            return element;
        }

        private AudioPauseElementDataViewModel CreateAudioPauseElement(
           CreateElementParameters createParams,
           out string displayText)
        {
            var element = new AudioPauseElementDataViewModel(this.Shell)
            {
                Duration = { Value = Settings.Default.AudioPauseDefaultDuration },
                IsItemSelected = true
            };
            this.FillCommonProperties(element, createParams, PhysicalScreenType.Audio);
            element.ListIndex.Value = createParams.InsertIndex;
            displayText = MediaStrings.AudioEditorController_CreateAudioPauseElementHistoryEntryLabel;
            return element;
        }

        private TextToSpeechElementDataViewModel CreateStaticTtsElement(
           CreateElementParameters createParams,
           out string displayText)
        {
            var element = new TextToSpeechElementDataViewModel(this.Shell)
            {
                Voice = { Value = Settings.Default.TtsDefaultVoice },
                IsItemSelected = true
            };
            this.FillCommonProperties(element, createParams, PhysicalScreenType.Audio);
            element.ListIndex.Value = createParams.InsertIndex;
            displayText = MediaStrings.AudioEditorController_CreateAudioTtsElementHistoryEntryLabel;
            return element;
        }

        private DynamicTtsElementDataViewModel CreateDynamicTtsElement(
           CreateElementParameters createParams,
           out string displayText)
        {
            var element = new DynamicTtsElementDataViewModel(this.Shell)
            {
                Voice = { Value = Settings.Default.TtsDefaultVoice },
                IsItemSelected = true
            };
            this.FillCommonProperties(element, createParams, PhysicalScreenType.Audio);
            element.ListIndex.Value = createParams.InsertIndex;
            displayText = MediaStrings.AudioEditorController_CreateAudioDynamicTtsElementHistoryEntryLabel;
            return element;
        }
    }
}