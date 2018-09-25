// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteLayoutElementsHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of a layout element.
    /// </summary>
    public class DeleteLayoutElementsHistoryEntry : HistoryEntryBase
    {
        private readonly Lazy<IResourceManager> lazyResourceManager =
          new Lazy<IResourceManager>(() => ServiceLocator.Current.GetInstance<IResourceManager>());

        private readonly IEnumerable<LayoutElementDataViewModelBase> elements;

        private readonly IEditorViewModel editor;

        private readonly ICommandRegistry commandRegistry;

        private readonly MediaProjectDataViewModel project;

        private readonly IMediaApplicationState state;

        private readonly ResolutionConfigDataViewModel resolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteLayoutElementsHistoryEntry"/> class.
        /// </summary>
        /// <param name="elements">
        ///     The list of selected elements to be deleted.
        /// </param>
        /// <param name="editor">
        ///     The editor that contains the list of elements.
        /// </param>
        /// <param name="commandRegistry">
        ///     The command registry.
        /// </param>
        /// <param name="state">the state</param>
        /// <param name="displayText">
        ///     The text to be displayed for this Entry on the UI.
        /// </param>
        public DeleteLayoutElementsHistoryEntry(
            IEnumerable<LayoutElementDataViewModelBase> elements,
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
            this.editor = editor;
            this.commandRegistry = commandRegistry;
            this.state = state;
            this.project = this.state.CurrentProject;
            var layout = this.state.CurrentLayout as LayoutConfigDataViewModel;
            if (layout == null)
            {
                throw new InvalidCastException("LayoutElement deletion requires a layout with resolution.");
            }

            var screen = this.state.CurrentVirtualDisplay;
            this.resolution = layout.IndexedResolutions[screen.Width.Value, screen.Height.Value];
        }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        protected IResourceManager ResourceManager
        {
            get
            {
                return this.lazyResourceManager.Value;
            }
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            var isAudio = false;
            if (this.elements == null || this.editor == null)
            {
                return;
            }

            foreach (var element in this.elements)
            {
                element.MakeDirty();
                this.editor.SelectedElements.Remove(element);
                var graphicalElement = element as GraphicalElementDataViewModelBase;
                if (graphicalElement != null)
                {
                    this.editor.Elements.Remove(graphicalElement);
                    this.resolution.Elements.Remove(graphicalElement);
                }
                else
                {
                    var audioElement = element as PlaybackElementDataViewModelBase;
                    if (audioElement != null)
                    {
                        isAudio = true;
                        this.editor.CurrentAudioOutputElement.Elements.Remove(audioElement);
                    }
                }

                this.UnsetMediaReferences(element);
                this.UnsetFontReferences(element);
                this.UnsetPredefinedFormulaReferences(element);
            }

            if (!isAudio)
            {
                var depthIndex = this.editor.Elements.Count - 1;
                foreach (var layoutElement in this.editor.Elements.OfType<DrawableElementDataViewModelBase>())
                {
                    layoutElement.ZIndex.Value = depthIndex;
                    depthIndex--;
                }

                return;
            }

            for (var i = 0; i < this.editor.CurrentAudioOutputElement.Elements.Count; i++)
            {
                this.editor.CurrentAudioOutputElement.Elements[i].ListIndex.Value = i;
            }
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            var isAudio = false;
            var resultElementsCount = this.editor.Elements.Count + this.elements.Count();
            foreach (var element in this.elements)
            {
                var graphicalElement = element as GraphicalElementDataViewModelBase;
                if (graphicalElement != null)
                {
                    var insertIndex =
                        resultElementsCount - 1 - ((DrawableElementDataViewModelBase)element).ZIndex.Value;
                    if (insertIndex < 0)
                    {
                        insertIndex = 0;
                    }

                    if (insertIndex > this.resolution.Elements.Count)
                    {
                        insertIndex = this.resolution.Elements.Count;
                    }

                    this.resolution.Elements.Insert(insertIndex, element);
                    this.editor.Elements.Insert(insertIndex, graphicalElement);

                    if (this.resolution.Elements.SequenceEqual(this.editor.Elements))
                    {
                        this.editor.SelectedElements.Add(element);
                    }
                }
                else
                {
                    var audioElement = element as PlaybackElementDataViewModelBase;
                    if (audioElement != null)
                    {
                        isAudio = true;
                        var insertIndex = audioElement.ListIndex.Value;
                        if (insertIndex > this.editor.CurrentAudioOutputElement.Elements.Count)
                        {
                            insertIndex = this.editor.CurrentAudioOutputElement.Elements.Count;
                        }

                        this.editor.CurrentAudioOutputElement.Elements.Insert(insertIndex, audioElement);

                        if (this.resolution.Elements.Count == 1
                            && this.resolution.Elements.First().Equals(this.editor.CurrentAudioOutputElement))
                        {
                            this.editor.SelectedElements.Add(element);
                        }
                    }
                }

                this.SetMediaReferences(element);
                this.SetFontReferences(element);
                this.ResetPredefinedFormulaReferences(element);
            }

            if (!isAudio)
            {
                var depthIndex = this.editor.Elements.Count - 1;
                foreach (var layoutElement in this.editor.Elements.OfType<DrawableElementDataViewModelBase>())
                {
                    layoutElement.ZIndex.Value = depthIndex;
                    depthIndex--;
                }

                return;
            }

            for (var i = 0; i < this.editor.CurrentAudioOutputElement.Elements.Count; i++)
            {
                this.editor.CurrentAudioOutputElement.Elements[i].ListIndex.Value = i;
            }
        }

        private void SetFontReferences(LayoutElementDataViewModelBase element)
        {
            var textElement = element as TextElementDataViewModel;
            if (textElement == null)
            {
                return;
            }

            var resourceFont =
                this.state.CurrentProject.Resources.FirstOrDefault(f => f.Facename == textElement.FontFace.Value);
            if (resourceFont == null)
            {
                return;
            }

            var selectionParameters = new SelectResourceParameters
            {
                CurrentSelectedResourceHash = resourceFont.Hash
            };

            this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                .Execute(selectionParameters);
            this.ResourceManager.TextElementManager.SetReferences(textElement);
        }

        private void UnsetFontReferences(LayoutElementDataViewModelBase element)
        {
            var textElement = element as TextElementDataViewModel;
            if (textElement == null)
            {
                return;
            }

            var resourceFont =
                this.state.CurrentProject.Resources.FirstOrDefault(f => f.Facename == textElement.FontFace.Value);
            if (resourceFont == null)
            {
                return;
            }

            var selectionParameters = new SelectResourceParameters
            {
                PreviousSelectedResourceHash = resourceFont.Hash
            };

            this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                .Execute(selectionParameters);
            this.ResourceManager.TextElementManager.UnsetReferences(textElement);
        }

        private void UnsetMediaReferences(LayoutElementDataViewModelBase element)
        {
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
                this.ResourceManager.ImageElementManager.UnsetReferences(image);
                return;
            }

            var video = element as VideoElementDataViewModel;
            if (video != null)
            {
                var selectionParameters = new SelectResourceParameters { PreviousSelectedResourceHash = video.Hash };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                this.ResourceManager.VideoElementManager.UnsetReferences(video);
                return;
            }

            var audio = element as AudioFileElementDataViewModel;
            if (audio != null)
            {
                var res = audio.AudioFile;
                var selectionParameters = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash =
                        res == null ? null : res.Hash
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                this.ResourceManager.AudioElementManager.UnsetReferences(audio);
                return;
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
                this.ResourceManager.AnalogClockElementManager.UnsetReferences(clock);
            }
        }

        private void SetMediaReferences(LayoutElementDataViewModelBase element)
        {
            var image = element as ImageElementDataViewModel;
            if (image != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  CurrentSelectedResourceHash =
                                                      image.Image == null ? null : image.Image.Hash
                                              };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                this.ResourceManager.ImageElementManager.SetReferences(image);
                return;
            }

            var video = element as VideoElementDataViewModel;
            if (video != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  CurrentSelectedResourceHash = video.Hash
                                              };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                this.ResourceManager.VideoElementManager.SetReferences(video);
                return;
            }

            var audioFile = element as AudioFileElementDataViewModel;
            if (audioFile != null)
            {
                var selectionParameters = new SelectResourceParameters
                {
                    CurrentSelectedResourceHash =
                        audioFile.AudioFile == null ? null : audioFile.AudioFile.Hash
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                this.ResourceManager.AudioElementManager.SetReferences(audioFile);
                return;
            }

            var clock = element as AnalogClockElementDataViewModel;
            if (clock != null)
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
                this.ResourceManager.AnalogClockElementManager.SetReferences(clock);
            }
        }

        private void UnsetPredefinedFormulaReferences(LayoutElementDataViewModelBase element)
        {
            if (element == null)
            {
                return;
            }

            var predefinedFormulas = element.GetContainedPredefinedFormulas();
            predefinedFormulas.DecreaseReferencesCount(this.project);
        }

        private void ResetPredefinedFormulaReferences(LayoutElementDataViewModelBase element)
        {
            if (element == null)
            {
                return;
            }

            var predefinedFormulas = element.GetContainedPredefinedFormulas();
            predefinedFormulas.IncreaseReferencesCount(this.project);
        }
    }
}
