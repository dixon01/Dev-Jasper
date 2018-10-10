namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>Combo box control</summary>
    public class ComboBox : Button
    {
        public const int MainLayer = 0;
        public const int ComboButtonLayer = 1;
        public const int DropdownLayer = 2;
        public const int SelectionLayer = 3;
        #region Event code
        public event EventHandler Changed;
        /// <summary>Create new button instance</summary>
        protected void RaiseChangedEvent(ComboBox sender, bool wasTriggeredByUser)
        {
            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!this.Parent.IsUsingNonUserEvents && !wasTriggeredByUser)
                return;

            // Fire both the changed and clicked event
            base.RaiseClickEvent(sender, wasTriggeredByUser);
            if (this.Changed != null)
                this.Changed(sender, EventArgs.Empty);
        }
        #endregion
        private bool isScrollBarInit;

        #region Instance data
        protected int selectedIndex;
        protected int focusedIndex;
        protected int dropHeight;
        protected ScrollBar scrollbarControl;
        protected int scrollWidth;
        protected bool isComboOpen;
        protected System.Drawing.Rectangle textRect;
        protected System.Drawing.Rectangle buttonRect;
        protected System.Drawing.Rectangle dropDownRect;
        protected System.Drawing.Rectangle dropDownTextRect;
        protected ArrayList itemList;
        #endregion

        /// <summary>Create new combo box control</summary>
        public ComboBox(Dialog parent) : base(parent)
        {
            // Store control type and parent dialog
            this.controlType = ControlType.ComboBox;
            this.parentDialog = parent;
            // Create the scrollbar control too
            this.scrollbarControl = new ScrollBar(parent);

            // Set some default items
            this.dropHeight = 100;
            this.scrollWidth = 16;
            this.selectedIndex = -1;
            this.focusedIndex = -1;
            this.isScrollBarInit = false;

            // Create the item list array
            this.itemList = new ArrayList();
        }

        /// <summary>Update the rectangles for the combo box control</summary>
        protected override void UpdateRectangles()
        {
            // Get bounding box
            base.UpdateRectangles();

            // Update the bounding box for the items
            this.buttonRect = new System.Drawing.Rectangle(this.boundingBox.Right - this.boundingBox.Height, this.boundingBox.Top,
                this.boundingBox.Height, this.boundingBox.Height);

            this.textRect = this.boundingBox;
            this.textRect.Size = new System.Drawing.Size(this.textRect.Width - this.buttonRect.Width, this.textRect.Height);

            this.dropDownRect = this.textRect;
            this.dropDownRect.Offset(0, (int)(0.9f * this.textRect.Height));
            this.dropDownRect.Size = new System.Drawing.Size(this.dropDownRect.Width - this.scrollWidth, this.dropDownRect.Height + this.dropHeight);

            // Scale it down slightly
            System.Drawing.Point loc = this.dropDownRect.Location;
            System.Drawing.Size size = this.dropDownRect.Size;

            loc.X += (int)(0.1f * this.dropDownRect.Width);
            loc.Y += (int)(0.1f * this.dropDownRect.Height);
            size.Width -= (2 * (int)(0.1f * this.dropDownRect.Width));
            size.Height -= (2 * (int)(0.1f * this.dropDownRect.Height));

            this.dropDownTextRect = new System.Drawing.Rectangle(loc, size);

            // Update the scroll bars rects too
            this.scrollbarControl.SetLocation(this.dropDownRect.Right, this.dropDownRect.Top + 2);
            this.scrollbarControl.SetSize(this.scrollWidth, this.dropDownRect.Height - 2);
            FontNode fNode = DialogResourceManager.GetGlobalInstance().GetFontNode((int)(this.elementList[2] as Element).FontIndex);
            if ((fNode != null) && (fNode.Height > 0))
            {
                this.scrollbarControl.PageSize = (int)(this.dropDownTextRect.Height / fNode.Height);

                // The selected item may have been scrolled off the page.
                // Ensure that it is in page again.
                this.scrollbarControl.ShowItem(this.selectedIndex);
            }
        }

        /// <summary>Sets the drop height of this control</summary>
        public void SetDropHeight(int height) { this.dropHeight = height; this.UpdateRectangles(); }
        /// <summary>Sets the scroll bar width of this control</summary>
        public void SetScrollbarWidth(int width) { this.scrollWidth = width; this.UpdateRectangles(); }
        /// <summary>Can this control have focus</summary>
        public override bool CanHaveFocus { get { return (this.IsVisible && this.IsEnabled); } }
        /// <summary>Number of items current in the list</summary>
        public int NumberItems { get { return this.itemList.Count; } }
        /// <summary>Indexer for items in the list</summary>
        public ComboBoxItem this[int index]
        {
            get { return (ComboBoxItem)this.itemList[index]; }
        }

        /// <summary>Initialize the scrollbar control here</summary>
        public override void OnInitialize()
        {
            this.parentDialog.InitializeControl(this.scrollbarControl);
        }

        /// <summary>Called when focus leaves the control</summary>
        public override void OnFocusOut()
        {
            // Call base first
            base.OnFocusOut ();
            this.isComboOpen = false;
        }
        /// <summary>Called when the control's hotkey is pressed</summary>
        public override void OnHotKey()
        {
            if (this.isComboOpen)
                return; // Nothing to do yet

            if (this.selectedIndex == -1)
                return; // Nothing selected

            this.selectedIndex++;
            if (this.selectedIndex >= this.itemList.Count)
                this.selectedIndex = 0;

            this.focusedIndex = this.selectedIndex;
            this.RaiseChangedEvent(this, true);
        }


        /// <summary>Called when the control needs to handle the keyboard</summary>
        public override bool HandleKeyboard(NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            const uint RepeatMask = (0x40000000);

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
                            case System.Windows.Forms.Keys.Return:
                                {
                                    if (this.isComboOpen)
                                    {
                                        if (this.selectedIndex != this.focusedIndex)
                                        {
                                            this.selectedIndex = this.focusedIndex;
                                            this.RaiseChangedEvent(this, true);
                                        }
                                        this.isComboOpen = false;

                                        if (!this.Parent.IsUsingKeyboardInput)
                                            Dialog.ClearFocus();

                                        return true;
                                    }
                                    break;
                                }
                            case System.Windows.Forms.Keys.F4:
                                {
                                    // Filter out auto repeats
                                    if ((lParam.ToInt32() & RepeatMask) != 0)
                                        return true;

                                    this.isComboOpen = !this.isComboOpen;
                                    if (!this.isComboOpen)
                                    {
                                        this.RaiseChangedEvent(this, true);

                                        if (!this.Parent.IsUsingKeyboardInput)
                                            Dialog.ClearFocus();
                                    }

                                    return true;
                                }
                            case System.Windows.Forms.Keys.Left:
                            case System.Windows.Forms.Keys.Up:
                                {
                                    if (this.focusedIndex > 0)
                                    {
                                        this.focusedIndex--;
                                        this.selectedIndex = this.focusedIndex;
                                        if (!this.isComboOpen)
                                            this.RaiseChangedEvent(this, true);
                                    }
                                    return true;
                                }
                            case System.Windows.Forms.Keys.Right:
                            case System.Windows.Forms.Keys.Down:
                                {
                                    if (this.focusedIndex + 1 < (int)this.NumberItems)
                                    {
                                        this.focusedIndex++;
                                        this.selectedIndex = this.focusedIndex;
                                        if (!this.isComboOpen)
                                            this.RaiseChangedEvent(this, true);
                                    }
                                    return true;
                                }
                        }
                        break;
                    }
            }

            return false;
        }

        /// <summary>Called when the control should handle the mouse</summary>
        public override bool HandleMouse(NativeMethods.WindowMessage msg, System.Drawing.Point pt, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false; // Nothing to do

            // Let the scroll bar handle it first
            if (this.scrollbarControl.HandleMouse(msg, pt, wParam, lParam))
                return true;

            // Ok, scrollbar didn't handle it, move on
            switch(msg)
            {
                case NativeMethods.WindowMessage.MouseMove:
                    {
                        if (this.isComboOpen && this.dropDownRect.Contains(pt))
                        {
                            // Determine which item has been selected
                            for (int i = 0; i < this.itemList.Count; i++)
                            {
                                ComboBoxItem cbi = (ComboBoxItem)this.itemList[i];
                                if (cbi.IsItemVisible && cbi.ItemRect.Contains(pt))
                                {
                                    this.focusedIndex = i;
                                }
                            }
                            return true;
                        }
                        break;
                    }
                case NativeMethods.WindowMessage.LeftButtonDoubleClick:
                case NativeMethods.WindowMessage.LeftButtonDown:
                    {
                        if (this.ContainsPoint(pt))
                        {
                            // Pressed while inside the control
                            this.isPressed = true;
                            this.Parent.SampleFramework.Window.Capture = true;

                            if (!this.hasFocus)
                                Dialog.RequestFocus(this);

                            // Toggle dropdown
                            if (this.hasFocus)
                            {
                                this.isComboOpen = !this.isComboOpen;
                                if (!this.isComboOpen)
                                {
                                    if (!this.parentDialog.IsUsingKeyboardInput)
                                        Dialog.ClearFocus();
                                }
                            }

                            return true;
                        }

                        // Perhaps this click is within the dropdown
                        if (this.isComboOpen && this.dropDownRect.Contains(pt))
                        {
                            // Determine which item has been selected
                            for (int i = this.scrollbarControl.TrackPosition; i < this.itemList.Count; i++)
                            {
                                ComboBoxItem cbi = (ComboBoxItem)this.itemList[i];
                                if (cbi.IsItemVisible && cbi.ItemRect.Contains(pt))
                                {
                                    this.selectedIndex = this.focusedIndex = i;
                                    this.RaiseChangedEvent(this, true);

                                    this.isComboOpen = false;

                                    if (!this.parentDialog.IsUsingKeyboardInput)
                                        Dialog.ClearFocus();

                                    break;
                                }
                            }
                            return true;
                        }
                        // Mouse click not on main control or in dropdown, fire an event if needed
                        if (this.isComboOpen)
                        {
                            this.focusedIndex = this.selectedIndex;
                            this.RaiseChangedEvent(this, true);
                            this.isComboOpen = false;
                        }

                        // Make sure the control is no longer 'pressed'
                        this.isPressed = false;

                        // Release focus if appropriate
                        if (!this.parentDialog.IsUsingKeyboardInput)
                            Dialog.ClearFocus();

                        break;
                    }
                case NativeMethods.WindowMessage.LeftButtonUp:
                    {
                        if (this.isPressed && this.ContainsPoint(pt))
                        {
                            // Button click
                            this.isPressed = false;
                            this.Parent.SampleFramework.Window.Capture = false;
                            return true;
                        }
                        break;
                    }
                case NativeMethods.WindowMessage.MouseWheel:
                    {
                        int zdelta = (short)NativeMethods.HiWord((uint)wParam.ToInt32()) / Dialog.WheelDelta;
                        if (this.isComboOpen)
                        {
                            this.scrollbarControl.Scroll(-zdelta * System.Windows.Forms.SystemInformation.MouseWheelScrollLines);
                        }
                        else
                        {
                            if (zdelta > 0)
                            {
                                if (this.focusedIndex > 0)
                                {
                                    this.focusedIndex--;
                                    this.selectedIndex = this.focusedIndex;
                                    if (!this.isComboOpen)
                                    {
                                        this.RaiseChangedEvent(this, true);
                                    }
                                }
                            }
                            else
                            {
                                if (this.focusedIndex +1 < this.NumberItems)
                                {
                                    this.focusedIndex++;
                                    this.selectedIndex = this.focusedIndex;
                                    if (!this.isComboOpen)
                                    {
                                        this.RaiseChangedEvent(this, true);
                                    }
                                }
                            }
                        }
                        return true;
                    }
            }

            // Didn't handle it
            return false;
        }

        /// <summary>Called when the control should be rendered</summary>
        public override void Render(Device device, float elapsedTime)
        {
            ControlState state = ControlState.Normal;
            if (!this.isComboOpen)
                state = ControlState.Hidden;

            // Dropdown box
            Element e = this.elementList[ComboBox.DropdownLayer] as Element;
            
            // If we have not initialized the scroll bar page size,
            // do that now.
            if (!this.isScrollBarInit)
            {
                FontNode fNode = DialogResourceManager.GetGlobalInstance().GetFontNode((int)e.FontIndex);
                if ((fNode != null) && (fNode.Height > 0))
                    this.scrollbarControl.PageSize = (int)(this.dropDownTextRect.Height / fNode.Height);
                else
                    this.scrollbarControl.PageSize = this.dropDownTextRect.Height;

                this.isScrollBarInit = true;
            }

            if (this.isComboOpen)
                this.scrollbarControl.Render(device, elapsedTime);

            // Blend current color
            e.TextureColor.Blend(state, elapsedTime);
            e.FontColor.Blend(state, elapsedTime);
            this.parentDialog.DrawSprite(e, this.dropDownRect);

            // Selection outline
            Element selectionElement = this.elementList[ComboBox.SelectionLayer] as Element;
            selectionElement.TextureColor.Current = e.TextureColor.Current;
            selectionElement.FontColor.Current = selectionElement.FontColor.States[(int)ControlState.Normal];

            FontNode font = DialogResourceManager.GetGlobalInstance().GetFontNode((int)e.FontIndex);
            int currentY = this.dropDownTextRect.Top;
            int remainingHeight = this.dropDownTextRect.Height;

            for (int i = this.scrollbarControl.TrackPosition; i < this.itemList.Count; i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)this.itemList[i];

                // Make sure there's room left in the dropdown
                remainingHeight -= (int)font.Height;
                if (remainingHeight < 0)
                {
                    // Not visible, store that item
                    cbi.IsItemVisible = false;
                    this.itemList[i] = cbi; // Store this back in list
                    continue;
                }

                cbi.ItemRect = new System.Drawing.Rectangle(this.dropDownTextRect.Left, currentY,
                    this.dropDownTextRect.Width, (int)font.Height);
                cbi.IsItemVisible = true;
                currentY += (int)font.Height;
                this.itemList[i] = cbi; // Store this back in list

                if (this.isComboOpen)
                {
                    if (this.focusedIndex == i)
                    {
                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(
                            this.dropDownRect.Left, cbi.ItemRect.Top - 2, this.dropDownRect.Width,
                            cbi.ItemRect.Height + 4);
                        this.parentDialog.DrawSprite(selectionElement, rect);
                        this.parentDialog.DrawText(cbi.ItemText, selectionElement, cbi.ItemRect);
                    }
                    else
                    {
                        this.parentDialog.DrawText(cbi.ItemText, e, cbi.ItemRect);
                    }
                }
            }

            int offsetX = 0;
            int offsetY = 0;

            state = ControlState.Normal;
            if (this.IsVisible == false)
                state = ControlState.Hidden;
            else if (this.IsEnabled == false)
                state = ControlState.Disabled;
            else if (this.isPressed)
            {
                state = ControlState.Pressed;
                offsetX = 1;
                offsetY = 2;
            }
            else if (this.isMouseOver)
            {
                state = ControlState.MouseOver;
                offsetX = -1;
                offsetY = -2;
            }
            else if (this.hasFocus)
                state = ControlState.Focus;

            float blendRate = (state == ControlState.Pressed) ? 0.0f : 0.8f;

            // Button
            e = this.elementList[ComboBox.ComboButtonLayer] as Element;
            
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            
            System.Drawing.Rectangle windowRect = this.buttonRect;
            windowRect.Offset(offsetX, offsetY);
            // Draw sprite
            this.parentDialog.DrawSprite(e, windowRect);

            if (this.isComboOpen)
                state = ControlState.Pressed;

            // Main text box
            e = this.elementList[ComboBox.MainLayer] as Element;
            
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            e.FontColor.Blend(state, elapsedTime, blendRate);

            // Draw sprite
            this.parentDialog.DrawSprite(e, this.textRect);

            if (this.selectedIndex >= 0 && this.selectedIndex < this.itemList.Count)
            {
                try
                {
                    ComboBoxItem cbi = (ComboBoxItem)this.itemList[this.selectedIndex];
                    this.parentDialog.DrawText(cbi.ItemText, e, this.textRect);
                }
                catch {} // Ignore
            }

        }

        #region Item Controlling methods
        /// <summary>Adds an item to the combo box control</summary>
        public void AddItem(string text, object data)
        {
            if ((text == null) || (text.Length == 0))
                throw new ArgumentNullException("text", "You must pass in a valid item name when adding a new item.");

            // Create a new item and add it
            ComboBoxItem newitem = new ComboBoxItem();
            newitem.ItemText = text;
            newitem.ItemData = data;
            this.itemList.Add(newitem);

            // Update the scrollbar with the new range
            this.scrollbarControl.SetTrackRange(0, this.itemList.Count);

            // If this is the only item in the list, it should be selected
            if (this.NumberItems == 1)
            {
                this.selectedIndex = 0;
                this.focusedIndex = 0;
                this.RaiseChangedEvent(this, false);
            }
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
        }

        /// <summary>Removes all items from the control</summary>
        public void Clear()
        {
            // clear the list
            this.itemList.Clear();

            // Update scroll bar and index
            this.scrollbarControl.SetTrackRange(0, 1);
            this.focusedIndex = this.selectedIndex = -1;
        }

        /// <summary>Determines whether this control contains an item</summary>
        public bool ContainsItem(string text, int start)
        {
            return (this.FindItem(text, start) != -1);
        }
        /// <summary>Determines whether this control contains an item</summary>
        public bool ContainsItem(string text) { return this.ContainsItem(text, 0); }

        /// <summary>Gets the data for the selected item</summary>
        public object GetSelectedData()
        {
            if (this.selectedIndex < 0)
                return null; // Nothing selected

            ComboBoxItem cbi = (ComboBoxItem)this.itemList[this.selectedIndex];
            return cbi.ItemData;
        }

        /// <summary>Gets the selected item</summary>
        public ComboBoxItem GetSelectedItem()
        {
            if (this.selectedIndex < 0)
                throw new ArgumentOutOfRangeException("selectedIndex", "No item selected.");

            return (ComboBoxItem)this.itemList[this.selectedIndex];
        }

        /// <summary>Gets the data for an item</summary>
        public object GetItemData(string text)
        {
            int index = this.FindItem(text);
            if (index == -1)
                return null; // no item

            ComboBoxItem cbi = (ComboBoxItem)this.itemList[index];
            return cbi.ItemData;
        }

        /// <summary>Finds an item in the list and returns the index</summary>
        protected int FindItem(string text, int start)
        {
            if ((text == null) || (text.Length == 0))
                return -1;

            for(int i = start; i < this.itemList.Count; i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)this.itemList[i];
                if (string.Compare(cbi.ItemText, text, true) == 0)
                {
                    return i;
                }
            }

            // Never found it
            return -1;
        }
        /// <summary>Finds an item in the list and returns the index</summary>
        protected int FindItem(string text) { return this.FindItem(text, 0); }

        /// <summary>Sets the selected item by index</summary>
        public void SetSelected(int index)
        {
            if (index >= this.NumberItems)
                throw new ArgumentOutOfRangeException("index", "There are not enough items in the list to select this index.");

            this.focusedIndex = this.selectedIndex = index;
            this.RaiseChangedEvent(this, false);
        }

        /// <summary>Sets the selected item by text</summary>
        public void SetSelected(string text)
        {
            if ((text == null) || (text.Length == 0))
                throw new ArgumentNullException("text", "You must pass in a valid item name when adding a new item.");

            int index = this.FindItem(text);
            if (index == -1)
                throw new InvalidOperationException("This item could not be found.");

            this.focusedIndex = this.selectedIndex = index;
            this.RaiseChangedEvent(this, false);
        }

        /// <summary>Sets the selected item by data</summary>
        public void SetSelectedByData(object data)
        {
            for (int index = 0; index < this.itemList.Count; index++)
            {
                ComboBoxItem cbi = (ComboBoxItem)this.itemList[index];
                if (cbi.ItemData.ToString().Equals(data.ToString()))
                {
                    this.focusedIndex = this.selectedIndex = index;
                    this.RaiseChangedEvent(this, false);
                }
            }

            // Could not find this item.  Uncomment line below for debug information
            //System.Diagnostics.Debugger.Log(9,string.Empty, "Could not find an object with this data.\r\n");
        }

        #endregion
    }
}