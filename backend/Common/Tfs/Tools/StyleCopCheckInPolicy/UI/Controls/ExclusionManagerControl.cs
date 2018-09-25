//--------------------------------------------------------------------------
// <copyright file="ExclusionManagerControl.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      Microsoft Public License (Ms-PL) which can be found in the License.rtf 
//      at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Controls.Design;

    /// <summary>
    /// Provides a user control for managing exclusions. This class cannot be inherited.
    /// </summary>
    [ToolboxItem(true)]
    internal sealed partial class ExclusionManagerControl : BaseUserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExclusionManagerControl"/> class.
        /// </summary>
        public ExclusionManagerControl()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when an item is being added.
        /// </summary>
        public event EventHandler<ItemEventArgs> AddingItem;

        /// <summary>
        /// Occurs when an item has been added.
        /// </summary>
        public event EventHandler<EventArgs> AddedItem;

        /// <summary>
        /// Occurs when an item is being edited.
        /// </summary>
        public event EventHandler<ItemEventArgs> EditingItem;

        /// <summary>
        /// Occurs when an item has been edited.
        /// </summary>
        public event EventHandler<EventArgs> EditedItem;

        /// <summary>
        /// Occurs when an item is being removed.
        /// </summary>
        public event EventHandler<ItemEventArgs> RemovingItem;

        /// <summary>
        /// Occurs when an item has been removed.
        /// </summary>
        public event EventHandler<EventArgs> RemovedItem;

        /// <summary>
        /// Occurs when an item is being enabled.
        /// </summary>
        public event EventHandler<ItemEventArgs> EnablingItem;

        /// <summary>
        /// Occurs when an item has been enabled.
        /// </summary>
        public event EventHandler<EventArgs> EnabledItem;

        /// <summary>
        /// Occurs when an item is being disabled.
        /// </summary>
        public event EventHandler<ItemEventArgs> DisablingItem;

        /// <summary>
        /// Occurs when an item has been disabled.
        /// </summary>
        public event EventHandler<EventArgs> DisabledItem;

        /// <summary>
        /// Occurs when the selected index has changed.
        /// </summary>
        public event EventHandler<EnableButtonsEventArgs> SelectedIndexChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection containing all items in the control.
        /// </summary>
        public ListView.ListViewItemCollection Items
        {
            get
            {
                return this.ExclusionsListView.Items;
            }
        }

        /// <summary>
        /// Gets the items that are selected on the control.
        /// </summary>
        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get
            {
                return this.ExclusionsListView.SelectedItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the enable button is visible.
        /// </summary>
        [DefaultValue(true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used for the Visual Studio forms designer.")]
        public bool EnableButtonVisible
        {
            get
            {
                return this.EnableButton.Visible;
            }

            set
            {
                this.EnableButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the disable button is visible.
        /// </summary>
        [DefaultValue(true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used for the Visual Studio forms designer.")]
        public bool DisableButtonVisible
        {
            get
            {
                return this.DisableButton.Visible;
            }

            set
            {
                this.DisableButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can give the focus to this control using the TAB key.
        /// </summary>
        [DefaultValue(true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used for the Visual Studio forms designer.")]
        public bool ListViewTabStop
        {
            get
            {
                return this.ExclusionsListView.TabStop;
            }

            set
            {
                this.ExclusionsListView.TabStop = value;
            }
        }

        #endregion

        /// <summary>
        /// Raises the <see cref="SelectedIndexChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="UpdateButtonsEventArgs"/> containing event data.</param>
        private void OnSelectedIndexChanged(EnableButtonsEventArgs e)
        {
            if (this.SelectedIndexChanged != null)
            {
                this.SelectedIndexChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="EnablingItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ItemEventArgs"/> containing event data.</param>
        private void OnEnablingItem(ItemEventArgs e)
        {
            if (this.EnablingItem != null)
            {
                this.EnablingItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="EnabledItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnEnabledItem(EventArgs e)
        {
            if (this.EnabledItem != null)
            {
                this.EnabledItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DisablingItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ItemEventArgs"/> containing event data.</param>
        private void OnDisablingItem(ItemEventArgs e)
        {
            if (this.DisablingItem != null)
            {
                this.DisablingItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DisabledItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnDisabledItem(EventArgs e)
        {
            if (this.DisabledItem != null)
            {
                this.DisabledItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="EditingItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ItemEventArgs"/> containing event data.</param>
        private void OnEditingItem(ItemEventArgs e)
        {
            if (this.EditingItem != null)
            {
                this.EditingItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="EditedItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnEditedItem(EventArgs e)
        {
            if (this.EditedItem != null)
            {
                this.EditedItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="AddingItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ItemEventArgs"/> containing event data.</param>
        private void OnAddingItem(ItemEventArgs e)
        {
            if (this.AddingItem != null)
            {
                this.AddingItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="AddedItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnAddedItem(EventArgs e)
        {
            if (this.AddedItem != null)
            {
                this.AddedItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RemovingItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ItemEventArgs"/> containing event data.</param>
        private void OnRemovingItem(ItemEventArgs e)
        {
            if (this.RemovingItem != null)
            {
                this.RemovingItem(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RemovedItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnRemovedItem(EventArgs e)
        {
            if (this.RemovedItem != null)
            {
                this.RemovedItem(this, e);
            }
        }

        /// <summary>
        /// Adds an existing item.
        /// </summary>
        private void AddItem()
        {
            ListViewItem item = new ListViewItem();

            ItemEventArgs e = new ItemEventArgs(item);
            this.OnAddingItem(e);

            if (!e.Cancel)
            {
                this.ExclusionsListView.Items.Add(item);

                this.OnAddedItem(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Edits an existing item.
        /// </summary>
        private void EditItem()
        {
            if (this.ExclusionsListView.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem item = this.ExclusionsListView.SelectedItems[0];

            ItemEventArgs e = new ItemEventArgs(item);
            this.OnEditingItem(e);

            if (!e.Cancel)
            {
                this.OnEditedItem(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        private void RemoveItem()
        {
            if (this.ExclusionsListView.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem item = this.ExclusionsListView.SelectedItems[0];

            ItemEventArgs e = new ItemEventArgs(item);
            this.OnRemovingItem(e);

            if (!e.Cancel)
            {
                item.Remove();

                this.OnRemovedItem(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Enables an item.
        /// </summary>
        private void EnableItem()
        {
            if (this.ExclusionsListView.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem item = this.ExclusionsListView.SelectedItems[0];

            ItemEventArgs e = new ItemEventArgs(item);
            this.OnEnablingItem(e);

            if (!e.Cancel)
            {
                this.EnableButton.Enabled = false;
                this.DisableButton.Enabled = true;

                this.OnEnabledItem(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Disables an item.
        /// </summary>
        private void DisableItem()
        {
            if (this.ExclusionsListView.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem item = this.ExclusionsListView.SelectedItems[0];

            ItemEventArgs e = new ItemEventArgs(item);
            this.OnDisablingItem(e);

            if (!e.Cancel)
            {
                this.EnableButton.Enabled = true;
                this.DisableButton.Enabled = false;

                this.OnDisabledItem(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Updates the buttons on the control.
        /// </summary>
        private void UpdateButtons()
        {
            bool selected = this.ExclusionsListView.SelectedItems.Count > 0;

            if (!selected)
            {
                this.EditButton.Enabled = selected;
                this.RemoveButton.Enabled = selected;
                this.EnableButton.Enabled = selected;
                this.DisableButton.Enabled = selected;
            }
            else if (selected)
            {
                ListViewItem item = this.ExclusionsListView.SelectedItems[0];

                if (item != null)
                {
                    EnableButtonsEventArgs e = new EnableButtonsEventArgs(item);
                    e.AddButton = true;
                    e.EditButton = selected;
                    e.RemoveButton = selected;
                    e.EnableButton = false;
                    e.DisableButton = false;

                    this.OnSelectedIndexChanged(e);

                    if (!e.Cancel)
                    {
                        this.AddButton.Enabled = e.AddButton;
                        this.EditButton.Enabled = e.EditButton;
                        this.RemoveButton.Enabled = e.RemoveButton;
                        this.EnableButton.Enabled = e.EnableButton;
                        this.DisableButton.Enabled = e.DisableButton;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionsListView"/> selected index has changed.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void ExclusionsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateButtons();
        }

        /// <summary>
        /// Occurs when the <see cref="AddButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            this.AddItem();
        }

        /// <summary>
        /// Occurs when the <see cref="EditButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void EditButton_Click(object sender, EventArgs e)
        {
            this.EditItem();
        }

        /// <summary>
        /// Occurs when the <see cref="RemoveButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            this.RemoveItem();
        }

        /// <summary>
        /// Occurs when the <see cref="EnableButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void EnableButton_Click(object sender, EventArgs e)
        {
            this.EnableItem();
        }

        /// <summary>
        /// Occurs when the <see cref="DisableButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void DisableButton_Click(object sender, EventArgs e)
        {
            this.DisableItem();
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionsListView"/> is double clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void ExclusionsListView_DoubleClick(object sender, EventArgs e)
        {
            this.EditItem();
        }
    }
}