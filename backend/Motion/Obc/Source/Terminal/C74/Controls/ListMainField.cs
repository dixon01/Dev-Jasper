// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The list main field.
    /// </summary>
    public partial class ListMainField : MainField, IList
    {
        private readonly ScrollableListModel<string> model;

        private readonly ButtonInput[] buttons;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListMainField"/> class.
        /// </summary>
        public ListMainField()
        {
            this.InitializeComponent();

            this.buttons = new[]
                               {
                                   this.buttonInput1, this.buttonInput2, this.buttonInput3, this.buttonInput4,
                                   this.buttonInput5, this.buttonInput6
                               };
            this.model = new ScrollableListModel<string>(this.buttons.Length);
            this.model.SelectedIndexChanged += (s, e) => this.UpdateItems();
        }

        /// <summary>
        /// The selected index changed event.
        /// </summary>
        public event EventHandler<IndexEventArgs> SelectedIndexChanged;

        /// <summary>
        /// List for TC
        /// </summary>
        /// <param name = "caption">List caption/header</param>
        /// <param name = "items">Items to be displayed</param>
        /// <param name = "allowEscape">
        /// Allows to use go back from the list without choose an item.
        /// Set this to false if you want that a user has join an item
        /// </param>
        public void Init(string caption, List<string> items, bool allowEscape)
        {
            this.Init(caption, items, allowEscape, 0);
        }

        /// <summary>
        /// List for TC
        /// </summary>
        /// <param name = "caption">List caption/header</param>
        /// <param name = "items">Items to be displayed</param>
        /// <param name = "allowEscape">
        /// Allows to use go back from the list without choose an item.
        /// Set this to false if you want that a user has join an item
        /// </param>
        /// <param name="focusIndex">
        /// Index of the focused item in the list.
        /// </param>
        public void Init(string caption, List<string> items, bool allowEscape, int focusIndex)
        {
            // TODO: allowEscape is ignored since I couldn't find a place where it is set to false
            this.Init();

            this.labelCaption.Text = caption;

            this.model.Fill(items);
            this.model.SelectedIndex = focusIndex;
        }

        /// <summary>
        /// Processes the given key.
        /// </summary>
        /// <param name="key">
        /// The key. This is never <see cref="C74Keys.None"/>.
        /// </param>
        /// <returns>
        /// True if the key was handled otherwise false.
        /// </returns>
        public override bool ProcessKey(C74Keys key)
        {
            if (this.model.ProcessKey(key))
            {
                return true;
            }

            return base.ProcessKey(key);
        }

        /// <summary>
        /// Raises the <see cref="SelectedIndexChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseSelectedIndexChanged(IndexEventArgs e)
        {
            var handler = this.SelectedIndexChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void UpdateItems()
        {
            for (var i = 0; i < this.buttons.Length; i++)
            {
                var item = this.model.GetVisibleItem(i);
                if (item != null)
                {
                    this.buttons[i].Text = item;
                    this.buttons[i].Visible = true;
                    this.buttons[i].IsSelected = i == this.model.SelectedIndex - this.model.Offset;
                }
                else
                {
                    this.buttons[i].Visible = false;
                    this.buttons[i].IsSelected = false;
                }
            }
        }

        private void ButtonInputOnPressed(object sender, EventArgs e)
        {
            this.RaiseSelectedIndexChanged(new IndexEventArgs(this.model.SelectedIndex));
        }
    }
}
