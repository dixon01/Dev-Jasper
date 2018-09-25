// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateLayoutElementHistoryEntry.cs" company="Gorba AG">
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
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a layout element.
    /// </summary>
    public class CreateLayoutElementHistoryEntry : RestoreSelectionElementHistoryBase
    {
        private readonly LayoutElementDataViewModelBase element;
        private readonly ICommandRegistry commandRegistry;
        private readonly IMediaApplicationState state;

        private readonly ResolutionConfigDataViewModel resolution;

        private List<LayoutElementDataViewModelBase> previousSelectedElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateLayoutElementHistoryEntry"/> class.
        /// </summary>
        /// <param name="element">
        /// The element data view model.
        /// </param>
        /// <param name="editor">
        /// The editor that contains the element collection.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="state">
        /// The application state.
        /// </param>
        /// <param name="displayText">
        /// The display text shown on the UI.
        /// </param>
        public CreateLayoutElementHistoryEntry(
            LayoutElementDataViewModelBase element,
            IEditorViewModel editor,
            ICommandRegistry commandRegistry,
            IMediaApplicationState state,
            string displayText)
            : base(displayText)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }

            this.element = element;
            this.Editor = editor;
            this.commandRegistry = commandRegistry;
            this.state = state;

            var layout = this.state.CurrentLayout as LayoutConfigDataViewModel;
            if (layout == null)
            {
                throw new InvalidCastException("LayoutElement creation requires a layout with resolution.");
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
            this.element.MakeDirty();
            this.resolution.Elements.Remove(this.element);
            if (this.element is GraphicalElementDataViewModelBase)
            {
                this.Editor.Elements.Remove((GraphicalElementDataViewModelBase)this.element);
            }
            else
            {
                this.Editor.CurrentAudioOutputElement.Elements.Remove((PlaybackElementDataViewModelBase)this.element);
                for (var i = 0; i < this.Editor.CurrentAudioOutputElement.Elements.Count; i++)
                {
                    this.Editor.CurrentAudioOutputElement.Elements[i].ListIndex.Value = i;
                }

                foreach (var audioElement in this.previousSelectedElements)
                {
                    audioElement.IsItemSelected = true;
                }
            }

            this.UnsetMediaReferences();
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            base.Do();
            this.element.MakeDirty();
            this.Editor.SelectedElements.Clear();

            if (this.element is GraphicalElementDataViewModelBase)
            {
                this.resolution.Elements.Insert(0, this.element);
                this.Editor.Elements.Insert(0, (GraphicalElementDataViewModelBase)this.element);
                this.Editor.SelectedElements.Add(this.element);
            }
            else
            {
                this.previousSelectedElements = new List<LayoutElementDataViewModelBase>();
                foreach (
                    var audioBaseElement in this.Editor.CurrentAudioOutputElement.Elements.Where(e => e.IsItemSelected))
                {
                    this.previousSelectedElements.Add(audioBaseElement);
                    audioBaseElement.IsItemSelected = false;
                }

                var audioElement = this.element as PlaybackElementDataViewModelBase;
                if (audioElement == null)
                {
                    return;
                }

                this.Editor.CurrentAudioOutputElement.Elements.Insert(audioElement.ListIndex.Value, audioElement);
                for (int i = 0; i < this.Editor.CurrentAudioOutputElement.Elements.Count; i++)
                {
                    this.Editor.CurrentAudioOutputElement.Elements[i].ListIndex.Value = i;
                }

                if (this.resolution.Elements.SequenceEqual(this.Editor.CurrentAudioOutputElement.Elements))
                {
                    this.Editor.SelectedElements.Add(this.element);
                }
            }

            this.SetMediaReferences();
        }

        private void SetMediaReferences()
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var textElement = this.element as TextElementDataViewModel;
            if (textElement != null)
            {
                resourceManager.TextElementManager.SetReferences(textElement);
                return;
            }

            var clock = this.element as AnalogClockElementDataViewModel;
            if (clock != null)
            {
                resourceManager.AnalogClockElementManager.SetReferences(clock);
                var selectionParametersHour = new SelectResourceParameters
                                                  {
                                                      CurrentSelectedResourceHash =
                                                          this.state.CurrentProject.GetMediaHash(
                                                                   clock.Hour.Filename.Value)
                                                  };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersHour);

                var selectionParametersMinutes = new SelectResourceParameters
                                                     {
                                                         CurrentSelectedResourceHash =
                                                             this.state.CurrentProject.GetMediaHash(
                                                                      clock.Minute.Filename.Value)
                                                     };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersMinutes);

                var selectionParametersSeconds = new SelectResourceParameters
                                                     {
                                                         CurrentSelectedResourceHash =
                                                             this.state.CurrentProject.GetMediaHash(
                                                                      clock.Seconds.Filename.Value)
                                                     };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersSeconds);
            }
        }

        private void UnsetMediaReferences()
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var textElement = this.element as TextElementDataViewModel;
            if (textElement != null)
            {
                resourceManager.TextElementManager.UnsetReferences(textElement);
                return;
            }

            var clock = this.element as AnalogClockElementDataViewModel;
            if (clock != null)
            {
                resourceManager.AnalogClockElementManager.UnsetReferences(clock);
                var selectionParametersHour = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash =
                        this.state.CurrentProject.GetMediaHash(
                                 clock.Hour.Filename.Value)
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersHour);

                var selectionParametersMinutes = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash =
                        this.state.CurrentProject.GetMediaHash(
                                 clock.Minute.Filename.Value)
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersMinutes);

                var selectionParametersSeconds = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash =
                        this.state.CurrentProject.GetMediaHash(
                                 clock.Seconds.Filename.Value)
                };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParametersSeconds);
            }
        }
    }
}