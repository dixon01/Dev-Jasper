// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderAudioElementsHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   History entry that contains all information to undo / redo the order of audio elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// History entry that contains all information to undo / redo the order of audio elements.
    /// </summary>
    public class OrderAudioElementsHistoryEntry : HistoryEntryBase
    {
        private readonly IList elementsList;

        private readonly IOrderedEnumerable<object> dataList;

        private readonly int insertIndex;

        private int oldIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAudioElementsHistoryEntry"/> class.
        /// </summary>
        /// <param name="insertIndex">
        /// The insert Index.
        /// </param>
        /// <param name="elementList">
        /// The element List.
        /// </param>
        /// <param name="dataList">
        /// The data List.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public OrderAudioElementsHistoryEntry(
              int insertIndex,
            IList elementList,
            IList<object> dataList,
            string displayText)
            : base(displayText)
        {
            this.elementsList = elementList;
            this.dataList = dataList.OrderBy(item => ((AudioElementDataViewModelBase)item).ListIndex.Value);
            this.insertIndex = insertIndex;
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

            for (var i = 0; i < this.elementsList.Count; i++)
            {
                ((AudioElementDataViewModelBase)this.elementsList[i]).ListIndex.Value = i;
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

            for (var i = 0; i < this.elementsList.Count; i++)
            {
                ((AudioElementDataViewModelBase)this.elementsList[i]).ListIndex.Value = i;
            }
        }
    }
}
