namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// A scroll bar control
    /// </summary>
    public class ScrollBar : Control
    {
        public const int TrackLayer = 0;
        public const int UpButtonLayer = 1;
        public const int DownButtonLayer = 2;
        public const int ThumbLayer = 3;
        protected const int MinimumThumbSize = 8;
        #region Instance Data
        protected bool showingThumb;
        protected System.Drawing.Rectangle upButtonRect;
        protected System.Drawing.Rectangle downButtonRect;
        protected System.Drawing.Rectangle trackRect;
        protected System.Drawing.Rectangle thumbRect;
        protected int position; // Position of the first displayed item
        protected int pageSize; // How many items are displayable in one page
        protected int start; // First item
        protected int end; // The index after the last item
        private int thumbOffsetY;
        private bool isDragging;
        #endregion

        /// <summary>
        /// Creates a new instance of the scroll bar class
        /// </summary>
        public ScrollBar(Dialog parent) : base(parent)
        {
            // Store parent and control type
            this.controlType = ControlType.Scrollbar;
            this.parentDialog = parent;

            // Set default properties
            this.showingThumb = true;
            this.upButtonRect = System.Drawing.Rectangle.Empty;
            this.downButtonRect = System.Drawing.Rectangle.Empty;
            this.trackRect = System.Drawing.Rectangle.Empty;
            this.thumbRect = System.Drawing.Rectangle.Empty;

            this.position = 0;
            this.pageSize = 1;
            this.start = 0;
            this.end = 1;
        }

        /// <summary>
        /// Update all of the rectangles
        /// </summary>
        protected override void UpdateRectangles()
        {
            // Get the bounding box first
            base.UpdateRectangles();

            // Make sure buttons are square
            this.upButtonRect = new System.Drawing.Rectangle(this.boundingBox.Location,
                new System.Drawing.Size(this.boundingBox.Width, this.boundingBox.Width));

            this.downButtonRect = new System.Drawing.Rectangle(this.boundingBox.Left, this.boundingBox.Bottom - this.boundingBox.Width,
                this.boundingBox.Width, this.boundingBox.Width);

            this.trackRect = new System.Drawing.Rectangle(this.upButtonRect.Left, this.upButtonRect.Bottom, 
                this.upButtonRect.Width, this.downButtonRect.Top - this.upButtonRect.Bottom);

            this.thumbRect = this.upButtonRect;

            this.UpdateThumbRectangle();
        }

        /// <summary>
        /// Position of the track
        /// </summary>
        public int TrackPosition
        {
            get { return this.position; }
            set { this.position = value; this.Cap(); this.UpdateThumbRectangle(); }
        }
        /// <summary>
        /// Size of a 'page'
        /// </summary>
        public int PageSize
        {
            get { return this.pageSize; }
            set { this.pageSize = value; this.Cap(); this.UpdateThumbRectangle(); }
        }

        /// <summary>Clips position at boundaries</summary>
        protected void Cap()
        {
            if (this.position < this.start || this.end - this.start <= this.pageSize)
            {
                this.position = this.start;
            }
            else if (this.position + this.pageSize > this.end)
                this.position = this.end - this.pageSize;
        }

        /// <summary>Compute the dimension of the scroll thumb</summary>
        protected void UpdateThumbRectangle()
        {
            if (this.end - this.start > this.pageSize)
            {
                int thumbHeight = Math.Max(this.trackRect.Height * this.pageSize / (this.end-this.start), MinimumThumbSize);
                int maxPosition = this.end - this.start - this.pageSize;
                this.thumbRect.Location = new System.Drawing.Point(this.thumbRect.Left,
                    this.trackRect.Top + (this.position - this.start) * (this.trackRect.Height - thumbHeight) / maxPosition);
                this.thumbRect.Size = new System.Drawing.Size(this.thumbRect.Width, thumbHeight);
                this.showingThumb = true;
            }
            else
            {
                // No content to scroll
                this.thumbRect.Height = 0;
                this.showingThumb = false;
            }
        }

        /// <summary>Scrolls by delta items.  A positive value scrolls down, while a negative scrolls down</summary>
        public void Scroll(int delta)
        {
            // Perform scroll
            this.position += delta;
            // Cap position
            this.Cap();
            // Update thumb rectangle
            this.UpdateThumbRectangle();
        }

        /// <summary>Shows an item</summary>
        public void ShowItem(int index)
        {
            // Cap the index
            if (index < 0)
                index = 0;

            if (index >= this.end)
                index = this.end - 1;

            // Adjust the position to show this item
            if (this.position > index)
                this.position = index;
            else if (this.position + this.pageSize <= index)
                this.position = index - this.pageSize + 1;

            // Update thumbs again
            this.UpdateThumbRectangle();
        }

        /// <summary>Sets the track range</summary>
        public void SetTrackRange(int startRange, int endRange)
        {
            this.start = startRange; this.end = endRange;
            this.Cap();
            this.UpdateThumbRectangle();
        }

        /// <summary>Render the scroll bar control</summary>
        public override void Render(Device device, float elapsedTime)
        {
            ControlState state = ControlState.Normal;
            if (this.IsVisible == false)
                state = ControlState.Hidden;
            else if ( (this.IsEnabled == false) || (this.showingThumb == false) )
                state = ControlState.Disabled;
            else if (this.isMouseOver)
                state = ControlState.MouseOver;
            else if (this.hasFocus)
                state = ControlState.Focus;

            float blendRate = (state == ControlState.Pressed) ? 0.0f : 0.8f;

            // Background track layer
            Element e = this.elementList[ScrollBar.TrackLayer] as Element;
            
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            this.parentDialog.DrawSprite(e, this.trackRect);

            // Up arrow
            e = this.elementList[ScrollBar.UpButtonLayer] as Element;
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            this.parentDialog.DrawSprite(e, this.upButtonRect);

            // Down arrow
            e = this.elementList[ScrollBar.DownButtonLayer] as Element;
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            this.parentDialog.DrawSprite(e, this.downButtonRect);

            // Thumb button
            e = this.elementList[ScrollBar.ThumbLayer] as Element;
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            this.parentDialog.DrawSprite(e, this.thumbRect);
        }

        /// <summary>Stores data for a combo box item</summary>
        public override bool HandleMouse(NativeMethods.WindowMessage msg, System.Drawing.Point pt, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            switch(msg)
            {
                case NativeMethods.WindowMessage.LeftButtonDoubleClick:
                case NativeMethods.WindowMessage.LeftButtonDown:
                    {
                        this.Parent.SampleFramework.Window.Capture = true;

                        // Check for on up button
                        if (this.upButtonRect.Contains(pt))
                        {
                            if (this.position > this.start)
                                --this.position;
                            this.UpdateThumbRectangle();
                            return true;
                        }

                        // Check for on down button
                        if (this.downButtonRect.Contains(pt))
                        {
                            if (this.position + this.pageSize < this.end)
                                ++this.position;
                            this.UpdateThumbRectangle();
                            return true;
                        }

                        // Check for click on thumb
                        if (this.thumbRect.Contains(pt))
                        {
                            this.isDragging = true;
                            this.thumbOffsetY = pt.Y - this.thumbRect.Top;
                            return true;
                        }

                        // check for click on track
                        if (this.thumbRect.Left <= pt.X &&
                            this.thumbRect.Right > pt.X)
                        {
                            if (this.thumbRect.Top > pt.Y &&
                                this.trackRect.Top <= pt.Y)
                            {
                                this.Scroll(-(this.pageSize-1));
                                return true;
                            }
                            else if (this.thumbRect.Bottom <= pt.Y &&
                                     this.trackRect.Bottom > pt.Y)
                            {
                                this.Scroll(this.pageSize-1);
                                return true;
                            }
                        }

                        break;
                    }
                case NativeMethods.WindowMessage.LeftButtonUp:
                    {
                        this.isDragging = false;
                        this.Parent.SampleFramework.Window.Capture = false;
                        this.UpdateThumbRectangle();
                        break;
                    }

                case NativeMethods.WindowMessage.MouseMove:
                    {
                        if (this.isDragging)
                        {
                            // Calculate new bottom and top of thumb rect
                            int bottom = this.thumbRect.Bottom + (pt.Y - this.thumbOffsetY - this.thumbRect.Top);
                            int top = pt.Y - this.thumbOffsetY;
                            this.thumbRect = new System.Drawing.Rectangle(this.thumbRect.Left, top, this.thumbRect.Width, bottom - top);
                            if (this.thumbRect.Top < this.trackRect.Top)
                                this.thumbRect.Offset(0, this.trackRect.Top - this.thumbRect.Top);
                            else if (this.thumbRect.Bottom > this.trackRect.Bottom)
                                this.thumbRect.Offset(0, this.trackRect.Bottom - this.thumbRect.Bottom);

                            // Compute first item index based on thumb position
                            int maxFirstItem = this.end - this.start - this.pageSize; // Largest possible index for first item
                            int maxThumb = this.trackRect.Height - this.thumbRect.Height; // Largest possible thumb position

                            this.position = this.start + (this.thumbRect.Top - this.trackRect.Top +
                                                maxThumb / (maxFirstItem * 2) ) * // Shift by half a row to avoid last row covered
                                       maxFirstItem / maxThumb;

                            return true;
                        }
                        break;
                    }
            }

            // Was not handled
            return false;
        }

    }
}