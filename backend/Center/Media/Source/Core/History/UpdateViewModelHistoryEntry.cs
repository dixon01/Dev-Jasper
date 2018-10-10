// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateViewModelHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using NLog;

    /// <summary>
    /// History entry that contains all information to undo / redo the update of layout elements.
    /// </summary>
    public class UpdateViewModelHistoryEntry : HistoryEntryBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<Type, Action<DataViewModelBase, DataViewModelBase>> PostUndoHook =
            new Dictionary<Type, Action<DataViewModelBase, DataViewModelBase>>();

        private readonly ICommandRegistry commandRegistry;

        private readonly IEnumerable<DataViewModelBase> elementsContainer;

        private readonly Action doCallback;

        private readonly Action undoCallback;

        private IEnumerable<DataViewModelBase> oldElements;

        private IEnumerable<DataViewModelBase> newElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateViewModelHistoryEntry"/> class.
        /// </summary>
        /// <param name="oldElements">
        /// The old elements.
        /// </param>
        /// <param name="newElements">
        /// The new elements.
        /// </param>
        /// <param name="elementsContainer">
        /// The elements Container.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        public UpdateViewModelHistoryEntry(
            IEnumerable<DataViewModelBase> oldElements,
            IEnumerable<DataViewModelBase> newElements,
            IEnumerable<DataViewModelBase> elementsContainer,
            string displayText,
            ICommandRegistry commandRegistry = null)
            : base(displayText)
        {
            this.oldElements = oldElements;
            this.newElements = newElements;
            this.elementsContainer = elementsContainer;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateViewModelHistoryEntry"/> class.
        /// </summary>
        /// <param name="oldElements">
        /// The old elements.
        /// </param>
        /// <param name="newElements">
        /// The new elements.
        /// </param>
        /// <param name="elementsContainer">
        /// The elements Container.
        /// </param>
        /// <param name="doCallback">
        /// The do callback
        /// </param>
        /// <param name="undoCallback">
        /// The undo callback
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        public UpdateViewModelHistoryEntry(
            IEnumerable<DataViewModelBase> oldElements,
            IEnumerable<DataViewModelBase> newElements,
            IEnumerable<DataViewModelBase> elementsContainer,
            Action doCallback,
            Action undoCallback,
            string displayText,
            ICommandRegistry commandRegistry = null)
            : base(displayText)
        {
            this.oldElements = oldElements;
            this.newElements = newElements;
            this.elementsContainer = elementsContainer;
            this.doCallback = doCallback;
            this.undoCallback = undoCallback;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// The execute post action hook.
        /// </summary>
        /// <param name="currentDataViewModelBase">
        /// The current data view model base.
        /// </param>
        /// <param name="previousDataViewModelBase">
        /// The previous data view model base.
        /// </param>
        public static void ExecutePostActionHook(
            DataViewModelBase currentDataViewModelBase, DataViewModelBase previousDataViewModelBase)
        {
            var type = currentDataViewModelBase.GetType();
            Action<DataViewModelBase, DataViewModelBase> action;
            PostUndoHook.TryGetValue(type, out action);
            if (action != null)
            {
                action.Invoke(currentDataViewModelBase, previousDataViewModelBase);
            }
        }

        /// <summary>
        /// Add a custom undo hook for a given type.
        /// </summary>
        /// <param name="type">Thy type to register a handle</param>
        /// <param name="action">The handle</param>
        public static void RegisterPostUndoHook(Type type, Action<DataViewModelBase, DataViewModelBase> action)
        {
            if (!PostUndoHook.ContainsKey(type))
            {
                PostUndoHook.Add(type, action);
            }
        }

        /// <summary>
        /// Clears all registered hooks.
        /// </summary>
        public static void ClearPostActionHooks()
        {
            PostUndoHook.Clear();
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.UpdateProperties(ref this.oldElements, true);

            int index;
            for (index = 0; index < this.oldElements.Count(); index++)
            {
                var currentDataViewModelBase = this.newElements.ElementAt(index);
                var previousDataViewModelBase = this.oldElements.ElementAt(index);
                ExecutePostActionHook(previousDataViewModelBase, currentDataViewModelBase);
                index++;
            }

            if (this.undoCallback != null)
            {
                this.undoCallback();
            }
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.UpdateProperties(ref this.newElements);

            int index;
            for (index = 0; index < this.newElements.Count(); index++)
            {
                var currentDataViewModelBase = this.newElements.ElementAt(index);
                var previousDataViewModelBase = this.oldElements.ElementAt(index);
                ExecutePostActionHook(currentDataViewModelBase, previousDataViewModelBase);
                index++;
            }

            if (this.doCallback != null)
            {
                this.doCallback();
            }
        }

        private void UpdateProperties(ref IEnumerable<DataViewModelBase> elements, bool undo = false)
        {
            Logger.Debug("Updating properties");
            foreach (var element in elements)
            {
                var historyElement = this.elementsContainer.SingleOrDefault(
                        e => e.GetHashCode() == element.ClonedFrom || e.GetHashCode() == element.GetHashCode());
                if (historyElement == null)
                {
                    Logger.Warn("Element '{0}' not found in the elements of the editor", element);
                    continue;
                }

                if (!undo)
                {
                    element.MakeDirty();
                }

                var properties = historyElement.GetType().GetProperties();
                foreach (var propertyInfo in properties)
                {
                    var property = historyElement.GetType().GetProperty(propertyInfo.Name);
                    var oldelementproperty = element.GetType().GetProperty(propertyInfo.Name);
                    if (property == null || !property.CanWrite)
                    {
                        Logger.Trace(
                            "Can't find property '{0}' on history element '{1}' or the property doesn't have a setter",
                            propertyInfo.Name,
                            historyElement);
                        continue;
                    }

                    var concreteValue = property.GetValue(historyElement, null) as IDataValue;
                    if (concreteValue == null)
                    {
                        Logger.Trace(
                            "The concrete value for property '{0}' in history element '{1}' is not an IDataValue",
                            property.Name,
                            historyElement);
                        if (property.Name == "Hash" && this.commandRegistry != null)
                        {
                            this.SetResourceReferences(undo, oldelementproperty, element, property, historyElement);
                        }

                        property.SetValue(historyElement, oldelementproperty.GetValue(element, null), null);
                        continue;
                    }

                    var oldConcreteValue = (IDataValue)oldelementproperty.GetValue(element, null);
                    concreteValue.ValueObject = oldConcreteValue.ValueObject;

                    this.SetAnimationValue(property, oldConcreteValue, concreteValue, historyElement);

                    if (this.TrySetDynamicValue(property, oldConcreteValue, concreteValue, historyElement))
                    {
                        continue;
                    }

                    Logger.Trace(
                        "The concrete value for property '{0}' in history element '{1}' is an IDataValue",
                        property.Name,
                        historyElement);
                    property.SetValue(historyElement, concreteValue, null);
                }
            }

            InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
        }

        private void SetResourceReferences(
            bool undo,
            PropertyInfo oldelementproperty,
            DataViewModelBase element,
            PropertyInfo property,
            DataViewModelBase historyElement)
        {
            var selectionParameters = new SelectResourceParameters();
            if (undo)
            {
                selectionParameters.CurrentSelectedResourceHash = (string)oldelementproperty.GetValue(element, null);
                selectionParameters.PreviousSelectedResourceHash = (string)property.GetValue(historyElement, null);
            }
            else
            {
                selectionParameters.CurrentSelectedResourceHash = (string)property.GetValue(historyElement, null);
                selectionParameters.PreviousSelectedResourceHash = (string)oldelementproperty.GetValue(element, null);
            }

            this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource).Execute(selectionParameters);
        }

        private bool TrySetDynamicValue(
            PropertyInfo property,
            IDataValue oldConcreteValue,
            IDataValue concreteValue,
            DataViewModelBase historyElement)
        {
            var oldConcreteDynamicValue = oldConcreteValue as IDynamicDataValue;
            if (oldConcreteDynamicValue != null)
            {
                var concreteDynamicValue = concreteValue as IDynamicDataValue;
                if (concreteDynamicValue != null)
                {
                    Logger.Trace(
                        "The concrete value for property '{0}' in history element '{1}' is an IDynamicDataValue",
                        property.Name,
                        historyElement);

                    concreteDynamicValue.Formula = oldConcreteDynamicValue.Formula;
                    property.SetValue(historyElement, concreteDynamicValue, null);
                    return true;
                }
            }

            return false;
        }

        private void SetAnimationValue(
            PropertyInfo property,
            IDataValue oldConcreteValue,
            IDataValue concreteValue,
            DataViewModelBase historyElement)
        {
            var oldConcreteAnimationValue = oldConcreteValue as IAnimatedDataValue;
            if (oldConcreteAnimationValue != null)
            {
                var concreteAnimationValue = concreteValue as IAnimatedDataValue;
                if (concreteAnimationValue != null)
                {
                    Logger.Trace(
                        "The concrete value for property '{0}' in history element '{1}' is an IDynamicDataValue",
                        property.Name,
                        historyElement);

                    concreteAnimationValue.Animation = oldConcreteAnimationValue.Animation;
                }
            }
        }
    }
}