namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>List box control</summary>
    public class ListBox : Control
    {
        public const int MainLayer = 0;
        public const int SelectionLayer = 1;

        #region Event code
        public event EventHandler ContentsChanged;
        public event EventHandler DoubleClick;
        public event EventHandler Selection;
        /// <summary>Raises the contents changed event</summary>
        protected void RaiseContentsChangedEvent(ListBox sender, bool wasTriggeredByUser)
        {
            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!this.Parent.IsUsingNonUserEvents && !wasTriggeredByUser)
                return;

            // Fire the event
            if (this.ContentsChanged != null)
                this.ContentsChanged(sender, EventArgs.Empty);
        }
        /// <summary>Raises the double click event</summary>
        protected void RaiseDoubleClickEvent(ListBox sender, bool wasTriggeredByUser)
        {
            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!this.Parent.IsUsingNonUserEvents && !wasTriggeredByUser)
                return;

            // Fire the event
            if (this.DoubleClick != null)
                this.DoubleClick(sender, EventArgs.Empty);
        }
        /// <summary>Raises the selection event</summary>
        protected void RaiseSelectionEvent(ListBox sender, bool wasTriggeredByUser)
        {
            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!this.Parent.IsUsingNonUserEvents && !wasTriggeredByUser)
                return;

            // Fire the event
            if (this.Selection != null)
                this.Selection(sender, EventArgs.Empty);
        }
        #endregion

        #region Instance data
        private bool isScrollBarInit;
        protected System.Drawing.Rectangle textRect; // Text rendering bound
        protected System.Drawing.Rectangle selectionRect; // Selection box bound
        protected ScrollBar scrollbarControl; 
        protected int scrollWidth;
        protected int border;
        protected int margin;
        protected int textHeight; // Height of a single line of text
        protected int selectedIndex;
        protected int selectedStarted;
        protected bool isDragging;
        protected ListBoxStyle style;
 
        protected ArrayList itemList;
        #endregion

        /// <summary>Create a new list box control</summary>
        public ListBox(Dialog parent) : base(parent)
        {
            // Store control type and parent dialog
            this.controlType = ControlType.ListBox;
            this.parentDialog = parent;
            // Create the scrollbar control too
            this.scrollbarControl = new ScrollBar(parent);

            // Set some default items
            this.style = ListBoxStyle.SingleSelection;
            this.scrollWidth = 16;
            this.selectedIndex = -1;
            this.selectedStarted = 0;
            this.isDragging = false;
            this.margin = 5;
            this.border = 6;
            this.textHeight = 0;
            this.isScrollBarInit = false;

            // Create the item list array
            this.itemList = new ArrayList();
        }

        /// <summary>Update the rectangles for the list box control</summary>
        protected override void UpdateRectangles()
        {
            // Get bounding box
            base.UpdateRectangles();

            // Calculate the size of the selection rectangle
            this.selectionRect = this.boundingBox;
            this.selectionRect.Width -= this.scrollWidth;
            this.selectionRect.Inflate(-this.border, -this.border);
            this.textRect = this.selectionRect;
            this.textRect.Inflate(-this.margin, 0);

            // Update the scroll bars rects too
            this.scrollbarControl.SetLocation(this.boundingBox.Right - this.scrollWidth, this.boundingBox.Top);
            this.scrollbarControl.SetSize(this.scrollWidth, this.height);
            FontNode fNode = DialogResourceManager.GetGlobalInstance().GetFontNode((int)(this.elementList[0] as Element).FontIndex);
            if ((fNode != null) && (fNode.Height > 0))
            {
                this.scrollbarControl.PageSize = (int)(this.textRect.Height / fNode.Height);

                // The selected item may have been scrolled off the page.
                // Ensure that it is in page again.
                this.scrollbarControl.ShowItem(this.selectedIndex);
            }
        }
        /// <summary>Sets the scroll bar width of this control</summary>
        public void SetScrollbarWidth(int width) { this.scrollWidth = width; this.UpdateRectangles(); }
        /// <summary>Can this control have focus</summary>
        public override bool CanHaveFocus { get { return (this.IsVisible && this.IsEnabled); } }
        /// <summary>Sets the style of the listbox</summary>
        public ListBoxStyle Style { get { return this.style; } set { this.style = value; } } 
        /// <summary>Number of items current in the list</summary>
        public int NumberItems { get { return this.itemList.Count; } }
        /// <summary>Indexer for items in the list</summary>
        public ListBoxItem this[int index]
        {
            get { return (ListBoxItem)this.itemList[index]; }
        }

        /// <summary>Initialize the scrollbar control here</summary>
        public override void OnInitialize()
        {
            this.parentDialog.InitializeControl(this.scrollbarControl);
        }


        /// <summary>Called when the control needs to handle the keyboard</summary>
        public override bool HandleKeyboard(NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            // Let the scroll bar have a chance to handle it first
            if (this.scrollbarControl.HandleKeyboard(msg, wParam, lParam))
                return true;

            switch (msg)
            {
                case NativeMethods.WindowMessage.KeyDown:
                    {
                        switch((System.Windows.Forms.Keys)wParam.ToInt32())
                        {
                            case System.Windows.Forms.Keys.Up:
                            case System.Windows.Forms.Keys.Down:
                            case System.Windows.Forms.Keys.Next:
                            case System.Windows.Forms.Keys.Prior:
                            case System.Windows.Forms.Keys.Home:
                            case System.Windows.Forms.Keys.End:
                                {
                                    // If no items exists, do nothing
                                    if (this.itemList.Count == 0)
                                        return true;

                                    int oldSelected = this.selectedIndex;

                                    // Adjust selectedIndex
                                    switch((System.Windows.Forms.Keys)wParam.ToInt32())
                                    {
                                        case System.Windows.Forms.Keys.Up: --this.selectedIndex; break;
                                        case System.Windows.Forms.Keys.Down: ++this.selectedIndex; break;
                                        case System.Windows.Forms.Keys.Next: this.selectedIndex += this.scrollbarControl.PageSize - 1; break;
                                        case System.Windows.Forms.Keys.Prior: this.selectedIndex -= this.scrollbarControl.PageSize - 1; break;
                                        case System.Windows.Forms.Keys.Home: this.selectedIndex = 0; break;
                                        case System.Windows.Forms.Keys.End: this.selectedIndex = this.itemList.Count - 1; break;
                                    }

                                    // Clamp the item
                                    if (this.selectedIndex < 0)
                                        this.selectedIndex = 0;
                                    if (this.selectedIndex >= this.itemList.Count)
                                        this.selectedIndex = this.itemList.Count - 1;

                                    // Did the selection change?
                                    if (oldSelected != this.selectedIndex)
                                    {
                                        if (this.style == ListBoxStyle.Multiselection)
                                        {
                                            // Clear all selection
                                            for(int i = 0; i < this.itemList.Count; i++)
                                            {
                                                ListBoxItem lbi = (ListBoxItem)this.itemList[i];
                                                lbi.IsItemSelected = false;
                                                this.itemList[i] = lbi;
                                            }

                                            // Is shift being held down?
                                            bool shiftDown = ((NativeMethods.GetAsyncKeyState
                                                                   ((int)System.Windows.Forms.Keys.ShiftKey) & 0x8000) != 0);

                                            if (shiftDown)
                                            {
                                                // Select all items from the start selection to current selected index
                                                int end = Math.Max(this.selectedStarted, this.selectedIndex);
                                                for(int i = Math.Min(this.selectedStarted, this.selectedIndex); i <= end; ++i)
                                                {
                                                    ListBoxItem lbi = (ListBoxItem)this.itemList[i];
                                                    lbi.IsItemSelected = true;
                                                    this.itemList[i] = lbi;
                                                }
                                            }
                                            else
                                            {
                                                ListBoxItem lbi = (ListBoxItem)this.itemList[this.selectedIndex];
                                                lbi.IsItemSelected = true;
                                                this.itemList[this.selectedIndex] = lbi;

                                                // Update selection start
                                                this.selectedStarted = this.selectedIndex;
                                            }

                                        }
                                        else // Update selection start
                                            this.selectedStarted = this.selectedIndex;

                                        // adjust scrollbar
                                        this.scrollbarControl.ShowItem(this.selectedIndex);
                                        this.RaiseSelectionEvent(this, true);
                                    }
                                }
                                return true;
                        }
                        break;
                    }
            }

            return false;
        }

        /// <summary>Called when the control should handle the mouse</summary>
        public override bool HandleMouse(NativeMethods.WindowMessage msg, System.Drawing.Point pt, IntPtr wParam, IntPtr lParam)
        {
            const int ShiftModifier = 0x0004;
            const int ControlModifier = 0x0008;

            if (!this.IsEnabled || !this.IsVisible)
                return false; // Nothing to do

            // First acquire focus
            if (msg == NativeMethods.WindowMessage.LeftButtonDown)
                if (!this.hasFocus)
                    Dialog.RequestFocus(this);


            // Let the scroll bar handle it first
            if (this.scrollbarControl.HandleMouse(msg, pt, wParam, lParam))
                return true;

            // Ok, scrollbar didn't handle it, move on
            switch(msg)
            {
                case NativeMethods.WindowMessage.LeftButtonDoubleClick:
                case NativeMethods.WindowMessage.LeftButtonDown:
                    {
                        // Check for clicks in the text area
                        if (this.itemList.Count > 0 && this.selectionRect.Contains(pt))
                        {
                            // Compute the index of the clicked item
                            int clicked = 0;
                            if (this.textHeight > 0)
                                clicked = this.scrollbarControl.TrackPosition + (pt.Y - this.textRect.Top) / this.textHeight;
                            else
                                clicked = -1;

                            // Only proceed if the click falls ontop of an item
                            if (clicked >= this.scrollbarControl.TrackPosition &&
                                clicked < this.itemList.Count &&
                                clicked < this.scrollbarControl.TrackPosition + this.scrollbarControl.PageSize )
                            {
                                this.Parent.SampleFramework.Window.Capture = true;
                                this.isDragging = true;

                                // If this is a double click, fire off an event and exit
                                // since the first click would have taken care of the selection
                                // updating.
                                if (msg == NativeMethods.WindowMessage.LeftButtonDoubleClick)
                                {
                                    this.RaiseDoubleClickEvent(this, true);
                                    return true;
                                }

                                this.selectedIndex = clicked;
                                if ( (wParam.ToInt32() & ShiftModifier) == 0)
                                    this.selectedStarted = this.selectedIndex; // Shift isn't down

                                // If this is a multi-selection listbox, update per-item
                                // selection data.
                                if (this.style == ListBoxStyle.Multiselection)
                                {
                                    // Determine behavior based on the state of Shift and Ctrl
                                    ListBoxItem selectedItem = (ListBoxItem)this.itemList[this.selectedIndex];
                                    if ((wParam.ToInt32() & (ShiftModifier | ControlModifier)) == ControlModifier)
                                    {
                                        // Control click, reverse the selection
                                        selectedItem.IsItemSelected = !selectedItem.IsItemSelected;
                                        this.itemList[this.selectedIndex] = selectedItem;
                                    }
                                    else if ((wParam.ToInt32() & (ShiftModifier | ControlModifier)) == ShiftModifier)
                                    {
                                        // Shift click. Set the selection for all items
                                        // from last selected item to the current item.
                                        // Clear everything else.
                                        int begin = Math.Min(this.selectedStarted, this.selectedIndex);
                                        int end = Math.Max(this.selectedStarted, this.selectedIndex);

                                        // Unselect everthing before the beginning
                                        for(int i = 0; i < begin; ++i)
                                        {
                                            ListBoxItem lb = (ListBoxItem)this.itemList[i];
                                            lb.IsItemSelected = false;
                                            this.itemList[i] = lb;
                                        }
                                        // unselect everything after the end
                                        for(int i = end + 1; i < this.itemList.Count; ++i)
                                        {
                                            ListBoxItem lb = (ListBoxItem)this.itemList[i];
                                            lb.IsItemSelected = false;
                                            this.itemList[i] = lb;
                                        }

                                        // Select everything between
                                        for(int i = begin; i <= end; ++i)
                                        {
                                            ListBoxItem lb = (ListBoxItem)this.itemList[i];
                                            lb.IsItemSelected = true;
                                            this.itemList[i] = lb;
                                        }
                                    }
                                    else if ((wParam.ToInt32() & (ShiftModifier | ControlModifier)) == (ShiftModifier | ControlModifier))
                                    {
                                        // Control-Shift-click.

                                        // The behavior is:
                                        //   Set all items from selectedStarted to selectedIndex to
                                        //     the same state as selectedStarted, not including selectedIndex.
                                        //   Set selectedIndex to selected.
                                        int begin = Math.Min(this.selectedStarted, this.selectedIndex);
                                        int end = Math.Max(this.selectedStarted, this.selectedIndex);

                                        // The two ends do not need to be set here.
                                        bool isLastSelected = ((ListBoxItem)this.itemList[this.selectedStarted]).IsItemSelected;

                                        for (int i = begin + 1; i < end; ++i)
                                        {
                                            ListBoxItem lb = (ListBoxItem)this.itemList[i];
                                            lb.IsItemSelected = isLastSelected;
                                            this.itemList[i] = lb;
                                        }

                                        selectedItem.IsItemSelected = true;
                                        this.itemList[this.selectedIndex] = selectedItem;

                                        // Restore selectedIndex to the previous value
                                        // This matches the Windows behavior

                                        this.selectedIndex = this.selectedStarted;
                                    }
                                    else
                                    {
                                        // Simple click.  Clear all items and select the clicked
                                        // item.
                                        for(int i = 0; i < this.itemList.Count; ++i)
                                        {
                                            ListBoxItem lb = (ListBoxItem)this.itemList[i];
                                            lb.IsItemSelected = false;
                                            this.itemList[i] = lb;
                                        }
                                        selectedItem.IsItemSelected = true;
                                        this.itemList[this.selectedIndex] = selectedItem;
                                    }
                                } // End of multi-selection case
                                this.RaiseSelectionEvent(this, true);
                            }
                            return true;
                        }
                        break;
                    }
                case NativeMethods.WindowMessage.LeftButtonUp:
                    {
                        this.Parent.SampleFramework.Window.Capture = false;
                        this.isDragging = false;

                        if (this.selectedIndex != -1)
                        {
                            // Set all items between selectedStarted and selectedIndex to
                            // the same state as selectedStarted
                            int end = Math.Max(this.selectedStarted, this.selectedIndex);
                            for (int i = Math.Min(this.selectedStarted, this.selectedIndex) + 1; i < end; ++i)
                            {
                                ListBoxItem lb = (ListBoxItem)this.itemList[i];
                                lb.IsItemSelected = ((ListBoxItem)this.itemList[this.selectedStarted]).IsItemSelected;
                                this.itemList[i] = lb;
                            }
                            ListBoxItem lbs = (ListBoxItem)this.itemList[this.selectedIndex];
                            lbs.IsItemSelected = ((ListBoxItem)this.itemList[this.selectedStarted]).IsItemSelected;
                            this.itemList[this.selectedIndex] = lbs;

                            // If selectedStarted and selectedIndex are not the same,
                            // the user has dragged the mouse to make a selection.
                            // Notify the application of this.
                            if (this.selectedIndex != this.selectedStarted)
                                this.RaiseSelectionEvent(this, true);
                        }
                        break;
                    }
                case NativeMethods.WindowMessage.MouseWheel:
                    {
                        int lines = System.Windows.Forms.SystemInformation.MouseWheelScrollLines;
                        int scrollAmount = (int)(NativeMethods.HiWord((uint)wParam.ToInt32()) / Dialog.WheelDelta * lines);
                        this.scrollbarControl.Scroll(-scrollAmount);
                        break;
                    }

                case NativeMethods.WindowMessage.MouseMove:
                    {
                        if (this.isDragging)
                        {
                            // compute the index of the item below the cursor
                            int itemIndex = -1;
                            if (this.textHeight > 0)
                                itemIndex = this.scrollbarControl.TrackPosition + (pt.Y - this.textRect.Top) / this.textHeight;

                            // Only proceed if the cursor is on top of an item
                            if (itemIndex >= this.scrollbarControl.TrackPosition &&
                                itemIndex < this.itemList.Count &&
                                itemIndex < this.scrollbarControl.TrackPosition + this.scrollbarControl.PageSize)
                            {
                                this.selectedIndex = itemIndex;
                                this.RaiseSelectionEvent(this, true);
                            }
                            else if (itemIndex < this.scrollbarControl.TrackPosition)
                            {
                                // User drags the mouse above window top
                                this.scrollbarControl.Scroll(-1);
                                this.selectedIndex = this.scrollbarControl.TrackPosition;
                                this.RaiseSelectionEvent(this, true);
                            }
                            else if (itemIndex >= this.scrollbarControl.TrackPosition + this.scrollbarControl.PageSize)
                            {
                                // User drags the mouse below the window bottom
                                this.scrollbarControl.Scroll(1);
                                this.selectedIndex = Math.Min(this.itemList.Count, this.scrollbarControl.TrackPosition + this.scrollbarControl.PageSize - 1);
                                this.RaiseSelectionEvent(this, true);
                            }
                        }
                        break;
                    }
            }

            // Didn't handle it
            return false;
        }

        /// <summary>Called when the control should be rendered</summary>
        public override void Render(Device device, float elapsedTime)
        {
            if (!this.IsVisible)
                return; // Nothing to render
            
            Element e = this.elementList[ListBox.MainLayer] as Element;

            // Blend current color
            e.TextureColor.Blend(ControlState.Normal, elapsedTime);
            e.FontColor.Blend(ControlState.Normal, elapsedTime);
            
            Element selectedElement = this.elementList[ListBox.SelectionLayer] as Element;

            // Blend current color
            selectedElement.TextureColor.Blend(ControlState.Normal, elapsedTime);
            selectedElement.FontColor.Blend(ControlState.Normal, elapsedTime);

            this.parentDialog.DrawSprite(e, this.boundingBox);

            // Render the text
            if (this.itemList.Count > 0)
            {
                // Find out the height of a single line of text
                System.Drawing.Rectangle rc = this.textRect;
                System.Drawing.Rectangle sel = this.selectionRect;
                rc.Height = (int)(DialogResourceManager.GetGlobalInstance().GetFontNode((int)e.FontIndex).Height);
                this.textHeight = rc.Height;

                // If we have not initialized the scroll bar page size,
                // do that now.
                if (!this.isScrollBarInit)
                {
                    if (this.textHeight > 0)
                        this.scrollbarControl.PageSize = (int)(this.textRect.Height / this.textHeight);
                    else
                        this.scrollbarControl.PageSize = this.textRect.Height;

                    this.isScrollBarInit = true;
                }
                rc.Width = this.textRect.Width;
                for (int i = this.scrollbarControl.TrackPosition; i < this.itemList.Count; ++i)
                {
                    if (rc.Bottom > this.textRect.Bottom)
                        break;

                    ListBoxItem lb = (ListBoxItem)this.itemList[i];

                    // Determine if we need to render this item with the
                    // selected element.
                    bool isSelectedStyle = false;

                    if ( (this.selectedIndex == i) && (this.style == ListBoxStyle.SingleSelection) )
                        isSelectedStyle = true;
                    else if (this.style == ListBoxStyle.Multiselection)
                    {
                        if (this.isDragging && ( ( i >= this.selectedIndex && i < this.selectedStarted) || 
                                            (i <= this.selectedIndex && i > this.selectedStarted) ) )
                        {
                            ListBoxItem selStart = (ListBoxItem)this.itemList[this.selectedStarted];
                            isSelectedStyle = selStart.IsItemSelected;
                        }
                        else
                            isSelectedStyle = lb.IsItemSelected;
                    }

                    // Now render the text
                    if (isSelectedStyle)
                    {
                        sel.Location = new System.Drawing.Point(sel.Left, rc.Top); 
                        sel.Height = rc.Height;
                        this.parentDialog.DrawSprite(selectedElement, sel);
                        this.parentDialog.DrawText(lb.ItemText, selectedElement, rc);
                    }
                    else
                        this.parentDialog.DrawText(lb.ItemText, e, rc);

                    rc.Offset(0, this.textHeight);
                }
            }

            // Render the scrollbar finally
            this.scrollbarControl.Render(device, elapsedTime);
        }

        
        #region Item Controlling methods
        /// <summary>Adds an item to the list box control</summary>
        public void AddItem(string text, object data)
        {
            if ((text == null) || (text.Length == 0))
                throw new ArgumentNullException("text", "You must pass in a valid item name when adding a new item.");

            // Create a new item and add it
            ListBoxItem newitem = new ListBoxItem();
            newitem.ItemText = text;
            newitem.ItemData = data;
            newitem.IsItemSelected = false;
            this.itemList.Add(newitem);

            // Update the scrollbar with the new range
            this.scrollbarControl.SetTrackRange(0, this.itemList.Count);
        }
        /// <summary>Inserts an item to the list box control</summary>
        public void InsertItem(int index, string text, object data)
        {
            if ((text == null) || (text.Length == 0))
                throw new ArgumentNullException("text", "You must pass in a valid item name when adding a new item.");

            // Create a new item and insert it
            ListBoxItem newitem = new ListBoxItem();
            newitem.ItemText = text;
            newitem.ItemData = data;
            newitem.IsItemSelected = false;
            this.itemList.Insert(index, newitem);

            // Update the scrollbar with the new range
            this.scrollbarControl.SetTrackRange(0, this.itemList.Count);
        }

        /// <summary>Removes an item at a particular index</summary>
        public void RemoveAt(int index)
        {
            // Remove the item
            this.itemList.RemoveAt(index);

            // Update the scrollbar with the new range
            this.scrollbarControl.SetTrackRange(0, this.itemList.Count);

            if (this.selectedIndex >= this.itemList.Count)
                this.selectedIndex = this.itemList.Count - 1;

            this.RaiseSelectionEvent(this, true);
        }

        /// <summary>Removes all items from the control</summary>
        public void Clear()
        {
            // clear the list
            this.itemList.Clear();

            // Update scroll bar and index
            this.scrollbarControl.SetTrackRange(0, 1);
            this.selectedIndex = -1;
        }

        /// <summary>
        /// For single-selection listbox, returns the index of the selected item.
        /// For multi-selection, returns the first selected item after the previousSelected position.
        /// To search for the first selected item, the app passes -1 for previousSelected.  For
        /// subsequent searches, the app passes the returned index back to GetSelectedIndex as.
        /// previousSelected.
        /// Returns -1 on error or if no item is selected.
        /// </summary>
        public int GetSelectedIndex(int previousSelected)
        {
            if (previousSelected < -1)
                return -1;

            if (this.style == ListBoxStyle.Multiselection)
            {
                // Multiple selections enabled.  Search for the next item with the selected flag
                for (int i = previousSelected + 1; i < this.itemList.Count; ++i)
                {
                    ListBoxItem lbi = (ListBoxItem)this.itemList[i];
                    if (lbi.IsItemSelected)
                        return i;
                }

                return -1;
            }
            else
            {
                // Single selection
                return this.selectedIndex;
            }
        }
        /// <summary>Gets the selected item</summary>
        public ListBoxItem GetSelectedItem(int previousSelected)
        {
            return (ListBoxItem)this.itemList[this.GetSelectedIndex(previousSelected)];
        }
        /// <summary>Gets the selected item</summary>
        public ListBoxItem GetSelectedItem() { return this.GetSelectedItem(-1); }

        /// <summary>Sets the border and margin sizes</summary>
        public void SetBorder(int borderSize, int marginSize)
        {
            this.border = borderSize;
            this.margin = marginSize;
            this.UpdateRectangles();
        }

        /// <summary>Selects this item</summary>
        public void SelectItem(int newIndex)
        {
            if (this.itemList.Count == 0)
                return; // If no items exist there's nothing to do

            int oldSelected = this.selectedIndex;

            // Select the new item
            this.selectedIndex = newIndex;

            // Clamp the item
            if (this.selectedIndex < 0)
                this.selectedIndex = 0;
            if (this.selectedIndex > this.itemList.Count)
                this.selectedIndex = this.itemList.Count - 1;

            // Did the selection change?
            if (oldSelected != this.selectedIndex)
            {
                if (this.style == ListBoxStyle.Multiselection)
                {
                    ListBoxItem lbi = (ListBoxItem)this.itemList[this.selectedIndex];
                    lbi.IsItemSelected = true;
                    this.itemList[this.selectedIndex] = lbi;
                }

                // Update selection start
                this.selectedStarted = this.selectedIndex;

                // adjust scrollbar
                this.scrollbarControl.ShowItem(this.selectedIndex);
            }
            this.RaiseSelectionEvent(this, true);
        }
        #endregion
    }
}