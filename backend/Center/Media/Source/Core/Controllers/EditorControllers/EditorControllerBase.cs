// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditorControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILayoutEditorController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.EditorControllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Resources;
    using System.Text.RegularExpressions;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The editor controller base.
    /// </summary>
    public abstract class EditorControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues;

        private static ExtendedObservableCollection<ResourceInfoDataViewModel> recentMediaResources;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorControllerBase"/> class.
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
        protected EditorControllerBase(
            IMediaShellController shellController,
            IMediaShell shell,
            ICommandRegistry commandRegistry)
        {
            this.ShellController = shellController;
            this.CommandRegistry = commandRegistry;
            this.Shell = shell;

            if (this.RecentDictionaryValues == null)
            {
                this.RecentDictionaryValues = new ExtendedObservableCollection<DictionaryValueDataViewModel>();
            }

            if (this.RecentMediaResources == null)
            {
                this.RecentMediaResources = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
            }
        }

        /// <summary>
        /// Gets the recent dictionary values, shared by all editors.
        /// </summary>
        protected ExtendedObservableCollection<DictionaryValueDataViewModel> RecentDictionaryValues
        {
            get
            {
                return recentDictionaryValues;
            }

            private set
            {
                recentDictionaryValues = value;
            }
        }

        /// <summary>
        /// Gets the recent media resources type, shared by all editors.
        /// </summary>
        protected ExtendedObservableCollection<ResourceInfoDataViewModel> RecentMediaResources
        {
            get
            {
                return recentMediaResources;
            }

            private set
            {
                recentMediaResources = value;
            }
        }

        /// <summary>
        /// Gets the ViewModel of the shell.
        /// </summary>
        protected IMediaShell Shell { get; private set; }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        protected ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets the shell controller.
        /// </summary>
        protected IMediaShellController ShellController { get; private set; }

        /// <summary>
        /// Initializes properties that can't be set during composition.
        /// </summary>
        public void Initialize()
        {
            var state =
                ServiceLocator.Current.GetInstance<IApplicationState>() as MediaApplicationState;
            if (state != null && state.RecentDictionaryValues != null)
            {
                this.RecentDictionaryValues = state.RecentDictionaryValues;
            }

            if (state == null || state.RecentMediaResources == null)
            {
                return;
            }

            this.RecentMediaResources = state.CurrentProjectRecentMediaResources;
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
        /// The screen type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected virtual int FillCommonProperties(
            LayoutElementDataViewModelBase element,
            CreateElementParameters elementParameters,
            PhysicalScreenType screenType)
        {
            var progressiveNumber = this.GetProgressiveNumber(elementParameters.Type.ToString());
            var name = MediaStrings.ResourceManager.GetString("ElementName_" + elementParameters.Type);
            element.ElementName = new DataValue<string>(string.Format(
                                        "{0}{1}",
                                        name,
                                        progressiveNumber));
            return progressiveNumber;
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
        protected virtual int GetProgressiveNumber(
            string elementTypeName)
        {
            return 0;
        }

        /// <summary>
        /// The create next progressive number.
        /// </summary>
        /// <param name="existingElements">
        /// The existing elements.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected int CreateNextProgressiveNumber(
            IEnumerable<LayoutElementDataViewModelBase> existingElements)
        {
            var ids = (from existingElement in existingElements
                       select Regex.Match(existingElement.ElementName.Value, @"\d+").Value
                           into idString
                           where !string.IsNullOrEmpty(idString)
                           select int.Parse(idString)).ToList();
            return ids.Count == 0 ? 1 : ids.Max() + 1;
        }

        /// <summary>
        /// The show edit popup.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        protected virtual void ShowEditPopup(LayoutElementDataViewModelBase parameter)
        {
            Logger.Debug("Show edit popup not handled");
        }

        /// <summary>
        /// The update media reference.
        /// </summary>
        /// <param name="currentFilename">
        /// The current filename.
        /// </param>
        /// <param name="previousFilename">
        /// The previous filename.
        /// </param>
        protected void UpdateMediaReference(string currentFilename, string previousFilename)
        {
            var previousHash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(previousFilename);
            var currentHash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(currentFilename);

            var selectionParameters = new SelectResourceParameters
                                      {
                                          CurrentSelectedResourceHash = currentHash,
                                          PreviousSelectedResourceHash = previousHash
                                      };

            this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                .Execute(selectionParameters);
        }

        /// <summary>
        /// The update media reference.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="previous">
        /// The previous.
        /// </param>
        protected void UpdateMediaReference(ResourceInfoDataViewModel current, ResourceInfoDataViewModel previous)
        {
            var selectionParameters = new SelectResourceParameters();

            if (current != null)
            {
                selectionParameters.CurrentSelectedResourceHash = current.Hash;
            }

            if (previous != null)
            {
                selectionParameters.PreviousSelectedResourceHash = previous.Hash;
            }

            this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource).Execute(selectionParameters);
        }
    }
}