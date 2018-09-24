// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasteElementsHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The history entry for the paste of layout elements.
    /// </summary>
    public class PasteElementsHistoryEntry : RestoreSelectionElementHistoryBase
    {
        private readonly List<LayoutElementDataViewModelBase> elements;

        private readonly ICommandRegistry commandRegistry;

        private readonly IMediaApplicationState state;

        private readonly ResolutionConfigDataViewModel resolution;

        private readonly bool isAudio;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasteElementsHistoryEntry"/> class.
        /// </summary>
        /// <param name="elements">
        /// The element data view models.
        /// </param>
        /// <param name="editor">
        /// The editor that contains the element collection.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="state">
        /// The project state
        /// </param>
        /// <param name="displayText">
        /// The display text shown on the UI.
        /// </param>
        public PasteElementsHistoryEntry(
            List<LayoutElementDataViewModelBase> elements,
            IEditorViewModel editor,
            ICommandRegistry commandRegistry,
            IMediaApplicationState state,
            string displayText)
            : base(displayText)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }

            this.elements = elements;
            this.Editor = editor;
            this.isAudio = this.Editor is AudioEditorViewModel;
            this.commandRegistry = commandRegistry;
            this.state = state;

            var layout = this.state.CurrentLayout as LayoutConfigDataViewModel;
            if (layout == null)
            {
                throw new InvalidCastException("LayoutElement pasting requires a layout with resolution.");
            }

            var screen = this.state.CurrentVirtualDisplay;
            this.resolution = layout.IndexedResolutions[screen.Width.Value, screen.Height.Value];
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            base.Undo();

            foreach (var element in this.elements)
            {
                element.MakeDirty();
                this.resolution.Elements.Remove(element);
                if (this.isAudio)
                {
                    this.Editor.CurrentAudioOutputElement.Elements.Remove((PlaybackElementDataViewModelBase)element);
                }
                else
                {
                    this.Editor.Elements.Remove((GraphicalElementDataViewModelBase)element);
                }

                this.UnsetMediaReferences(element);
            }
        }

        /// <summary>
        /// Executes the logic of this entry for the first time.
        /// </summary>
        public void InitialDo()
        {
            base.Do();
            this.Execute(false);
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            base.Do();
            this.Execute(true);
        }

        private void Execute(bool doIncreasePredefinedFormulaReferenceCount)
        {
            this.Editor.SelectedElements.Clear();
            foreach (var element in this.elements)
            {
                element.MakeDirty();
                if (this.isAudio)
                {
                    this.resolution.Elements.Add(element);
                    this.Editor.CurrentAudioOutputElement.Elements.Add((PlaybackElementDataViewModelBase)element);
                }
                else
                {
                    this.resolution.Elements.Insert(0, element);
                    this.Editor.Elements.Insert(0, (GraphicalElementDataViewModelBase)element);
                }

                this.Editor.SelectedElements.Add(element);
                this.SetMediaReferences(element, doIncreasePredefinedFormulaReferenceCount);
            }

            var depthIndex = this.Editor.Elements.Count - 1;
            foreach (var layoutElement in this.Editor.Elements.OfType<DrawableElementDataViewModelBase>())
            {
                layoutElement.ZIndex.Value = depthIndex;
                depthIndex--;
            }
        }

        private void SetMediaReferences(
            LayoutElementDataViewModelBase element,
            bool doIncreasePredefinedFormulaReferenceCount)
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var image = element as ImageElementDataViewModel;
            var video = element as VideoElementDataViewModel;
            var clock = element as AnalogClockElementDataViewModel;
            var audioFile = element as AudioFileElementDataViewModel;
            if (image != null)
            {
                var selectionParameters = new SelectResourceParameters
                {
                    CurrentSelectedResourceHash =
                        image.Image == null ? null : image.Image.Hash
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                resourceManager.ImageElementManager.SetReferences(image);
            }
            else if (video != null)
            {
                var selectionParameters = new SelectResourceParameters
                {
                    CurrentSelectedResourceHash = video.Hash
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                resourceManager.VideoElementManager.SetReferences(video);
            }
            else if (clock != null)
            {
                var selectionParametersHour = new SelectResourceParameters
                                                  {
                                                      CurrentSelectedResourceHash =
                                                          clock.Hour.Shell.MediaApplicationState
                                                               .CurrentProject.GetMediaHash(
                                                                   clock.Hour.Filename.Value)
                                                  };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersHour);

                var selectionParametersMinutes = new SelectResourceParameters
                                                     {
                                                         CurrentSelectedResourceHash =
                                                             clock.Minute.Shell.MediaApplicationState
                                                                  .CurrentProject.GetMediaHash(
                                                                      clock.Minute.Filename.Value)
                                                     };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersMinutes);

                var selectionParametersSeconds = new SelectResourceParameters
                                                     {
                                                         CurrentSelectedResourceHash =
                                                             clock.Seconds.Shell.MediaApplicationState
                                                                  .CurrentProject.GetMediaHash(
                                                                      clock.Seconds.Filename.Value)
                                                     };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersSeconds);
                resourceManager.AnalogClockElementManager.SetReferences(clock);
            }
            else if (audioFile != null)
            {
                var selectionParameters = new SelectResourceParameters
                {
                    CurrentSelectedResourceHash = audioFile.AudioFile == null ? null : audioFile.AudioFile.Hash
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                resourceManager.AudioElementManager.SetReferences(audioFile);
            }

            if (doIncreasePredefinedFormulaReferenceCount)
            {
                this.UpdatePredefinedFormulaReferenceCount(element);
            }
        }

        private void UpdatePredefinedFormulaReferenceCount(LayoutElementDataViewModelBase element)
        {
            var predefinedFormulas = element.GetContainedPredefinedFormulas();
            foreach (var predefinedFormula in predefinedFormulas)
            {
                predefinedFormula.ReferencesCount++;
            }
        }

        private void UnsetMediaReferences(LayoutElementDataViewModelBase element)
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var image = element as ImageElementDataViewModel;
            if (image != null)
            {
                var res = image.Image;
                var selectionParameters = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash =
                        res == null ? null : res.Hash
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                resourceManager.ImageElementManager.UnsetReferences(image);
                return;
            }

            var video = element as VideoElementDataViewModel;
            if (video != null)
            {
                var selectionParameters = new SelectResourceParameters { PreviousSelectedResourceHash = video.Hash };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                resourceManager.VideoElementManager.UnsetReferences(video);
            }

            var clock = element as AnalogClockElementDataViewModel;
            if (clock != null)
            {
                var selectionParametersHour = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash =
                        clock.Hour.Shell.MediaApplicationState
                             .CurrentProject.GetMediaHash(
                                 clock.Hour.Filename.Value)
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersHour);

                var selectionParametersMinutes = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash =
                        clock.Minute.Shell.MediaApplicationState
                             .CurrentProject.GetMediaHash(
                                 clock.Minute.Filename.Value)
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersMinutes);

                var selectionParametersSeconds = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash =
                        clock.Seconds.Shell.MediaApplicationState
                             .CurrentProject.GetMediaHash(
                                 clock.Seconds.Filename.Value)
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersSeconds);
                resourceManager.AnalogClockElementManager.UnsetReferences(clock);
            }

            var audioFile = element as AudioFileElementDataViewModel;
            if (audioFile != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  PreviousSelectedResourceHash =
                                                      audioFile.AudioFile == null ? null : audioFile.AudioFile.Hash
                                              };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                resourceManager.AudioElementManager.UnsetReferences(audioFile);
            }

            var predefinedFormulas = element.GetContainedPredefinedFormulas();
            foreach (var predefinedFormula in predefinedFormulas)
            {
                predefinedFormula.ReferencesCount--;
            }
        }
    }
}
