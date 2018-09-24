// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedEditorController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILayoutEditorController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.EditorControllers
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The graphical editor controller base.
    /// </summary>
    public class LedEditorController : GraphicalEditorControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LedEditorController"/> class.
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
        public LedEditorController(
            IMediaShellController shellController,
            IMediaShell shell,
            ICommandRegistry commandRegistry)
            : base(shellController, shell, commandRegistry)
        {
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.Led.CreateElement,
                new RelayCommand<CreateElementParameters>(this.CreateElement));

            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Layout.Led.ShowLayoutEditPopup,
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
            return new FontDataViewModel(this.Shell)
            {
                Color = new DataValue<string>(Settings.Default.FontColor),
                Face = new DataValue<string>(
                    this.Shell.MediaApplicationState.CurrentProject.AvailableLedFonts.FirstOrDefault()),
            };
        }

        /// <summary>
        /// The create static text element.
        /// </summary>
        /// <param name="createParameters">
        /// The create parameters.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <param name="screenType">
        /// The screen type.
        /// </param>
        /// <returns>
        /// The <see cref="StaticTextElementDataViewModel"/>.
        /// </returns>
        protected override StaticTextElementDataViewModel CreateStaticTextElement(
            CreateElementParameters createParameters,
            out string displayText,
            PhysicalScreenType screenType)
        {
            var staticText = base.CreateStaticTextElement(createParameters, out displayText, screenType);
            staticText.Overflow.Value = TextOverflow.Extend;

            return staticText;
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
            var editor = this.Shell.Editors[PhysicalScreenType.LED];
            var existingElements = editor.Elements.Where(e => e.GetType().Name.Contains(elementTypeName));

            return this.CreateNextProgressiveNumber(existingElements);
        }

        /// <summary>
        /// The create dynamic text element.
        /// </summary>
        /// <param name="createParameters">
        /// The create parameters.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <param name="screenType">
        /// The screen type.
        /// </param>
        /// <returns>
        /// The <see cref="DynamicTextElementDataViewModel"/>.
        /// </returns>
        protected override DynamicTextElementDataViewModel CreateDynamicTextElement(
            CreateElementParameters createParameters,
            out string displayText,
            PhysicalScreenType screenType)
        {
            var dynamicText = base.CreateDynamicTextElement(createParameters, out displayText, PhysicalScreenType.LED);
            dynamicText.Overflow.Value = TextOverflow.Extend;

            return dynamicText;
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
                case LayoutElementType.StaticText:
                    element = this.CreateStaticTextElement(createParams, out displayText, PhysicalScreenType.LED);
                    break;
                case LayoutElementType.DynamicText:
                    element = this.CreateDynamicTextElement(createParams, out displayText, PhysicalScreenType.LED);
                    this.ShowEditDynamicTextPopup((DynamicTextElementDataViewModel)element);
                    break;
                case LayoutElementType.Image:
                    element = new ImageElementDataViewModel(this.Shell);
                    this.FillCommonProperties(element, createParams, PhysicalScreenType.LED);
                    displayText = MediaStrings.GraphicalEditorController_CreateImageElementHistoryEntryLabel;
                    break;
                case LayoutElementType.Rectangle:
                    element = this.CreateRectangleElement(createParams, out displayText);
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

        private RectangleElementDataViewModel CreateRectangleElement(
            CreateElementParameters createParams,
            out string displayText)
        {
            var element = new RectangleElementDataViewModel(this.Shell) { Color = { Value = "Red" } };
            this.FillCommonProperties(element, createParams, PhysicalScreenType.LED);
            displayText = MediaStrings.LedEditorController_CreateRectangleElementHistoryEntryLabel;
            return element;
        }
    }
}