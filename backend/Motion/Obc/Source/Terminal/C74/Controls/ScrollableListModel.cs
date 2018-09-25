// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollableListModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScrollableListModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model for a list that can be scrolled through.
    /// The list will move up and down one item before the last.
    /// Thus the last visible item will only be selected when we are at the bottom (and vice versa for the top).
    /// </summary>
    /// <typeparam name="T">
    /// The type of item in this list model.
    /// </typeparam>
    internal class ScrollableListModel<T>
        where T : class
    {
        private readonly int visibleCount;

        private readonly List<T> allItems = new List<T>();

        private int selectedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollableListModel{T}"/> class.
        /// </summary>
        /// <param name="visibleCount">
        /// The number of items that are visible at the maximum.
        /// </param>
        public ScrollableListModel(int visibleCount)
        {
            this.visibleCount = visibleCount;
        }

        /// <summary>
        /// Event that is fired whenever the <see cref="SelectedIndex"/> changes.
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// Gets the offset of the first visible item into the list.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Gets or sets the index of the selected item in the entire list.
        /// This is NOT the index in the visible list, but in the entire list!
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                var index = Math.Min(value, this.allItems.Count - 1);
                if (this.selectedIndex == index)
                {
                    return;
                }

                this.selectedIndex = index;
                this.SetBottomOffset();
                this.RaiseSelectedIndexChanged();
            }
        }

        /// <summary>
        /// Resets this model and fills it with the given items.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void Fill(IEnumerable<T> items)
        {
            this.SelectedIndex = 0;
            this.Offset = 0;

            this.allItems.Clear();
            this.allItems.AddRange(items);
            this.RaiseSelectedIndexChanged();
        }

        /// <summary>
        /// Gets the visible item at the given index.
        /// </summary>
        /// <param name="index">
        /// The index into the visible items.
        /// </param>
        /// <returns>
        /// The item or null if there is no item at the given index.
        /// </returns>
        public T GetVisibleItem(int index)
        {
            return this.Offset + index < this.allItems.Count ? this.allItems[this.Offset + index] : null;
        }

        /// <summary>
        /// Processes the given key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// True if the key was handled otherwise false.
        /// </returns>
        public bool ProcessKey(C74Keys key)
        {
            // special focus handling (since we might be moving items)
            if (key == C74Keys.Up)
            {
                this.selectedIndex = (this.selectedIndex + this.allItems.Count - 1) % this.allItems.Count;
                if (this.selectedIndex == this.Offset)
                {
                    if (this.Offset > 0)
                    {
                        this.Offset--;
                    }
                }
                else if (this.selectedIndex == this.allItems.Count - 1)
                {
                    this.SetBottomOffset();
                }
            }
            else if (key == C74Keys.Down)
            {
                this.selectedIndex = (this.selectedIndex + 1) % this.allItems.Count;
                if (this.selectedIndex < this.allItems.Count - 1
                    && this.Offset + this.visibleCount - 1 == this.selectedIndex)
                {
                    this.Offset++;
                }
                else if (this.selectedIndex == 0)
                {
                    this.Offset = 0;
                }
            }
            else
            {
                return false;
            }

            this.RaiseSelectedIndexChanged();
            return true;
        }

        /// <summary>
        /// Raises the <see cref="SelectedIndexChanged"/> event.
        /// </summary>
        protected virtual void RaiseSelectedIndexChanged()
        {
            var handler = this.SelectedIndexChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void SetBottomOffset()
        {
            this.Offset = Math.Max(
                0,
                Math.Min(this.selectedIndex - this.visibleCount + 2, this.allItems.Count - this.visibleCount));
        }
    }
}
