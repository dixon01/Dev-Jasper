// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalEditorControllerBase.cs" company="Gorba AG">
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

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The graphical editor controller base.
    /// </summary>
    public abstract class GraphicalEditorControllerBase : EditorControllerBase
    {
        private readonly Lazy<IResourceManager> lazyResourceManager =
            new Lazy<IResourceManager>(() => ServiceLocator.Current.GetInstance<IResourceManager>());

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicalEditorControllerBase"/> class.
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
        protected GraphicalEditorControllerBase(
            IMediaShellController shellController,
            IMediaShell shell,
            ICommandRegistry commandRegistry)
            : base(shellController, shell, commandRegistry)
        {
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
        /// The get default font.
        /// </summary>
        /// <returns>
        /// The <see cref="FontDataViewModel"/>.
        /// </returns>
        protected virtual FontDataViewModel GetDefaultFont()
        {
            throw new NotImplementedException();
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
        /// The <see cref="GraphicalElementDataViewModelBase"/>.
        /// </returns>
        protected virtual StaticTextElementDataViewModel CreateStaticTextElement(
            CreateElementParameters createParameters, out string displayText, PhysicalScreenType screenType)
        {
            var staticText = new StaticTextElementDataViewModel(this.Shell);
            var progressiveNumber = this.FillCommonProperties(staticText, createParameters, screenType);
            staticText.Value.Value = string.Format("StaticText{0}", progressiveNumber);
            staticText.Font = this.GetDefaultFont();
            this.ResourceManager.TextElementManager.SetReferences(staticText);
            displayText = MediaStrings.GraphicalEditorController_CreateStaticElementHistoryEntryLabel;
            return staticText;
        }

        /// <summary>
        /// The fill common properties.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="elementParameters">
        /// The element parameters.
        /// </param>
        /// <param name="screenType">
        /// The screen type for which the properties are filled.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected override int FillCommonProperties(
            LayoutElementDataViewModelBase element,
            CreateElementParameters elementParameters,
            PhysicalScreenType screenType)
        {
            var progressiveNumber = base.FillCommonProperties(element, elementParameters, screenType);

            var editor = this.Shell.Editors[screenType];
            var drawableElement = element as DrawableElementDataViewModelBase;
            if (drawableElement != null)
            {
                drawableElement.ZIndex.Value = editor.Elements.Count;
            }

            var graphicalElement = element as GraphicalElementDataViewModelBase;
            if (graphicalElement != null)
            {
                graphicalElement.X.Value = (int)elementParameters.Bounds.Left;
                graphicalElement.Y.Value = (int)elementParameters.Bounds.Top;
                graphicalElement.Width.Value = (int)elementParameters.Bounds.Width;
                graphicalElement.Height.Value = (int)elementParameters.Bounds.Height;
            }

            return progressiveNumber;
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
        /// The screen Type.
        /// </param>
        /// <returns>
        /// The <see cref="DynamicTextElementDataViewModel"/>.
        /// </returns>
        protected virtual DynamicTextElementDataViewModel CreateDynamicTextElement(
            CreateElementParameters createParameters,
            out string displayText,
            PhysicalScreenType screenType)
        {
            var dynamicText = new DynamicTextElementDataViewModel(this.Shell);
            int progressiveNumber = this.FillCommonProperties(dynamicText, createParameters, screenType);
            var name = string.Format("DynamicText{0}", progressiveNumber);
            this.FillDynamicTextProperties(dynamicText, name);

            displayText = MediaStrings.GraphicalEditorController_CreateDynamicElementHistoryEntryLabel;
            this.ResourceManager.TextElementManager.SetReferences(dynamicText);
            return dynamicText;
        }

        /// <summary>
        /// The show edit dynamic text popup.
        /// </summary>
        /// <param name="dynamicTextElement">
        /// The dynamic text element.
        /// </param>
        protected void ShowEditDynamicTextPopup(DynamicTextElementDataViewModel dynamicTextElement)
        {
            Logger.Debug("Request to show the EditDynamic Text Popup.");

            Action<EditDynamicTextPrompt> action = prompt =>
            {
                var generic = new GenericEvalDataViewModel(this.Shell);
                if (prompt.DynamicTextElement.SelectedDictionaryValue.Column != null)
                {
                    generic.Column.Value = prompt.DynamicTextElement.SelectedDictionaryValue.Column.Index;
                }

                if (prompt.DynamicTextElement.SelectedDictionaryValue.Table != null)
                {
                    generic.Table.Value = prompt.DynamicTextElement.SelectedDictionaryValue.Table.Index;
                    if (!prompt.DynamicTextElement.SelectedDictionaryValue.Table.MultiRow)
                    {
                        prompt.DynamicTextElement.SelectedDictionaryValue.Row = 0;
                    }
                }

                if (prompt.DynamicTextElement.SelectedDictionaryValue.Language != null)
                {
                    generic.Language.Value = prompt.DynamicTextElement.SelectedDictionaryValue.Language.Index;
                }

                generic.Row.Value = prompt.DynamicTextElement.SelectedDictionaryValue.Row;
                dynamicTextElement.Value.Formula = generic;

                InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
            };

            var dictionaryPrompt = new EditDynamicTextPrompt(
                dynamicTextElement, this.Shell, this.RecentDictionaryValues)
            {
                IsOpen = true,
            };
            InteractionManager<EditDynamicTextPrompt>.Current.Raise(dictionaryPrompt, action);
        }

        /// <summary>
        /// The show edit image popup.
        /// </summary>
        /// <param name="imageElement">
        /// The image element.
        /// </param>
        protected void ShowEditImagePopup(ImageElementDataViewModel imageElement)
        {
            Logger.Debug("Request to show the Edit image popup.");
            var prompt = new SelectMediaPrompt(
                imageElement, this.Shell, this.CommandRegistry, this.RecentMediaResources);
            InteractionManager<SelectMediaPrompt>.Current.Raise(prompt);
        }

        private void FillDynamicTextProperties(DynamicTextElementDataViewModel dynamicText, string name)
        {
            dynamicText.TestData.Value = name;
            dynamicText.SelectedDictionaryValue.Row = 0;
            dynamicText.SelectedDictionaryValue.Language = this.Shell.Dictionary.Languages.FirstOrDefault();
            dynamicText.Value = new AnimatedDynamicDataValue<string>(name);
            dynamicText.Font = this.GetDefaultFont();
        }
    }
}