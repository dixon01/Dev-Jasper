// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderLayerHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   History entry that contains all information to undo / redo the order of layout elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// History entry that contains all information to undo / redo the order of layout elements (z-Index).
    /// </summary>
    public class OrderLayerHistoryEntry : HistoryEntryBase
    {
        private readonly IList elementsList;

        private readonly IOrderedEnumerable<object> dataList;

        private readonly int insertIndex;

        private ExtendedObservableCollection<LayoutElementDataViewModelBase> selectedElementsList;

        private int oldIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderLayerHistoryEntry"/> class.
        /// </summary>
        /// <param name="insertIndex">
        /// The insert index of the drop info.
        /// </param>
        /// <param name="elementList">
        /// The layout element list to modify.
        /// </param>
        /// <param name="dataList">
        /// The objects list of the drop info.
        /// </param>
        /// <param name="selectedElementsList">
        /// The selected Elements List.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public OrderLayerHistoryEntry(
            int insertIndex,
            IList elementList,
            IList<object> dataList,
            ExtendedObservableCollection<LayoutElementDataViewModelBase> selectedElementsList,
            string displayText)
            : base(displayText)
        {
            this.elementsList = elementList;
            this.dataList = dataList.OrderByDescending(item => ((DrawableElementDataViewModelBase)item).ZIndex.Value);
            this.insertIndex = insertIndex;
            this.selectedElementsList = selectedElementsList;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            var currentInsertIndex = this.insertIndex;

            foreach (var index in this.dataList.Select(o => this.elementsList.IndexOf(o)).Where(index => index != -1))
            {
                this.oldIndex = index;
                this.elementsList.RemoveAt(index);

                if (index < currentInsertIndex)
                {
                    --currentInsertIndex;
                }
            }

            foreach (var o in this.dataList)
            {
                this.elementsList.Insert(currentInsertIndex++, o);
            }

            var depthIndex = this.elementsList.Count - 1;
            this.selectedElementsList.Clear();
            foreach (var layoutElement in this.elementsList.OfType<DrawableElementDataViewModelBase>())
            {
                layoutElement.ZIndex.Value = depthIndex;
                depthIndex--;
                if (this.dataList.Contains(layoutElement))
                {
                    this.selectedElementsList.Add(layoutElement);
                }
            }
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            var oldInsertIndex = this.oldIndex;
            foreach (var index in this.dataList.Select(o => this.elementsList.IndexOf(o)).Where(index => index != -1))
            {
                this.elementsList.RemoveAt(index);
            }

            foreach (var o in this.dataList)
            {
                if (oldInsertIndex > this.elementsList.Count - 1)
                {
                    this.elementsList.Add(o);
                }
                else
                {
                    this.elementsList.Insert(oldInsertIndex, o);
                }

                oldInsertIndex++;
            }

            var depthIndex = this.elementsList.Count - 1;
            this.selectedElementsList.Clear();
            foreach (var layoutElement in this.elementsList.OfType<DrawableElementDataViewModelBase>())
            {
                layoutElement.ZIndex.Value = depthIndex;
                depthIndex--;

                if (this.dataList.Contains(layoutElement))
                {
                    this.selectedElementsList.Add(layoutElement);
                }
            }
        }
    }
}
