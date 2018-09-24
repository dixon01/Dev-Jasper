// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralEditorController.cs" company="Gorba AG">
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
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Xml;
    using System.Xml.Serialization;

    using ClosedXML.Excel;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The graphical editor controller base.
    /// </summary>
    public class GeneralEditorController : EditorControllerBase
    {
        private const int DefaultMoveControlPressed = 10;

        private const int DefaultMoveControlShiftPressed = 50;

        private const int DefaultMoveControlShiftAltPressed = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralEditorController"/> class.
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
        public GeneralEditorController(
            IMediaShellController shellController,
            IMediaShell shell,
            ICommandRegistry commandRegistry)
            : base(shellController, shell, commandRegistry)
        {
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.DeleteSelectedElements,
                new RelayCommand(this.DeleteSelectedElements, this.CanDeleteElements));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.SelectElements,
                new RelayCommand<CreateElementParameters>(this.SelectElements));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.DeleteElements,
                new RelayCommand<IEnumerable<LayoutElementDataViewModelBase>>(
                    this.DeleteElements,
                    this.CanDeleteElements));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.SelectElement,
                new RelayCommand<SelectElementParameters>(this.SelectElement));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.MoveSelectedElements,
                new RelayCommand<MoveElementsCommandParameters>(this.MoveSelectedElements));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.UpdateElement,
                new RelayCommand<UpdateEntityParameters>(this.UpdateElements));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.ResizeElement,
                new RelayCommand<ResizeElementParameters>(this.ResizeElement));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Default.SelectAll,
                new RelayCommand(this.SelectAllElements));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowMediaSelector,
                new RelayCommand<TextualReplacementDataViewModel>(this.ShowMediaSelector));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Default.Copy,
                new RelayCommand(this.CopySelectedElementsToClipboard));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Default.Paste,
                new RelayCommand(this.PasteElements));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Default.Cut,
                new RelayCommand(this.CutElements));
        }

        /// <summary>
        /// The select element.
        /// </summary>
        /// <param name="selectionParameters">
        /// The selection parameters.
        /// </param>
        private void SelectElement(SelectElementParameters selectionParameters)
        {
            var editor = this.Shell.Editor;
            Logger.Debug("Select layout element at: {0}", selectionParameters);
            if (editor == null)
            {
                Logger.Error("The LayoutEditor can't be null.");
                return;
            }

            var selectionPosition = selectionParameters.Position;

            if (!selectionParameters.Modifiers.IsShiftPressed && !selectionParameters.Modifiers.IsControlPressed)
            {
                editor.SelectedElements.Clear();
            }

            if (selectionParameters.ClearSelection)
            {
                return;
            }

            var layoutElementDataViewModelBase = editor.Elements.GetElementAt(selectionPosition);
            if (layoutElementDataViewModelBase != null)
            {
                if (!editor.SelectedElements.Contains(layoutElementDataViewModelBase))
                {
                    editor.SelectedElements.Add(layoutElementDataViewModelBase);
                }
                else if (selectionParameters.Modifiers.IsControlPressed)
                {
                    editor.SelectedElements.Remove(layoutElementDataViewModelBase);
                }
            }
        }

        private void SelectElements(CreateElementParameters selectionParams)
        {
            var editor = this.Shell.Editor;
            Logger.Debug("Select layout elements within the bounds: {0}", selectionParams.Bounds);
            if (editor == null)
            {
                Logger.Error("The LayoutEditor can't be null.");
                return;
            }

            if (!selectionParams.Modifiers.IsShiftPressed && !selectionParams.Modifiers.IsControlPressed)
            {
                editor.SelectedElements.Clear();
            }

            var elements = editor.Elements.GetElementsIn(selectionParams.Bounds);
            foreach (var layoutElementDataViewModelBase in elements)
            {
                if (!editor.SelectedElements.Contains(layoutElementDataViewModelBase))
                {
                    editor.SelectedElements.Add(layoutElementDataViewModelBase);
                }
                else if (selectionParams.Modifiers.IsControlPressed)
                {
                    editor.SelectedElements.Remove(layoutElementDataViewModelBase);
                }
            }
        }

        /// <summary>
        /// The delete selected elements.
        /// </summary>
        private void DeleteSelectedElements()
        {
            var editor = this.Shell.Editor;
            if (editor != null)
            {
                this.DeleteElements(editor.SelectedElements);
            }
        }

        private void DeleteElements(IEnumerable<LayoutElementDataViewModelBase> parameter)
        {
            var editor = this.Shell.Editor;
            var elements = parameter.ToList();
            Logger.Info("Delete {0} layout elements", elements.Count());

            if (editor == null)
            {
                Logger.Error("The LayoutEditor can't be null.");
                return;
            }

            if (elements.Count == 0)
            {
                Logger.Debug("No elements to delete.");
                return;
            }

            try
            {
                var deletionEntry = new DeleteLayoutElementsHistoryEntry(
                    elements,
                    this.Shell.Editor,
                    this.CommandRegistry,
                    this.Shell.MediaApplicationState,
                   string.Format(MediaStrings.GeneralEditorController_DeleteElementsHistoryEntryLabel, elements.Count));
                this.ShellController.ChangeHistoryController.AddHistoryEntry(deletionEntry);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error deleting layout element.", e);
            }
        }

        private void UpdateElements(UpdateEntityParameters parameters)
        {
            var oldElements = parameters.OldElements.ToList();
            var newElements = parameters.NewElements.ToList();
            var elementsContainer = parameters.ElementsContainerReference;

            Action doCallBack = null;
            Action undoCallBack = null;
            if (parameters.NewResolution != null)
            {
                doCallBack = () =>
                {
                    foreach (var layout in
                        newElements.OfType<SectionConfigDataViewModelBase>()
                            .Select(section => (LayoutConfigDataViewModel)section.Layout))
                    {
                        layout.AddCurrentResolution();
                    }
                };
                undoCallBack = () =>
                {
                    foreach (var layout in
                        newElements.OfType<SectionConfigDataViewModelBase>()
                            .Select(section => (LayoutConfigDataViewModel)section.Layout))
                    {
                        layout.Resolutions.Remove(parameters.NewResolution);
                    }
                };
            }

            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                doCallBack,
                undoCallBack,
                MediaStrings.GeneralEditorController_UpdateLayoutHistoryEntryLabel,
                this.CommandRegistry);

            // No need to invoke the Do() method because the change was done by the WPF framework.
            foreach (var element in newElements)
            {
                element.MakeDirty();
            }

            int index;

            for (index = 0; index < newElements.Count(); index++)
            {
                var currentDataViewModelBase = newElements.ElementAt(index);
                var previousDataViewModelBase = oldElements.ElementAt(index);
                UpdateViewModelHistoryEntry.ExecutePostActionHook(currentDataViewModelBase, previousDataViewModelBase);
                index++;
            }

            this.ShellController.ChangeHistoryController.AddHistoryEntry(historyEntry, true);
        }

        private void ResizeElement(ResizeElementParameters parameters)
        {
            var historyEntry = new ResizeLayoutElementsHistoryEntry(
                parameters.Element,
                this.Shell.Editor,
                parameters.OldBounds,
                parameters.NewBounds,
                MediaStrings.GeneralEditorController_ResizeLayoutElementsHistoryEntryLabel);

            // No need to invoke the Do() method because the change was done by the WPF framework.
            parameters.Element.MakeDirty();
            this.ShellController.ChangeHistoryController.AddHistoryEntry(historyEntry, true);
        }

        private void MoveSelectedElements(MoveElementsCommandParameters parameters)
        {
            var editor = this.Shell.Editor;
            Logger.Debug("Move selected elements: {0}", parameters);
            if (editor == null)
            {
                Logger.Error("The LayoutEditor can't be null.");
                return;
            }

            var moveX = 0;
            var moveY = 0;

            if (parameters.Direction.HasValue)
            {
                var amount = 1;

                if (parameters.Modifiers.IsControlPressed && !parameters.Modifiers.IsShiftPressed
                    && !parameters.Modifiers.IsAltPressed)
                {
                    amount = DefaultMoveControlPressed;
                }
                else if (parameters.Modifiers.IsControlPressed && parameters.Modifiers.IsShiftPressed
                         && !parameters.Modifiers.IsAltPressed)
                {
                    amount = DefaultMoveControlShiftPressed;
                }
                else if (parameters.Modifiers.IsControlPressed && parameters.Modifiers.IsShiftPressed
                         && parameters.Modifiers.IsAltPressed)
                {
                    amount = DefaultMoveControlShiftAltPressed;
                }

                switch (parameters.Direction)
                {
                    case MovementDirection.Left:
                        moveX = -amount;
                        break;
                    case MovementDirection.Right:
                        moveX = amount;
                        break;
                    case MovementDirection.Up:
                        moveY = -amount;
                        break;
                    case MovementDirection.Down:
                        moveY = amount;
                        break;
                }
            }
            else
            {
                var delta = parameters.Delta;
                if (this.Shell.Editor.SnapConfiguration.IsAvailable && this.Shell.MediaApplicationState.UseEdgeSnap)
                {
                    delta = this.EdgeSnap(
                        parameters.Delta,
                        editor.SelectedElements.OfType<GraphicalElementDataViewModelBase>().ToList());
                }

                moveX = (int)delta.X;
                moveY = (int)delta.Y;
            }

            var displayText = string.Format(
                MediaStrings.GeneralEditorController_MoveLayoutElementsHistoryEntryLabel,
                editor.SelectedElements.Count);
            var historyEntry =
                new MoveLayoutElementsHistoryEntry(
                    new List<GraphicalElementDataViewModelBase>(
                        editor.SelectedElements.OfType<GraphicalElementDataViewModelBase>()),
                    editor,
                    moveX,
                    moveY,
                    displayText);
            this.ShellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private bool CanDeleteElements(object obj)
        {
            if (this.Shell.Editor != null)
            {
                return this.Shell.Editor.SelectedElements.Count > 0;
            }

            return false;
        }

        private void CopySelectedElementsToClipboard()
        {
            var editor = this.Shell.Editor;
            if (editor == null)
            {
                Logger.Debug("LayoutEditor not found. Can't copy layout elements to clipboard.");
                return;
            }

            var elementsToCopy = editor.SelectedElements.ToList();
            if (!elementsToCopy.Any())
            {
                Logger.Trace("No elements selected to copy.");
                return;
            }

            string serializedString;
            var mediaList = new StringCollection();
            using (var streamWriter = new StringWriter())
            {
                using (var writer = XmlWriter.Create(streamWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    var clipboardList = this.CreateClipboardList(elementsToCopy, mediaList);

                    var xmlSerializer = new XmlSerializer(typeof(ClipboardListDataModel));
                    xmlSerializer.Serialize(writer, clipboardList);
                    serializedString = streamWriter.ToString();
                }
            }

            var dataObject = new DataObject();
            dataObject.SetText(serializedString);

            if (mediaList.Count > 0)
            {
                dataObject.SetFileDropList(mediaList);
            }

            Clipboard.SetDataObject(dataObject);
            Clipboard.SetText(serializedString);
            Logger.Trace("{0} layout elements copied to clipboard", elementsToCopy.Count);
        }

        private ClipboardListDataModel CreateClipboardList(
            IEnumerable<LayoutElementDataViewModelBase> elementsToCopy,
            StringCollection mediaList)
        {
            var clipboardList = new ClipboardListDataModel
                                    {
                                        ScreenType = this.Shell.MediaApplicationState.CurrentPhysicalScreen.Type.Value
                                    };
            var configuration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
            foreach (var element in elementsToCopy)
            {
                var dataModel = element.ToDataModel();
                var imageElement = element as ImageElementDataViewModel;
                var videoElement = element as VideoElementDataViewModel;
                var audioFileElement = element as AudioFileElementDataViewModel;
                if (imageElement != null)
                {
                    if (imageElement.Image != null)
                    {
                        var filePath = Path.Combine(
                            configuration.ResourceSettings.LocalResourcePath,
                            Settings.Default.AppDataResourcesRelativePath,
                            imageElement.Image.Hash + ".rx");
                        if (File.Exists(filePath))
                        {
                            mediaList.Add(filePath);
                            clipboardList.Resources.Add(imageElement.Image.ToDataModel());
                        }
                    }
                }
                else if (videoElement != null)
                {
                    var filePath = Path.Combine(
                        configuration.ResourceSettings.LocalResourcePath,
                        Settings.Default.AppDataResourcesRelativePath,
                        videoElement.Hash + ".rx");
                    if (File.Exists(filePath))
                    {
                        mediaList.Add(filePath);
                        var resourceInfoViewModel =
                            this.Shell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                                r => r.Hash == videoElement.Hash);
                        if (resourceInfoViewModel != null)
                        {
                            clipboardList.Resources.Add(resourceInfoViewModel.ToDataModel());
                        }
                    }
                }
                else if (audioFileElement != null && audioFileElement.AudioFile != null)
                {
                    var filePath = Path.Combine(
                        configuration.ResourceSettings.LocalResourcePath,
                        Settings.Default.AppDataResourcesRelativePath,
                        audioFileElement.AudioFile.Hash + ".rx");
                    if (File.Exists(filePath))
                    {
                        mediaList.Add(filePath);
                        clipboardList.Resources.Add(audioFileElement.AudioFile.ToDataModel());
                    }
                }

                if (dataModel != null)
                {
                    clipboardList.Elements.Add(dataModel);
                }
            }

            return clipboardList;
        }

        private void PasteElements()
        {
            var dataObject = Clipboard.GetDataObject();
            if (dataObject == null)
            {
                return;
            }

            try
            {
                var editor = this.Shell.Editor;
                if (editor == null)
                {
                    return;
                }

                if (dataObject.GetDataPresent(DataFormats.Text))
                {
                    var elementsXml = (string)dataObject.GetData(DataFormats.Text);
                    ClipboardListDataModel elementsList;
                    using (var reader = XmlReader.Create(new StringReader(elementsXml)))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(ClipboardListDataModel));
                        elementsList = (ClipboardListDataModel)xmlSerializer.Deserialize(reader);
                    }

                    if (elementsList != null)
                    {
                        if (elementsList.ScreenType != editor.PasteConfiguration.ScreenType)
                        {
                            var message = string.Format(
                                MediaStrings.Paste_WrongScreenTypeMessage,
                                elementsList.ScreenType,
                                editor.PasteConfiguration.ScreenType);
                            MessageBox.Show(message, MediaStrings.Paste_WrongScreenTypeTitle);
                            return;
                        }

                        var viewModelsList = new List<LayoutElementDataViewModelBase>();
                        var displayText = MediaStrings.Paste_DisplayText;

                        if (elementsList.ScreenType == PhysicalScreenType.Audio)
                        {
                           this.LoadAudioElementsFromClipboard(elementsList, editor, viewModelsList);
                        }
                        else
                        {
                            this.SetOffset(elementsList, editor);
                            this.LoadGraphicalLayoutElementsFromClipboard(elementsList, editor, viewModelsList);
                        }

                        var historyEntry = new PasteElementsHistoryEntry(
                            viewModelsList,
                            editor,
                            this.CommandRegistry,
                            this.Shell.MediaApplicationState,
                            displayText);
                        historyEntry.InitialDo();
                        this.ShellController.ChangeHistoryController.AddHistoryEntry(historyEntry, true);
                        return;
                    }
                }

                if (dataObject.GetDataPresent(DataFormats.FileDrop))
                {
                    this.PasteResources(dataObject);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Error while trying to paste elements.", ex);
            }
        }

        private void LoadAudioElementsFromClipboard(
            ClipboardListDataModel elementsList,
            IEditorViewModel editor,
            List<LayoutElementDataViewModelBase> viewModelsList)
        {
            foreach (var elementDataModel in elementsList.Elements)
            {
                var typeName = elementDataModel.GetType().Name.Replace("DataModel", "DataViewModel");
                var assembly = this.GetType().Assembly;
                var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                if (type == null)
                {
                    Logger.Debug("Could not find type '{0}'", typeName);
                    return;
                }

                var elementDataViewModel =
                    (LayoutElementDataViewModelBase)Activator.CreateInstance(type, this.Shell, elementDataModel);
                elementDataViewModel.ResetContainedPredefinedFormulaReferences(
                    this.Shell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations,
                    false);
                var existingElementNames =
                    editor.CurrentAudioOutputElement.Elements.Where(
                        se => se.ElementName.Value.Contains(elementDataViewModel.ElementName.Value))
                        .Select(se => se.ElementName)
                        .ToList();

                elementDataViewModel.ElementName.Value = elementDataViewModel.ElementName.Value.Insert(0, "Copy of ");
                elementDataViewModel.ElementName = this.GetClonedElementName(
                    elementDataViewModel.ElementName,
                    existingElementNames);
                viewModelsList.Add(elementDataViewModel);

                var audioFileDataViewModel = elementDataViewModel as AudioFileElementDataViewModel;
                if (audioFileDataViewModel != null)
                {
                    this.LoadAudioFileElement(elementsList, audioFileDataViewModel);
                }
            }
        }

        private void SetOffset(ClipboardListDataModel elementsList, IEditorViewModel editor)
        {
            var offset = 0;
            var firstElement = elementsList.Elements.OfType<GraphicalElementDataModelBase>().First();
            if (firstElement == null)
            {
                Logger.Debug("Can't set offset because no graphical element found.");
                return;
            }

            // Ensure that the pasted element(s) are at least partially within the current layout
            if (firstElement.X + editor.PasteConfiguration.Offset
                > this.Shell.MediaApplicationState.CurrentVirtualDisplay.Width.Value)
            {
                firstElement.X = this.Shell.MediaApplicationState.CurrentVirtualDisplay.Width.Value
                                 - ((elementsList.Elements.Count + 1) * editor.PasteConfiguration.Offset);
            }

            if (firstElement.Y + editor.PasteConfiguration.Offset
                > this.Shell.MediaApplicationState.CurrentVirtualDisplay.Height.Value)
            {
                firstElement.Y = this.Shell.MediaApplicationState.CurrentVirtualDisplay.Height.Value
                                 - ((elementsList.Elements.Count + 1) * editor.PasteConfiguration.Offset);
            }

            while (this.HasElementAt(firstElement.X, firstElement.Y))
            {
                offset += editor.PasteConfiguration.Offset;
                firstElement.X += editor.PasteConfiguration.Offset;
                firstElement.Y += editor.PasteConfiguration.Offset;
            }

            foreach (var element in elementsList.Elements.OfType<GraphicalElementDataModelBase>())
            {
                if (!object.ReferenceEquals(element, firstElement))
                {
                    element.X += offset;
                    element.Y += offset;
                }
            }
        }

        private void LoadGraphicalLayoutElementsFromClipboard(
           ClipboardListDataModel elementsList,
            IEditorViewModel editor,
            List<LayoutElementDataViewModelBase> viewModelsList)
        {
            foreach (var elementDataModel in elementsList.Elements.OfType<GraphicalElementDataModelBase>())
            {
                var typeName = elementDataModel.GetType().Name.Replace("DataModel", "DataViewModel");
                var assembly = this.GetType().Assembly;
                var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                if (type == null)
                {
                    Logger.Debug("Could not find type '{0}'", typeName);
                    return;
                }

                var frameElement = elementDataModel as FrameElementDataModel;
                if (frameElement != null
                    && editor.Elements.Any(existingElement => existingElement is FrameElementDataViewModel))
                {
                    Logger.Debug("Can't paste a FrameLayoutElement because such an element already exists.");
                    return;
                }

                var elementDataViewModel =
                    (GraphicalElementDataViewModelBase)Activator.CreateInstance(type, this.Shell, elementDataModel);
                elementDataViewModel.ResetContainedPredefinedFormulaReferences(
                    this.Shell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations,
                    false);
                var existingElementNames =
                    editor.Elements.Where(se => se.ElementName.Value.Contains(elementDataViewModel.ElementName.Value))
                        .Select(se => se.ElementName)
                        .ToList();

                elementDataViewModel.ElementName.Value = elementDataViewModel.ElementName.Value.Insert(0, "Copy of ");
                elementDataViewModel.ElementName = this.GetClonedElementName(
                    elementDataViewModel.ElementName,
                    existingElementNames);
                viewModelsList.Add(elementDataViewModel);

                var imageElementDataViewModel = elementDataViewModel as ImageElementDataViewModel;
                if (imageElementDataViewModel != null)
                {
                    this.LoadImageElement(elementsList, imageElementDataViewModel);

                    return;
                }

                var videoElementDataViewModel = elementDataViewModel as VideoElementDataViewModel;
                if (videoElementDataViewModel != null)
                {
                    this.LoadVideoElement(elementsList, videoElementDataViewModel);
                }
            }
        }

        private void LoadVideoElement(
            ClipboardListDataModel elementsList,
            VideoElementDataViewModel videoElementDataViewModel)
        {
            var resourceInfo =
                elementsList.Resources.FirstOrDefault(
                    r => Path.GetFileName(r.Filename) == videoElementDataViewModel.VideoUri.Value);
            if (resourceInfo != null)
            {
                var resourceInfoDataViewModel = new ResourceInfoDataViewModel(resourceInfo);
                var existingResource =
                    this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                        r =>
                            r.Filename == resourceInfoDataViewModel.Filename
                            && r.Hash == resourceInfoDataViewModel.Hash);
                if (existingResource == null)
                {
                    resourceInfoDataViewModel.ReferencesCount = 0;
                    this.Shell.MediaApplicationState.CurrentProject.Resources.Add(resourceInfoDataViewModel);
                }
            }
        }

        private void LoadImageElement(
            ClipboardListDataModel elementsList,
            ImageElementDataViewModel imageElementDataViewModel)
        {
            var imageResourceInfo =
                elementsList.Resources.FirstOrDefault(
                    r => Path.GetFileName(r.Filename) == imageElementDataViewModel.Filename.Value);
            if (imageResourceInfo != null)
            {
                var resourceInfoDataViewModel = new ResourceInfoDataViewModel(imageResourceInfo);
                var existingResource =
                    this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                        r =>
                            r.Filename == resourceInfoDataViewModel.Filename
                            && r.Hash == resourceInfoDataViewModel.Hash);
                if (existingResource == null)
                {
                    resourceInfoDataViewModel.Dimension = resourceInfoDataViewModel.Dimension.Replace(
                        "?",
                        string.Empty);
                    resourceInfoDataViewModel.ReferencesCount = 0;
                    this.Shell.MediaApplicationState.CurrentProject.Resources.Add(resourceInfoDataViewModel);
                }
            }
        }

        private void LoadAudioFileElement(
            ClipboardListDataModel elementsList,
            AudioFileElementDataViewModel audioFileDataViewModel)
        {
            var resourceInfo =
                elementsList.Resources.FirstOrDefault(
                    r => Path.GetFileName(r.Filename) == audioFileDataViewModel.Filename.Value);
            if (resourceInfo == null)
            {
                return;
            }

            var resourceInfoDataViewModel = new ResourceInfoDataViewModel(resourceInfo);
            var existingResource =
                this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                    r => r.Filename == resourceInfoDataViewModel.Filename && r.Hash == resourceInfoDataViewModel.Hash);
            if (existingResource == null)
            {
                resourceInfoDataViewModel.ReferencesCount = 0;
                this.Shell.MediaApplicationState.CurrentProject.Resources.Add(resourceInfoDataViewModel);
            }
        }

        private void CutElements()
        {
            this.CopySelectedElementsToClipboard();
            this.DeleteSelectedElements();
        }

        private DataValue<string> GetClonedElementName(
            DataValue<string> elementName,
            IEnumerable<DataValue<string>> existingElementNames,
            int number = 0)
        {
            var existingList = existingElementNames.ToList();
            if (number > 0)
            {
                var oldValueString = " (" + (number - 1) + ")";
                var newValueString = " (" + number + ")";
                elementName.Value = elementName.Value.EndsWith(oldValueString)
                    ? elementName.Value.Replace(oldValueString, newValueString)
                    : string.Format("{0} ({1})", elementName.Value, number);
            }

            if (existingList.Any(name => name.Value == elementName.Value))
            {
                elementName = this.GetClonedElementName(elementName, existingList, ++number);
            }

            return elementName;
        }

        private void PasteResources(IDataObject dataObject)
        {
            var files = (string[])dataObject.GetData(DataFormats.FileDrop);

            if (files.Length <= 0)
            {
                return;
            }

            var messageResult = MessageBox.Show(
                MediaStrings.Paste_ConfirmAddResourceMessage,
                MediaStrings.Paste_ConfirmAddResourceTitle,
                MessageBoxButton.OKCancel);
            if (messageResult == MessageBoxResult.Cancel)
            {
                return;
            }

            foreach (var filename in files)
            {
                var file = FileSystemManager.Local.GetFile(filename);
                var extension = Path.GetExtension(filename);
                ResourceType resourceType;

                if (Settings.Default.ImageExtensions_Tft.Contains(extension)
                    || Settings.Default.ImageExtensions_Led.Contains(extension))
                {
                    resourceType = ResourceType.Image;
                }
                else if (Settings.Default.VideoExtensions.Contains(extension))
                {
                    resourceType = ResourceType.Video;
                }
                else
                {
                    Logger.Debug("Resource of type {0} not supported.", extension);
                    continue;
                }

                var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
                resourceManager.CleanupResources();
                if (!resourceManager.CheckUsedDiskSpace(file.Size) || !resourceManager.CheckAvailableDiskSpace(file))
                {
                    resourceManager.CleanupResources(true);
                    if (!resourceManager.CheckUsedDiskSpace(file.Size)
                        || !resourceManager.CheckAvailableDiskSpace(file))
                    {
                        var resourceRootPath = Path.GetPathRoot(resourceManager.GetResourcePath(string.Empty));
                        var message = string.Format(
                            MediaStrings.AddResources_NotEnoughSpace_Message,
                            resourceRootPath,
                            filename);
                        MessageBox.Show(
                            message,
                            MediaStrings.AddResources_NotEnoughSpace_Title,
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }
                }

                string hash;
                using (var stream = file.OpenRead())
                {
                    hash = ResourceHash.Create(stream);
                }

                var parameters = new AddResourceParameters { Resources = new[] { file }, Type = resourceType };
                this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.AddResource).Execute(parameters);
                var newResource =
                    this.Shell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(r => r.Hash == hash);
                IResource thumbnailResource;
                this.ShellController.ResourceController.EnsurePreview(newResource, out thumbnailResource);
            }
        }

        private bool HasElementAt(int x, int y)
        {
            var alreadyHasElementAtPos = false;

            foreach (var otherElement in this.Shell.Editor.Elements)
            {
                if (otherElement.X.Value == x && otherElement.Y.Value == y)
                {
                    alreadyHasElementAtPos = true;
                }
            }

            return alreadyHasElementAtPos;
        }

        private void ShowMediaSelector(TextualReplacementDataViewModel parameter)
        {
            if (parameter != null)
            {
                this.ShowEditTextualReplacementImagePopup(parameter);
            }
        }

        private void ShowEditTextualReplacementImagePopup(TextualReplacementDataViewModel replacement)
        {
            Logger.Debug("Request to show the textreplacement popup.");

            var prompt = new MenuSelectMediaPrompt(
                replacement,
                this.Shell,
                this.CommandRegistry,
                this.RecentMediaResources)
                         {
                             PlacementTarget =
                                 new CustomPlacementTarget
                                 {
                                     UseMousePosition =
                                         new DataValue<bool>(true),
                                 }
                         };

            InteractionManager<MenuSelectMediaPrompt>.Current.Raise(prompt);
        }

        private void SelectAllElements(object parameter)
        {
            var editor = this.Shell.Editor;
            if (editor != null)
            {
                foreach (var layoutElementDataViewModelBase in editor.Elements)
                {
                    if (!editor.SelectedElements.Contains(layoutElementDataViewModelBase))
                    {
                        editor.SelectedElements.Add(layoutElementDataViewModelBase);
                    }
                }
            }
        }

        private Vector EdgeSnap(Vector delta, IList<GraphicalElementDataViewModelBase> selectedElements)
        {
            var result = new Vector(delta.X, delta.Y);

            if (selectedElements.Any(e => e.EdgeSnapDataList.Count(esd => esd.IsHorizontal) > 0))
            {
                result.X = this.HandleSnapped((int)delta.X, selectedElements, true);
            }
            else
            {
                result.X = this.HandleUnsnapped((int)delta.X, selectedElements, true);
            }

            if (selectedElements.Any(e => e.EdgeSnapDataList.Count(esd => !esd.IsHorizontal) > 0))
            {
                result.Y = this.HandleSnapped((int)delta.Y, selectedElements, false);
            }
            else
            {
                result.Y = this.HandleUnsnapped((int)delta.Y, selectedElements, false);
            }

            return result;
        }

        private int HandleSnapped(
            int delta,
            IEnumerable<GraphicalElementDataViewModelBase> selectedElements,
            bool isHorizontal)
        {
            var result = 0;

            var handledEdgeSnapData = new List<EdgeSnapData>();

            foreach (var element in selectedElements)
            {
                foreach (var edgeSnapData
                    in element.EdgeSnapDataList.Where(esd => esd.IsHorizontal == isHorizontal).ToList())
                {
                    if (handledEdgeSnapData.Contains(edgeSnapData))
                    {
                        element.EdgeSnapDataList.Remove(edgeSnapData);
                        continue;
                    }

                    edgeSnapData.Delta += delta;

                    edgeSnapData.Distance = Math.Abs(edgeSnapData.Delta);

                    if (edgeSnapData.Distance > Settings.Default.EdgeSnapTolerance)
                    {
                        result += edgeSnapData.Delta;

                        element.EdgeSnapDataList.Remove(edgeSnapData);
                    }

                    handledEdgeSnapData.Add(edgeSnapData);
                }
            }

            return result;
        }

        private int HandleUnsnapped(
            int delta,
            IList<GraphicalElementDataViewModelBase> selectedElements,
            bool isHorizontal)
        {
            var result = delta;

            var otherElements = this.Shell.Editor.Elements.Where(e => !selectedElements.Contains(e)).ToList();

            var edgeSnapDataList = new List<EdgeSnapData>();
            foreach (var element in selectedElements)
            {
                if (isHorizontal)
                {
                    var minX = element.X.Value + delta;
                    edgeSnapDataList.Add(this.GetEdgeSnapData(otherElements, element, minX, true));

                    var maxX = element.X.Value + element.Width.Value + delta;
                    edgeSnapDataList.Add(this.GetEdgeSnapData(otherElements, element, maxX, true));
                }
                else
                {
                    var minY = element.Y.Value + delta;
                    edgeSnapDataList.Add(this.GetEdgeSnapData(otherElements, element, minY, false));

                    var maxY = element.Y.Value + element.Height.Value + delta;
                    edgeSnapDataList.Add(this.GetEdgeSnapData(otherElements, element, maxY, false));
                }
            }

            edgeSnapDataList = edgeSnapDataList.Where(e => e != null).ToList();

            var minEdge =
                edgeSnapDataList.Where(e => e.IsHorizontal == isHorizontal).OrderBy(e => e.Distance).FirstOrDefault();

            if (minEdge != null)
            {
                result += minEdge.Delta;
                minEdge.Delta = -minEdge.Delta;
                minEdge.Distance = Math.Abs(minEdge.Delta);
                selectedElements.ForEach(e => e.EdgeSnapDataList.Add(minEdge));
            }

            return result;
        }

        private EdgeSnapData GetEdgeSnapData(
            IEnumerable<GraphicalElementDataViewModelBase> elements,
            GraphicalElementDataViewModelBase element,
            int comparer,
            bool isHorizontal)
        {
            EdgeSnapData result = null;

            var tolerance = Settings.Default.EdgeSnapTolerance;

            var minDistance = int.MaxValue;
            foreach (var otherElement in elements)
            {
                int value1;
                int value2;
                if (isHorizontal)
                {
                    value1 = otherElement.X.Value;
                    value2 = otherElement.X.Value + otherElement.Width.Value;
                }
                else
                {
                    value1 = otherElement.Y.Value;
                    value2 = otherElement.Y.Value + otherElement.Height.Value;
                }

                var distance1 = this.GetDistance(value1, comparer);
                var distance2 = this.GetDistance(value2, comparer);
                if (distance1 < distance2)
                {
                    if (distance1 < minDistance && distance1 < tolerance)
                    {
                        result = result ?? new EdgeSnapData(isHorizontal);

                        minDistance = distance1;
                        result.Distance = distance1;
                        result.Delta = value1 - comparer;
                        result.TargetElement = element;
                    }
                }
                else
                {
                    if (distance2 < minDistance && distance2 < tolerance)
                    {
                        result = result ?? new EdgeSnapData(isHorizontal);

                        minDistance = distance2;
                        result.Distance = distance2;
                        result.Delta = value2 - comparer;
                        result.TargetElement = element;
                    }
                }
            }

            return result;
        }

        private int GetDistance(int a, int b)
        {
            return Math.Abs(a - b);
        }
    }
}