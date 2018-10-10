// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReusableListDropHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReusableListDropHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.Views.Controls;

    using NLog;

    /// <summary>
    /// The reusable list drop handler.
    /// </summary>
    public class ReusableListDropHandler : DefaultDropHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The drag over.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop info.
        /// </param>
        public override void DragOver(IDropInfo dropInfo)
        {
            if (!DefaultDropHandler.CanAcceptData(dropInfo))
            {
                return;
            }

            if (SourceEqualsTarget(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                return;
            }

            if (dropInfo.Data.GetType() == typeof(StandardCycleRefConfigDataViewModel)
                || dropInfo.Data.GetType() == typeof(EventCycleRefConfigDataViewModel))
            {
                dropInfo.Effects = (dropInfo.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey
                                       ? DragDropEffects.Copy
                                       : DragDropEffects.Link;
            }
            else if (dropInfo.Data.GetType() == typeof(CyclePackageConfigDataViewModel))
            {
                dropInfo.Effects = DragDropEffects.None;
                dropInfo.DropTargetAdorner = null;
                return;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }

        /// <summary>
        /// Clones the dropped value, if the drag source is not the same visual. Otherwise it rearranges the items
        /// in the list.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop info.
        /// </param>
        public override void Drop(IDropInfo dropInfo)
        {
            Logger.Trace("Drop an item to a reusable list.");
            var insertIndex = dropInfo.InsertIndex;
            var destinationList = GetList(dropInfo.TargetCollection);
            var data = ExtractData(dropInfo.Data);

            if (SourceEqualsTarget(dropInfo))
            {
                var sourceList = GetList(dropInfo.DragInfo.SourceCollection);

                var enumerable = data as IList<object> ?? data.Cast<object>().ToList();
                foreach (var o in enumerable)
                {
                    var index = sourceList.IndexOf(o);

                    if (index != -1)
                    {
                        sourceList.RemoveAt(index);

                        if (sourceList == destinationList && index < insertIndex)
                        {
                            --insertIndex;
                        }
                    }
                }

                foreach (var o in enumerable)
                {
                    destinationList.Insert(insertIndex++, o);
                }

                return;
            }

            var reusableList = FindParent<ReusableList>(dropInfo.VisualTarget);
            if (reusableList != null)
            {
                if (reusableList.Entities.GetType()
                    == typeof(ExtendedObservableCollection<StandardCycleRefConfigDataViewModel>)
                    || reusableList.Entities.GetType()
                    == typeof(ExtendedObservableCollection<EventCycleRefConfigDataViewModel>))
                {
                    if (dropInfo.KeyStates == DragDropKeyStates.ControlKey)
                    {
                        foreach (var o in data)
                        {
                            reusableList.CloneEntity.Execute(o);
                        }

                        return;
                    }

                    var context = reusableList.DataContext as CycleNavigationViewModel;
                    if (context != null)
                    {
                        foreach (var o in data)
                        {
                            if (context.CreateCycleReference.CanExecute(o))
                            {
                                context.CreateCycleReference.Execute(o);
                            }
                            else
                            {
                                Logger.Debug("Cycle reference already exists.");
                            }
                        }
                    }

                    return;
                }

                foreach (var o in data)
                {
                    reusableList.CloneEntity.Execute(o);
                }
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
            {
                return null;
            }

            var parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }

        private static bool SourceEqualsTarget(IDropInfo dropInfo)
        {
            return dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource;
        }
    }
}
