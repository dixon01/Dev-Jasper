// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FormulaController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Formulas;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The FormulaController.
    /// </summary>
    public class FormulaController : IFormulaController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediaShellController shellController;

        private readonly Lazy<IMediaApplicationState> lazyApplicationState =
            new Lazy<IMediaApplicationState>(GetApplicationState);

        private readonly FormulaModelGenerator formulaModelGenerator;

        private readonly Parser parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaController"/> class.
        /// </summary>
        /// <param name="shellController">the shell Controller</param>
        /// <param name="mediaShell">The media shell.</param>
        /// <param name="commandRegistry">The command registry.</param>
        public FormulaController(
            IMediaShellController shellController, IMediaShell mediaShell, ICommandRegistry commandRegistry)
        {
            this.shellController = shellController;
            this.MediaShell = mediaShell;
            this.CommandRegistry = commandRegistry;
            this.formulaModelGenerator = new FormulaModelGenerator(mediaShell);
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Layout.RemoveFormula,
               new RelayCommand<ContextMenu>(this.RemoveFormula, CanRemoveFormula));
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Layout.RemoveAnimation,
               new RelayCommand<ContextMenu>(this.RemoveAnimation, CanRemoveAnimation));
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Cycle.RemoveFormula,
               new RelayCommand<ContextMenu>(this.RemoveCycleFormula, CanRemoveFormula));
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Cycle.RemoveAnimation,
               new RelayCommand<ContextMenu>(this.RemoveCycleAnimation, CanRemoveAnimation));
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Section.RemoveFormula,
               new RelayCommand<ContextMenu>(this.RemoveSectionFormula, CanRemoveFormula));
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Section.RemoveAnimation,
               new RelayCommand<ContextMenu>(this.RemoveSectionAnimation, CanRemoveAnimation));
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.UI.ShowDictionarySelector,
               new RelayCommand(this.ShowDictionarySelector));
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.UI.ShowTriggerEditorDictionarySelector,
               new RelayCommand<GenericEvalDataViewModel>(this.ShowTriggerEditorDictionarySelector));
            this.CommandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.PhysicalScreen.RemoveFormula,
               new RelayCommand<ContextMenu>(this.RemovePhysicalScreenFormula, CanRemoveFormula));
            var grammar = new FormulaGrammar();
            var language = new LanguageData(grammar);
            this.parser = new Parser(language);
        }

        /// <summary>
        /// Gets or sets the media shell.
        /// </summary>
        /// <value>
        /// The media shell.
        /// </value>
        public IMediaShell MediaShell { get; set; }

        /// <summary>
        /// Gets the parentController.
        /// </summary>
        public IShellController Parent
        {
            get
            {
                return this.shellController;
            }
        }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        protected ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>
        /// The state of the application.
        /// </value>
        protected IMediaApplicationState ApplicationState
        {
            get
            {
                return this.lazyApplicationState.Value;
            }
        }

        /// <summary>
        /// Parses a formula string to an <see cref="EvalDataViewModelBase"/> object
        /// </summary>
        /// <param name="formula">
        /// The formula string.
        /// </param>
        /// <returns>
        /// The generated <see cref="EvalDataViewModelBase"/> object.
        /// </returns>
        public EvalDataViewModelBase ParseFormula(string formula)
        {
            var parseTree = this.parser.Parse(formula);

            if (parseTree.HasErrors())
            {
                var messages = parseTree.ParserMessages.Select(pm => pm.Message);
                throw new InvalidOperationException(string.Join("\n", messages));
            }

            var model = this.formulaModelGenerator.Generate((AstNode)parseTree.Root.AstNode);

            return model;
        }

        private static bool CanRemoveFormula(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return false;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item != null)
            {
                if (item.HasMultipleDifferingDataSources)
                {
                    return true;
                }

                var value = item.Tag as IDynamicDataValue;
                if (value != null)
                {
                    if (value.Formula != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CanRemoveAnimation(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return false;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item != null)
            {
                var value = item.Tag as IAnimatedDataValue;
                if (value != null)
                {
                    if (value.Animation != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static IMediaApplicationState GetApplicationState()
        {
            return ServiceLocator.Current.GetInstance<IMediaApplicationState>();
        }

        private void ShowTriggerEditorDictionarySelector(GenericEvalDataViewModel generic)
        {
            if (generic == null)
            {
                throw new ArgumentNullException("generic");
            }

            var recentDictionaryValues = new ExtendedObservableCollection<DictionaryValueDataViewModel>();
            var state =
            ServiceLocator.Current.GetInstance<IApplicationState>() as MediaApplicationState;
            if (state != null && state.RecentDictionaryValues != null)
            {
                recentDictionaryValues = state.RecentDictionaryValues;
            }

            Action<DictionaryTriggerEditorSelectorPrompt> afterDictionarySelectorCallback = prompt =>
            {
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

                generic.DisplayText = generic.ToString();
            };

            Logger.Debug("Request to show the trigger editor Dictionary Selector.");
            InteractionManager<DictionaryTriggerEditorSelectorPrompt>.Current.Raise(
                new DictionaryTriggerEditorSelectorPrompt(generic, this.MediaShell, recentDictionaryValues)
                    {
                        IsOpen = true
                    },
                afterDictionarySelectorCallback);
        }

        private void ShowDictionarySelector(object data)
        {
            var dataValue = data as ReflectionWrapper;
            var evaluation = string.Empty;
            if (dataValue != null)
            {
                if (dataValue.Value != null && dataValue.Value.ToString().StartsWith("$"))
                {
                    evaluation = dataValue.Value.ToString();
                }
            }
            else
            {
                var evaluationValue = data as EvalDataViewModelBase;
                if (evaluationValue != null)
                {
                    evaluation = evaluationValue.HumanReadable();
                }
            }

            var recentDictionaryValues = new ExtendedObservableCollection<DictionaryValueDataViewModel>();
            var state = ServiceLocator.Current.GetInstance<IApplicationState>() as MediaApplicationState;
            if (state != null && state.RecentDictionaryValues != null)
            {
                recentDictionaryValues = state.RecentDictionaryValues;
            }

            var action = this.CreateDictionarySelectorCallbackAction(data);

            Logger.Debug("Request to show the Dictionary Selector.");
            InteractionManager<DictionarySelectorPrompt>.Current.Raise(
                new DictionarySelectorPrompt(evaluation, this.MediaShell, recentDictionaryValues) { IsOpen = true },
                action);
        }

        private Action<DictionarySelectorPrompt> CreateDictionarySelectorCallbackAction(object data)
        {
            Action<DictionarySelectorPrompt> action = prompt =>
                {
                    var generic = new GenericEvalDataViewModel(this.MediaShell);
                    if (prompt.SelectedDictionaryValue.Column != null)
                    {
                        generic.Column.Value = prompt.SelectedDictionaryValue.Column.Index;
                    }

                    if (prompt.SelectedDictionaryValue.Table != null)
                    {
                        generic.Table.Value = prompt.SelectedDictionaryValue.Table.Index;
                        if (!prompt.SelectedDictionaryValue.Table.MultiRow)
                        {
                            prompt.SelectedDictionaryValue.Row = 0;
                        }
                    }

                    if (prompt.SelectedDictionaryValue.Language != null)
                    {
                        generic.Language.Value = prompt.SelectedDictionaryValue.Language.Index;
                    }

                    generic.Row.Value = prompt.SelectedDictionaryValue.Row;
                    var reflectionWrapper = data as ReflectionWrapper;
                    if (reflectionWrapper != null)
                    {
                        reflectionWrapper.Value = generic;
                        return;
                    }

                    var evalWrapper = data as EvaluationConfigDataViewModel;
                    if (evalWrapper != null)
                    {
                        evalWrapper.EvaluationWithReferenceCounting = generic;
                        return;
                    }

                    var matchProperty = data as MatchDynamicPropertyDataViewModel;
                    if (matchProperty != null)
                    {
                        matchProperty.EvaluationWithReferenceCounting = generic;
                        return;
                    }

                    var caseProperty = data as CaseDynamicPropertyDataViewModel;
                    if (caseProperty != null)
                    {
                        caseProperty.EvaluationWithReferenceCounting = generic;
                    }
                };
            return action;
        }

        private void RemoveFormula(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item == null)
            {
                return;
            }

            var value = item.Tag as IDynamicDataValue;
            if (value != null)
            {
                var oldElements = new List<LayoutElementDataViewModelBase>();
                var newElements = new List<LayoutElementDataViewModelBase>();

                foreach (var element in this.MediaShell.Editor.SelectedElements)
                {
                    oldElements.Add((LayoutElementDataViewModelBase)element.Clone());
                    var property = element.GetType().GetProperty(item.Name);
                    var concreteValue = (IDynamicDataValue)property.GetValue(element, null);

                    if (concreteValue == null)
                    {
                        Logger.Debug(
                            "Can't remove formula from property {0}, because it is not dynamic.", item.Name);
                        return;
                    }

                    concreteValue.Formula = null;
                    property.SetValue(element, concreteValue, null);
                    newElements.Add((LayoutElementDataViewModelBase)element.Clone());
                }

                InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
                var editor = (EditorViewModelBase)this.MediaShell.Editor;
                var updateCommand = editor.UpdateElementCommand;
                UpdateEntityParameters parameters;
                if (this.MediaShell.MediaApplicationState.CurrentPhysicalScreen.Type.Value == PhysicalScreenType.Audio)
                {
                    var audioOutputElement = newElements.FirstOrDefault(e => e is AudioOutputElementDataViewModel);
                    if (audioOutputElement != null)
                    {
                        parameters = new UpdateEntityParameters(
                    oldElements, newElements, new List<DataViewModelBase> { audioOutputElement });
                    }
                    else
                    {
                        parameters = new UpdateEntityParameters(
                        oldElements, newElements, editor.CurrentAudioOutputElement.Elements);
                    }
                }
                else
                {
                    parameters = new UpdateEntityParameters(
                        oldElements, newElements, editor.Elements);
                }

                updateCommand.Execute(parameters);
            }
            else
            {
                Logger.Debug("Can't remove formula from property {0}, because it is not dynamic.", item.Name);
            }
        }

        private void RemoveAnimation(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item == null)
            {
                return;
            }

            var value = item.Tag as IAnimatedDataValue;
            if (value != null)
            {
                var oldElements = new List<GraphicalElementDataViewModelBase>();
                var newElements = new List<GraphicalElementDataViewModelBase>();

                foreach (var element in this.MediaShell.Editor.SelectedElements)
                {
                    oldElements.Add((GraphicalElementDataViewModelBase)element.Clone());
                    var property = element.GetType().GetProperty(item.Name);
                    var concreteValue = (IAnimatedDataValue)property.GetValue(element, null);

                    if (concreteValue == null)
                    {
                        Logger.Debug(
                            "Can't remove animation from property {0}, because it is not dynamic.", item.Name);
                        return;
                    }

                    concreteValue.Animation = null;
                    property.SetValue(element, concreteValue, null);
                    newElements.Add((GraphicalElementDataViewModelBase)element.Clone());
                }

                InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
                var editor = (EditorViewModelBase)this.MediaShell.Editor;
                var updateCommand = editor.UpdateElementCommand;
                var parameters = new UpdateEntityParameters(
                    oldElements, newElements, editor.Elements);
                updateCommand.Execute(parameters);
            }
            else
            {
                Logger.Debug("Can't remove animation from property {0}, because it is not dynamic.", item.Name);
            }
        }

        private void RemoveCycleFormula(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item == null)
            {
                return;
            }

            var cycleReferenceElement = this.MediaShell.CycleNavigator.HighlightedCycle;
            if (cycleReferenceElement == null)
            {
                return;
            }

            var cycleElement = cycleReferenceElement.Reference;
            if (cycleElement == null)
            {
                return;
            }

            this.UpdateViewModelForRemoveFormula(item, cycleElement);
        }

        private void RemoveCycleAnimation(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item == null)
            {
                return;
            }

            var cycleReferenceElement = this.MediaShell.CycleNavigator.HighlightedCycle;
            if (cycleReferenceElement == null)
            {
                return;
            }

            var cycleElement = cycleReferenceElement.Reference;
            if (cycleElement == null)
            {
                return;
            }

            this.UpdateViewModelForRemoveAnimation(item, cycleElement);
        }

        private void RemoveSectionFormula(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item == null)
            {
                return;
            }

            var cycleElement = this.MediaShell.CycleNavigator.HighlightedSection;
            if (cycleElement == null)
            {
                return;
            }

            this.UpdateViewModelForRemoveFormula(item, cycleElement);
        }

        private void RemoveSectionAnimation(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item == null)
            {
                return;
            }

            var cycleElement = this.MediaShell.CycleNavigator.HighlightedSection;
            if (cycleElement == null)
            {
                return;
            }

            this.UpdateViewModelForRemoveAnimation(item, cycleElement);
        }

        private void RemovePhysicalScreenFormula(ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                return;
            }

            var item = contextMenu.PlacementTarget as PropertyGridItem;
            if (item == null)
            {
                return;
            }

            var selectedPhysicalScreen = this.MediaShell.ResolutionNavigation.HighlightedPhysicalScreen;
            if (selectedPhysicalScreen == null)
            {
                return;
            }

            this.UpdateViewModelForRemoveFormula(item, selectedPhysicalScreen);
        }

        private void UpdateViewModelForRemoveFormula(PropertyGridItem item, DataViewModelBase viewModel)
        {
            var value = item.Tag as IDynamicDataValue;
            if (value != null)
            {
                var oldElements = new List<DataViewModelBase>();
                var newElements = new List<DataViewModelBase>();

                oldElements.Add((DataViewModelBase)((ICloneable)viewModel).Clone());
                var property = viewModel.GetType().GetProperty(item.Name);
                var concreteValue = (IDynamicDataValue)property.GetValue(viewModel, null);

                if (concreteValue == null)
                {
                    Logger.Debug("Can't remove formula from property {0}, because it is not dynamic.", item.Name);
                    return;
                }

                concreteValue.Formula = null;
                property.SetValue(viewModel, concreteValue, null);
                newElements.Add((DataViewModelBase)((ICloneable)viewModel).Clone());

                InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
                var editor = (EditorViewModelBase)this.MediaShell.Editor;
                var updateCommand = editor.UpdateElementCommand;
                var parameters = new UpdateEntityParameters(
                    oldElements,
                    newElements,
                    new List<DataViewModelBase> { viewModel });
                updateCommand.Execute(parameters);
            }
            else
            {
                Logger.Debug("Can't remove formula from property {0}, because it is not dynamic.", item.Name);
            }
        }

        private void UpdateViewModelForRemoveAnimation(PropertyGridItem item, DataViewModelBase viewModel)
        {
            var value = item.Tag as IAnimatedDataValue;
            if (value != null)
            {
                var oldElements = new List<DataViewModelBase>();
                var newElements = new List<DataViewModelBase>();

                oldElements.Add((DataViewModelBase)((ICloneable)viewModel).Clone());
                var property = viewModel.GetType().GetProperty(item.Name);
                var concreteValue = (IAnimatedDataValue)property.GetValue(viewModel, null);

                if (concreteValue == null)
                {
                    Logger.Debug("Can't remove animation from property {0}, because it is not dynamic.", item.Name);
                    return;
                }

                concreteValue.Animation = null;
                property.SetValue(viewModel, concreteValue, null);
                newElements.Add((DataViewModelBase)((ICloneable)viewModel).Clone());

                InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
                var editor = (EditorViewModelBase)this.MediaShell.Editor;
                var updateCommand = editor.UpdateElementCommand;
                var parameters = new UpdateEntityParameters(
                    oldElements,
                    newElements,
                    new List<DataViewModelBase> { viewModel });
                updateCommand.Execute(parameters);
            }
            else
            {
                Logger.Debug("Can't remove animation from property {0}, because it is not dynamic.", item.Name);
            }
        }
    }
}