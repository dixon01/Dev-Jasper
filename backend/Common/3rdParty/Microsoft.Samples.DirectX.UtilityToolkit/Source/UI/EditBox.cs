namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>A basic edit box</summary>
    public class EditBox : Control
    {
        #region Element layers
        public const int TextLayer = 0;
        public const int TopLeftBorder = 1;
        public const int TopBorder = 2;
        public const int TopRightBorder = 3;
        public const int LeftBorder = 4;
        public const int RightBorder = 5;
        public const int LowerLeftBorder = 6;
        public const int LowerBorder = 7;
        public const int LowerRightBorder = 8;
        #endregion

        #region Event code
        public event EventHandler Changed;
        public event EventHandler Enter;
        /// <summary>Raises the changed event</summary>
        protected void RaiseChangedEvent(EditBox sender, bool wasTriggeredByUser)
        {
            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!this.Parent.IsUsingNonUserEvents && !wasTriggeredByUser)
                return;

            if (this.Changed != null)
                this.Changed(sender, EventArgs.Empty);
        }
        /// <summary>Raises the Enter event</summary>
        protected void RaiseEnterEvent(EditBox sender, bool wasTriggeredByUser)
        {
            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!this.Parent.IsUsingNonUserEvents && !wasTriggeredByUser)
                return;

            if (this.Enter != null)
                this.Enter(sender, EventArgs.Empty);
        }
        #endregion

        #region Class Data
        protected System.Windows.Forms.RichTextBox textData; // Text data
        protected int border; // Border of the window
        protected int spacing; // Spacing between the text and the edge of border
        protected System.Drawing.Rectangle textRect; // Bounding rectangle for the text
        protected System.Drawing.Rectangle[] elementRects = new System.Drawing.Rectangle[9];
        protected double blinkTime; // Caret blink time in milliseconds
        protected double lastBlink; // Last timestamp of caret blink
        protected bool isCaretOn; // Flag to indicate whether caret is currently visible
        protected int caretPosition; // Caret position, in characters
        protected bool isInsertMode; // If true, control is in insert mode. Else, overwrite mode.
        protected int firstVisible;  // First visible character in the edit control
        protected ColorValue textColor; // Text color
        protected ColorValue selectedTextColor; // Selected Text color
        protected ColorValue selectedBackColor; // Selected background color
        protected ColorValue caretColor; // Caret color

        // Mouse-specific
        protected bool isMouseDragging; // True to indicate the drag is in progress

        protected static bool isHidingCaret; // If true, we don't render the caret.
        
        #endregion

        #region Simple overrides/properties/methods
        /// <summary>Can the edit box have focus</summary>
        public override bool CanHaveFocus { get { return (this.IsVisible && this.IsEnabled); } }
        /// <summary>Update the spacing</summary>
        public void SetSpacing(int space) { this.spacing = space; this.UpdateRectangles(); }
        /// <summary>Update the border</summary>
        public void SetBorderWidth(int b) { this.border = b; this.UpdateRectangles(); }
        /// <summary>Update the text color</summary>
        public void SetTextColor(ColorValue color) { this.textColor = color; }
        /// <summary>Update the text selected color</summary>
        public void SetSelectedTextColor(ColorValue color) { this.selectedTextColor = color; }
        /// <summary>Update the selected background color</summary>
        public void SetSelectedBackColor(ColorValue color) { this.selectedBackColor = color; }
        /// <summary>Update the caret color</summary>
        public void SetCaretColor(ColorValue color) { this.caretColor = color; }

        /// <summary>Get or sets the text</summary>
        public string Text { get { return this.textData.Text; } set { this.SetText(value, false); } }
        /// <summary>Gets a copy of the text</summary>
        public string GetTextCopy() { return string.Copy(this.textData.Text); }
        #endregion

        /// <summary>Creates a new edit box control</summary>
        public EditBox(Dialog parent) : base(parent)
        {
            this.controlType = ControlType.EditBox;
            this.parentDialog = parent;

            this.border = 5; // Default border
            this.spacing = 4; // default spacing
            this.isCaretOn = true;

            this.textData = new System.Windows.Forms.RichTextBox();
            // Create the control
            this.textData.Visible = true;
            this.textData.Font = new System.Drawing.Font("Arial", 8.0f);
            this.textData.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.textData.Multiline = false;
            this.textData.Text = string.Empty;
            this.textData.MaxLength = ushort.MaxValue; // 65k characters should be plenty
            this.textData.WordWrap = false;
            // Now create the control
            this.textData.CreateControl();

            isHidingCaret = false;
            this.firstVisible = 0;
            this.blinkTime = NativeMethods.GetCaretBlinkTime() * 0.001f;
            this.lastBlink = FrameworkTimer.GetAbsoluteTime();
            this.textColor = new ColorValue(0.06f, 0.06f, 0.06f, 1.0f);
            this.selectedTextColor = new ColorValue(1.0f, 1.0f, 1.0f, 1.0f);
            this.selectedBackColor = new ColorValue(0.15f, 0.196f, 0.36f, 1.0f);
            this.caretColor = new ColorValue(0, 0, 0, 1.0f);
            this.caretPosition = this.textData.SelectionStart = 0;
            this.isInsertMode = true;
            this.isMouseDragging = false;
        }

        /// <summary>Set the caret to a character position, and adjust the scrolling if necessary</summary>
        protected void PlaceCaret(int pos)
        {
            // Store caret position
            this.caretPosition = pos;

            // First find the first visible char
            for (int i = 0; i < this.textData.Text.Length; i++)
            {
                System.Drawing.Point p = this.textData.GetPositionFromCharIndex(i);
                if (p.X >= 0) 
                {
                    this.firstVisible = i; // This is the first visible character
                    break;
                }
            }

            // if the new position is smaller than the first visible char 
            // we'll need to scroll
            if (this.firstVisible > this.caretPosition)
                this.firstVisible = this.caretPosition;
        }

        /// <summary>Clears the edit box</summary>
        public void Clear()
        {
            this.textData.Text = string.Empty;
            this.PlaceCaret(0);
            this.textData.SelectionStart = 0;
        }
        /// <summary>Sets the text for the control</summary>
        public void SetText(string text, bool selected)
        {
            if (text == null)
                text = string.Empty;

            this.textData.Text = text;
            this.textData.SelectionStart = text.Length;
            // Move the care to the end of the text
            this.PlaceCaret(text.Length);
            this.textData.SelectionStart = (selected) ? 0 : this.caretPosition;
            this.FocusText();
        }
        /// <summary>Deletes the text that is currently selected</summary>
        protected void DeleteSelectionText()
        {
            int first = Math.Min(this.caretPosition, this.textData.SelectionStart);
            int last = Math.Max(this.caretPosition, this.textData.SelectionStart);
            // Update caret and selection
            this.PlaceCaret(first);
            // Remove the characters
            this.textData.Text = this.textData.Text.Remove(first, (last-first));
            this.textData.SelectionStart = this.caretPosition;
            this.FocusText();
        }
        /// <summary>Updates the rectangles used by the control</summary>
        protected override void UpdateRectangles()
        {
            // Get the bounding box first
            base.UpdateRectangles ();

            // Update text rect
            this.textRect = this.boundingBox;
            // First inflate by border to compute render rects
            this.textRect.Inflate(-this.border, -this.border);
            
            // Update the render rectangles
            this.elementRects[0] = this.textRect;
            this.elementRects[1] = new System.Drawing.Rectangle(this.boundingBox.Left, this.boundingBox.Top, (this.textRect.Left - this.boundingBox.Left), (this.textRect.Top - this.boundingBox.Top));
            this.elementRects[2] = new System.Drawing.Rectangle(this.textRect.Left, this.boundingBox.Top, this.textRect.Width, (this.textRect.Top - this.boundingBox.Top));
            this.elementRects[3] = new System.Drawing.Rectangle(this.textRect.Right, this.boundingBox.Top, (this.boundingBox.Right - this.textRect.Right), (this.textRect.Top - this.boundingBox.Top));
            this.elementRects[4] = new System.Drawing.Rectangle(this.boundingBox.Left, this.textRect.Top, (this.textRect.Left - this.boundingBox.Left), this.textRect.Height);
            this.elementRects[5] = new System.Drawing.Rectangle(this.textRect.Right, this.textRect.Top, (this.boundingBox.Right - this.textRect.Right), this.textRect.Height);
            this.elementRects[6] = new System.Drawing.Rectangle(this.boundingBox.Left, this.textRect.Bottom, (this.textRect.Left - this.boundingBox.Left), (this.boundingBox.Bottom - this.textRect.Bottom));
            this.elementRects[7] = new System.Drawing.Rectangle(this.textRect.Left, this.textRect.Bottom, this.textRect.Width, (this.boundingBox.Bottom - this.textRect.Bottom));
            this.elementRects[8] = new System.Drawing.Rectangle(this.textRect.Right, this.textRect.Bottom, (this.boundingBox.Right - this.textRect.Right), (this.boundingBox.Bottom - this.textRect.Bottom));            

            // Inflate further by spacing
            this.textRect.Inflate(-this.spacing, -this.spacing);

            // Make the underlying rich text box the same size
            this.textData.Size = this.textRect.Size;
        }

        /// <summary>Copy the selected text to the clipboard</summary>
        protected void CopyToClipboard()
        {
            // Copy the selection text to the clipboard
            if (this.caretPosition != this.textData.SelectionStart)
            {
                int first = Math.Min(this.caretPosition, this.textData.SelectionStart);
                int last = Math.Max(this.caretPosition, this.textData.SelectionStart);
                // Set the text to the clipboard
                System.Windows.Forms.Clipboard.SetDataObject(this.textData.Text.Substring(first, (last-first)));
            }

        }
        /// <summary>Paste the clipboard data to the control</summary>
        protected void PasteFromClipboard()
        {
            // Get the clipboard data
            System.Windows.Forms.IDataObject clipData = System.Windows.Forms.Clipboard.GetDataObject();
            // Does the clipboard have string data?
            if (clipData.GetDataPresent(System.Windows.Forms.DataFormats.StringFormat))
            {
                // Yes, get that data
                string clipString = clipData.GetData(System.Windows.Forms.DataFormats.StringFormat) as string;
                // find any new lines, remove everything after that
                int index;
                if ((index = clipString.IndexOf("\n")) > 0)
                {
                    clipString = clipString.Substring(0, index-1);
                }

                // Insert that into the text data
                this.textData.Text = this.textData.Text.Insert(this.caretPosition, clipString);
                this.caretPosition += clipString.Length;
                this.textData.SelectionStart = this.caretPosition;
                this.FocusText();
            }
        }
        /// <summary>Reset's the caret blink time</summary>
        protected void ResetCaretBlink()
        {
            this.isCaretOn = true;
            this.lastBlink = FrameworkTimer.GetAbsoluteTime();
        }

        /// <summary>Update the caret when focus is in</summary>
        public override void OnFocusIn()
        {
            base.OnFocusIn();
            this.ResetCaretBlink();
        }

        /// <summary>Updates focus to the backing rich textbox so it updates it's state</summary>
        private void FocusText()
        {
            // Because of a design issue with the rich text box control that is used as 
            // the backing store for this control, the 'scrolling' mechanism built into
            // the control will only work if the control has focus.  Setting focus to the 
            // control here would work, but would cause a bad 'flicker' of the application.

            // Therefore, the automatic horizontal scrolling is turned off by default.  To 
            // enable it turn this define on.
#if (SCROLL_CORRECTLY)
            NativeMethods.SetFocus(textData.Handle);
            NativeMethods.SetFocus(Parent.SampleFramework.Window);
#endif
        }

        /// <summary>Handle keyboard input to the edit box</summary>
        public override bool HandleKeyboard(Microsoft.Samples.DirectX.UtilityToolkit.NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            // Default to not handling the message
            bool isHandled = false;
            if (msg == NativeMethods.WindowMessage.KeyDown)
            {
                switch((System.Windows.Forms.Keys)wParam.ToInt32())
                {
                    case System.Windows.Forms.Keys.End:
                    case System.Windows.Forms.Keys.Home:
                        // Move the caret
                        if (wParam.ToInt32() == (int)System.Windows.Forms.Keys.End)
                            this.PlaceCaret(this.textData.Text.Length);
                        else
                            this.PlaceCaret(0);
                        if (!NativeMethods.IsKeyDown(System.Windows.Forms.Keys.ShiftKey))
                        {
                            // Shift is not down. Update selection start along with caret
                            this.textData.SelectionStart = this.caretPosition;
                            this.FocusText();
                        }

                        this.ResetCaretBlink();
                        isHandled = true;
                        break;
                    case System.Windows.Forms.Keys.Insert:
                        if (NativeMethods.IsKeyDown(System.Windows.Forms.Keys.ControlKey))
                        {
                            // Control insert -> Copy to clipboard
                            this.CopyToClipboard();
                        }
                        else if (NativeMethods.IsKeyDown(System.Windows.Forms.Keys.ShiftKey))
                        {
                            // Shift insert -> Paste from clipboard
                            this.PasteFromClipboard();
                        }
                        else
                        {
                            // Toggle insert mode
                            this.isInsertMode = !this.isInsertMode;
                        }
                        break;
                    case System.Windows.Forms.Keys.Delete:
                        // Check to see if there is a text selection
                        if (this.caretPosition != this.textData.SelectionStart)
                        {
                            this.DeleteSelectionText();
                            this.RaiseChangedEvent(this, true);
                        }
                        else
                        {
                            if (this.caretPosition < this.textData.Text.Length)
                            {
                                // Deleting one character
                                this.textData.Text = this.textData.Text.Remove(this.caretPosition, 1);
                                this.RaiseChangedEvent(this, true);
                            }
                        }
                        this.ResetCaretBlink();
                        isHandled = true;
                        break;

                    case System.Windows.Forms.Keys.Left:
                        if (NativeMethods.IsKeyDown(System.Windows.Forms.Keys.ControlKey))
                        {
                            // Control is down. Move the caret to a new item
                            // instead of a character.
                        }
                        else if (this.caretPosition > 0)
                            this.PlaceCaret(this.caretPosition - 1); // Move one to the left

                        if (!NativeMethods.IsKeyDown(System.Windows.Forms.Keys.ShiftKey))
                        {
                            // Shift is not down. Update selection
                            // start along with the caret.
                            this.textData.SelectionStart = this.caretPosition;
                            this.FocusText();
                        }
                        this.ResetCaretBlink();
                        isHandled = true;
                        break;

                    case System.Windows.Forms.Keys.Right:
                        if (NativeMethods.IsKeyDown(System.Windows.Forms.Keys.ControlKey))
                        {
                            // Control is down. Move the caret to a new item
                            // instead of a character.
                        }
                        else if (this.caretPosition < this.textData.Text.Length)
                            this.PlaceCaret(this.caretPosition + 1); // Move one to the left
                        if (!NativeMethods.IsKeyDown(System.Windows.Forms.Keys.ShiftKey))
                        {
                            // Shift is not down. Update selection
                            // start along with the caret.
                            this.textData.SelectionStart = this.caretPosition;
                            this.FocusText();
                        }
                        this.ResetCaretBlink();
                        isHandled = true;
                        break;

                    case System.Windows.Forms.Keys.Up:
                    case System.Windows.Forms.Keys.Down:
                        // Trap up and down arrows so that the dialog
                        // does not switch focus to another control.
                        isHandled = true;
                        break;

                    default:
                        // Let the application handle escape
                        isHandled = ((System.Windows.Forms.Keys)wParam.ToInt32()) == System.Windows.Forms.Keys.Escape;
                        break;
                }
            }

            return isHandled;
        }

        /// <summary>Handle mouse messages</summary>
        public override bool HandleMouse(NativeMethods.WindowMessage msg, System.Drawing.Point pt, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            // We need a new point
            System.Drawing.Point p = pt;
            p.X -= this.textRect.Left;
            p.Y -= this.textRect.Top;

            switch(msg)
            {
                case NativeMethods.WindowMessage.LeftButtonDown:
                case NativeMethods.WindowMessage.LeftButtonDoubleClick:
                    // Get focus first
                    if (!this.hasFocus)
                        Dialog.RequestFocus(this);

                    if (!this.ContainsPoint(pt))
                        return false;

                    this.isMouseDragging = true;
                    this.Parent.SampleFramework.Window.Capture = true;
                    // Determine the character corresponding to the coordinates
                    int index = this.textData.GetCharIndexFromPosition(p);

                    System.Drawing.Point startPosition = this.textData.GetPositionFromCharIndex(index);

                    if (p.X > startPosition.X && index < this.textData.Text.Length)
                        this.PlaceCaret(index + 1);
                    else
                        this.PlaceCaret(index);
                    
                    this.textData.SelectionStart = this.caretPosition;
                    this.FocusText();
                    this.ResetCaretBlink();
                    return true;

                case NativeMethods.WindowMessage.LeftButtonUp:
                    this.Parent.SampleFramework.Window.Capture = false;
                    this.isMouseDragging = false;
                    break;
                case NativeMethods.WindowMessage.MouseMove:
                    if (this.isMouseDragging)
                    {
                        // Determine the character corresponding to the coordinates
                        int dragIndex = this.textData.GetCharIndexFromPosition(p);

                        if (dragIndex < this.textData.Text.Length)
                            this.PlaceCaret(dragIndex + 1);
                        else
                            this.PlaceCaret(dragIndex);
                    }
                    break;
            }
            return false;
        }

        /// <summary>Handle all other messages</summary>
        public override bool MsgProc(IntPtr hWnd, NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            if (msg == NativeMethods.WindowMessage.Character)
            {
                int charKey = wParam.ToInt32();
                switch(charKey)
                {
                    case (int)System.Windows.Forms.Keys.Back:
                        {
                            // If there's a selection, treat this
                            // like a delete key.
                            if (this.caretPosition != this.textData.SelectionStart)
                            {
                                this.DeleteSelectionText();
                                this.RaiseChangedEvent(this, true);
                            }
                            else if (this.caretPosition > 0)
                            {
                                // Move the caret and delete the char
                                this.textData.Text = this.textData.Text.Remove(this.caretPosition - 1, 1);
                                this.PlaceCaret(this.caretPosition - 1);
                                this.textData.SelectionStart = this.caretPosition;
                                this.FocusText();
                                this.RaiseChangedEvent(this, true);
                            }

                            this.ResetCaretBlink();
                            break;
                        }
                    case 24: // Ctrl-X Cut
                    case (int)System.Windows.Forms.Keys.Cancel: // Ctrl-C Copy
                        {
                            this.CopyToClipboard();

                            // If the key is Ctrl-X, delete the selection too.
                            if (charKey == 24)
                            {
                                this.DeleteSelectionText();
                                this.RaiseChangedEvent(this, true);
                            }

                            break;
                        }

                        // Ctrl-V Paste
                    case 22:
                        {
                            this.PasteFromClipboard();
                            this.RaiseChangedEvent(this, true);
                            break;
                        }
                    case (int)System.Windows.Forms.Keys.Return:
                        // Invoke the event when the user presses Enter.
                        this.RaiseEnterEvent(this, true);
                        break;

                        // Ctrl-A Select All
                    case 1:
                        {
                            if (this.textData.SelectionStart == this.caretPosition)
                            {
                                this.textData.SelectionStart = 0;
                                this.PlaceCaret(this.textData.Text.Length);
                            }
                            break;
                        }

                        // Junk characters we don't want in the string
                    case 26:  // Ctrl Z
                    case 2:   // Ctrl B
                    case 14:  // Ctrl N
                    case 19:  // Ctrl S
                    case 4:   // Ctrl D
                    case 6:   // Ctrl F
                    case 7:   // Ctrl G
                    case 10:  // Ctrl J
                    case 11:  // Ctrl K
                    case 12:  // Ctrl L
                    case 17:  // Ctrl Q
                    case 23:  // Ctrl W
                    case 5:   // Ctrl E
                    case 18:  // Ctrl R
                    case 20:  // Ctrl T
                    case 25:  // Ctrl Y
                    case 21:  // Ctrl U
                    case 9:   // Ctrl I
                    case 15:  // Ctrl O
                    case 16:  // Ctrl P
                    case 27:  // Ctrl [
                    case 29:  // Ctrl ]
                    case 28:  // Ctrl \ 
                        break;
                    
                    default:
                        {
                            // If there's a selection and the user
                            // starts to type, the selection should
                            // be deleted.
                            if (this.caretPosition != this.textData.SelectionStart)
                            {
                                this.DeleteSelectionText();
                            }
                            // If we are in overwrite mode and there is already
                            // a char at the caret's position, simply replace it.
                            // Otherwise, we insert the char as normal.
                            if (!this.isInsertMode && this.caretPosition < this.textData.Text.Length)
                            {
                                // This isn't the most efficient way to do this, but it's simple
                                // and shows the correct behavior
                                char[] charData = this.textData.Text.ToCharArray();
                                charData[this.caretPosition] = (char)wParam.ToInt32();
                                this.textData.Text = new string(charData);
                            }
                            else
                            {
                                // Insert the char
                                char c = (char)wParam.ToInt32();
                                this.textData.Text = this.textData.Text.Insert(this.caretPosition, c.ToString());
                            }

                            // Move the caret and selection position now
                            this.PlaceCaret(this.caretPosition + 1);
                            this.textData.SelectionStart = this.caretPosition;
                            this.FocusText();

                            this.ResetCaretBlink();
                            this.RaiseChangedEvent(this, true);
                            break;
                        }
                }
            }
            return false;
        }


        /// <summary>Render the control</summary>
        public override void Render(Device device, float elapsedTime)
        {
            if (!this.IsVisible)
                return; // Nothing to render

            // Render the control graphics
            for (int i = 0; i <= LowerRightBorder; ++i)
            {
                Element e = this.elementList[i] as Element;
                e.TextureColor.Blend(ControlState.Normal,elapsedTime);
                this.parentDialog.DrawSprite(e, this.elementRects[i]);
            }
            //
            // Compute the X coordinates of the first visible character.
            //
            int xFirst = this.textData.GetPositionFromCharIndex(this.firstVisible).X;
            int xCaret = this.textData.GetPositionFromCharIndex(this.caretPosition).X;
            int xSel;

            if (this.caretPosition != this.textData.SelectionStart)
                xSel = this.textData.GetPositionFromCharIndex(this.textData.SelectionStart).X;
            else
                xSel = xCaret;

            // Render the selection rectangle
            System.Drawing.Rectangle selRect = System.Drawing.Rectangle.Empty;
            if (this.caretPosition != this.textData.SelectionStart)
            {
                int selLeft = xCaret, selRight = xSel;
                // Swap if left is beigger than right
                if (selLeft > selRight)
                {
                    int temp = selLeft;
                    selLeft = selRight;
                    selRight = temp;
                }
                selRect = System.Drawing.Rectangle.FromLTRB(
                    selLeft, this.textRect.Top, selRight, this.textRect.Bottom);
                selRect.Offset(this.textRect.Left - xFirst, 0);
                selRect.Intersect(this.textRect);
                this.Parent.DrawRectangle(selRect, this.selectedBackColor);
            }

            // Render the text
            Element textElement = this.elementList[TextLayer] as Element;
            textElement.FontColor.Current = this.textColor;
            this.parentDialog.DrawText(this.textData.Text.Substring(this.firstVisible), textElement, this.textRect);
            
            // Render the selected text
            if (this.caretPosition != this.textData.SelectionStart)
            {
                int firstToRender = Math.Max(this.firstVisible, Math.Min(this.textData.SelectionStart, this.caretPosition));
                int numToRender = Math.Max(this.textData.SelectionStart, this.caretPosition) - firstToRender;
                textElement.FontColor.Current = this.selectedTextColor;
                this.parentDialog.DrawText(this.textData.Text.Substring(firstToRender, numToRender), textElement, selRect);
            }

            //
            // Blink the caret
            //
            if(FrameworkTimer.GetAbsoluteTime() - this.lastBlink >= this.blinkTime)
            {
                this.isCaretOn = !this.isCaretOn;
                this.lastBlink = FrameworkTimer.GetAbsoluteTime();
            }

            //
            // Render the caret if this control has the focus
            //
            if( this.hasFocus && this.isCaretOn && !isHidingCaret )
            {
                // Start the rectangle with insert mode caret
                System.Drawing.Rectangle caretRect = this.textRect;
                caretRect.Width = 2;
                caretRect.Location = new System.Drawing.Point(
                    caretRect.Left - xFirst + xCaret -1, 
                    caretRect.Top);
                
                // If we are in overwrite mode, adjust the caret rectangle
                // to fill the entire character.
                if (!this.isInsertMode)
                {
                    // Obtain the X coord of the current character
                    caretRect.Width = 4;
                }

                this.parentDialog.DrawRectangle(caretRect, this.caretColor);
            }

        }
    }
}