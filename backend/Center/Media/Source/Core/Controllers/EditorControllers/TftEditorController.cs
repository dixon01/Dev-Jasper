// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftEditorController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILayoutEditorController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.EditorControllers
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// The graphical editor controller base.
    /// </summary>
    public class TftEditorController : GraphicalEditorControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TftEditorController"/> class.
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
        public TftEditorController(
            IMediaShellController shellController,
            IMediaShell shell,
            ICommandRegistry commandRegistry)
            : base(shellController, shell, commandRegistry)
        {
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.Tft.CreateElement,
                new RelayCommand<CreateElementParameters>(this.CreateElement));

            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Layout.Tft.ShowLayoutEditPopup,
               new RelayCommand<LayoutElementDataViewModelBase>(this.ShowEditPopup));
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

            if (parameter is DynamicTextElementDataViewModel)
            {
                this.ShowEditDynamicTextPopup(parameter as DynamicTextElementDataViewModel);
                return;
            }

            if (parameter is ImageElementDataViewModel)
            {
                this.ShowEditImagePopup(parameter as ImageElementDataViewModel);
                return;
            }

            if (parameter is VideoElementDataViewModel)
            {
                this.ShowEditVideoPopup(parameter as VideoElementDataViewModel);
                return;
            }

            if (parameter is ImageListElementDataViewModel)
            {
                this.ShowEditImageListPopup(parameter as ImageListElementDataViewModel);
                return;
            }

            Logger.Debug("Show edit popup not handled");
        }

        /// <summary>
        /// The get default font.
        /// </summary>
        /// <returns>
        /// The <see cref="FontDataViewModel"/>.
        /// </returns>
        protected override FontDataViewModel GetDefaultFont()
        {
            return new FontDataViewModel(null);
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
            var editor = this.Shell.Editors[PhysicalScreenType.TFT];
            var existingElements = editor.Elements.Where(e => e.GetType().Name.Contains(elementTypeName));

            return this.CreateNextProgressiveNumber(existingElements);
        }

        private void ShowEditVideoPopup(VideoElementDataViewModel videoElement)
        {
            Logger.Debug("Request to show the Edit video popup.");
            var prompt = new SelectMediaPrompt(
                videoElement, this.Shell, this.CommandRegistry, this.RecentMediaResources);
            InteractionManager<SelectMediaPrompt>.Current.Raise(prompt);
        }

        private async void CreateElement(CreateElementParameters createParams)
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

            LayoutElementDataViewModelBase element = null;
            var displayText = string.Empty;
            switch (createParams.Type)
            {
                case LayoutElementType.StaticText:
                    element = this.CreateStaticTextElement(createParams, out displayText, PhysicalScreenType.TFT);
                    break;
                case LayoutElementType.DynamicText:
                    element = this.CreateDynamicTextElement(createParams, out displayText, PhysicalScreenType.TFT);
                    this.ShowEditDynamicTextPopup((DynamicTextElementDataViewModel)element);
                    break;
                case LayoutElementType.Image:
                    element = new ImageElementDataViewModel(this.Shell);
                    this.FillCommonProperties(element, createParams, PhysicalScreenType.TFT);
                    displayText = MediaStrings.GraphicalEditorController_CreateImageElementHistoryEntryLabel;
                    break;
                case LayoutElementType.Video:
                    element = new VideoElementDataViewModel(this.Shell);
                    this.FillCommonProperties(element, createParams, PhysicalScreenType.TFT);
                    displayText = MediaStrings.TftEditorController_CreateVideoElementHistoryEntryLabel;
                    break;
                case LayoutElementType.Frame:
                    element = this.CreateFrameElement(createParams, out displayText);
                    break;
                case LayoutElementType.AnalogClock:
                    element = await this.CreateAnalogClockElement(createParams);
                    displayText = MediaStrings.TftEditorController_CreateAnalogClockElementHistoryEntryLabel;
                    break;
                case LayoutElementType.ImageList:
                    element = this.CreateImageListElement(createParams, out displayText);
                    break;
                case LayoutElementType.RssTicker:
                    element = this.CreateRssTickerElement(createParams, out displayText);
                    break;
                case LayoutElementType.LiveStream:
                    element = this.CreateLiveStreamElement(createParams);
                    this.FillCommonProperties(element, createParams, PhysicalScreenType.TFT);
                    displayText = MediaStrings.TftEditorController_CreateVideoElementHistoryEntryLabel;
                    break;
                case LayoutElementType.Template:
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

        private LiveStreamElementDataViewModel CreateLiveStreamElement(CreateElementParameters createParams)
        {
            var liveStream = new LiveStreamElementDataViewModel(this.Shell);
            this.FillCommonProperties(liveStream, createParams, PhysicalScreenType.TFT);
            return liveStream;
        }

        private RssTickerElementDataViewModel CreateRssTickerElement(
            CreateElementParameters createParams,
            out string displayText)
        {
            var rssTicker = new RssTickerElementDataViewModel(this.Shell);
            this.FillCommonProperties(rssTicker, createParams, PhysicalScreenType.TFT);

            displayText = MediaStrings.TftEditorController_CreateRssTickerElementHistoryEntryLabel;
            rssTicker.Validity.Value = TimeSpan.FromMinutes(1439);
            rssTicker.Delimiter.Value = "-";
            rssTicker.UpdateInterval.Value = TimeSpan.FromMinutes(5);
            rssTicker.TestData.Value = rssTicker.ElementName.Value;
            rssTicker.ExportingRow =
                this.Shell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.Sum(
                    layout =>
                    layout.Resolutions.Sum(
                        resolution => resolution.Elements.OfType<RssTickerElementDataViewModel>().Count()));
            return rssTicker;
        }

        private async Task<AnalogClockElementDataViewModel> CreateAnalogClockElement(
            CreateElementParameters createParams)
        {
            // create before clock so references can be set
            await this.CreateClockHandResourceAsync("HourHand.png");
            await this.CreateClockHandResourceAsync("MinuteHand.png");
            await this.CreateClockHandResourceAsync("SecondsHand.png");

            var analogClock = new AnalogClockElementDataViewModel(this.Shell);
            this.FillCommonProperties(analogClock, createParams, PhysicalScreenType.TFT);

            return analogClock;
        }

        private ImageListElementDataViewModel CreateImageListElement(
            CreateElementParameters createParams,
            out string displayText)
        {
            var imageList = new ImageListElementDataViewModel(this.Shell);
            this.FillCommonProperties(imageList, createParams, PhysicalScreenType.TFT);
            imageList.FilePatterns.Value = @"Symbols\{0}.jpg;Symbols\{0}.jpeg;Symbols\{0}.png";
            imageList.HorizontalImageGap.Value = 10;
            imageList.VerticalImageGap.Value = 10;
            displayText = MediaStrings.TftEditorController_CreateImageListElementHistoryEntryLabel;
            return imageList;
        }

        private async Task CreateClockHandResourceAsync(string filename)
        {
            using (
            var stream =
                Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Gorba.Center.Media.Core.Resources.Images" + @"." + filename))
            {
                if (stream == null)
                {
                    return;
                }

                var originalImage = Image.FromStream(stream, true, true);
                var tempPath = Path.GetTempPath();
                var tempFileName = Path.Combine(tempPath, filename);
                if (!File.Exists(tempFileName))
                {
                    originalImage.Save(tempFileName);
                }

                var file = FileSystemManager.Local.GetFile(tempFileName);
                using (var fileStream = file.OpenRead())
                {
                    var hash = ResourceHash.Create(fileStream);
                    var existingResource =
                        this.Shell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                            r => r.Hash == hash);
                    if (existingResource != null)
                    {
                        return;
                    }
                }

                var parameters = new AddResourceParameters { Resources = new[] { file }, Type = ResourceType.Image };
                this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.AddResource).Execute(parameters);
                await parameters.Completed.Task;
            }
        }

        private GraphicalElementDataViewModelBase CreateFrameElement(
             CreateElementParameters createParams, out string displayText)
        {
            GraphicalElementDataViewModelBase element = null;
            if (!this.Shell.Editor.Elements.Any(elem => elem is FrameElementDataViewModel))
            {
                element = new FrameElementDataViewModel(this.Shell);
                this.FillCommonProperties(element, createParams, PhysicalScreenType.TFT);
                displayText = MediaStrings.TftEditorController_CreateFrameElementHistoryEntryLabel;
            }
            else
            {
                displayText = string.Empty;
                MessageBox.Show(MediaStrings.Shell_Information_OnlyOneFrameAllowed);
            }

            return element;
        }

        private void ShowEditImageListPopup(ImageListElementDataViewModel imageListElementDataViewModel)
        {
            Logger.Debug("Request to show the DictionarySelector Popup for an ImageList.");
            var dataValue = imageListElementDataViewModel.Values;
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
                imageListElementDataViewModel.Values.Formula = generic;
                InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
            };

            var dictionaryPrompt = new ImageListDictionaryPrompt(evaluation, this.Shell, this.RecentDictionaryValues)
            {
                IsOpen = true,
                PlacementTarget = imageListElementDataViewModel,
            };
            InteractionManager<ImageListDictionaryPrompt>.Current.Raise(dictionaryPrompt, action);
        }
    }
}