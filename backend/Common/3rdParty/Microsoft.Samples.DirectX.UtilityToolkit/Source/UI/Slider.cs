namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>Slider control</summary>
    public class Slider : Control
    {
        public const int TrackLayer = 0;
        public const int ButtonLayer = 1;
        #region Instance Data
        public event EventHandler ValueChanged;
        protected int currentValue; 
        protected int maxValue;
        protected int minValue;
        
        protected int dragX; // Mouse position at the start of the drag
        protected int dragOffset; // Drag offset from the center of the button
        protected int buttonX;

        protected bool isPressed;
        protected System.Drawing.Rectangle buttonRect;

        /// <summary>Slider's can always have focus</summary>
        public override bool CanHaveFocus { get { return true; }}

        /// <summary>Current value of the slider</summary>
        protected void RaiseValueChanged(Slider sender, bool wasTriggeredByUser)
        {
            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!this.Parent.IsUsingNonUserEvents && !wasTriggeredByUser)
                return;

            if (this.ValueChanged != null)
                this.ValueChanged(sender, EventArgs.Empty);
        }
        /// <summary>Current value of the slider</summary>
        public int Value { get { return this.currentValue; } set { this.SetValueInternal(value, false); } }
        /// <summary>Sets the range of the slider</summary>
        public void SetRange(int min, int max)
        {
            this.minValue = min;
            this.maxValue = max;
            this.SetValueInternal(this.currentValue, false);
        }

        /// <summary>Sets the value internally and fires the event if needed</summary>
        protected void SetValueInternal(int newValue, bool fromInput)
        {
            // Clamp to the range
            newValue = Math.Max(this.minValue, newValue);
            newValue = Math.Min(this.maxValue, newValue);
            if (newValue == this.currentValue)
                return;

            // Update the value, the rects, then fire the events if necessar
            this.currentValue = newValue; 
            this.UpdateRectangles();
            this.RaiseValueChanged(this, fromInput);
        }
        #endregion

        /// <summary>Create new button instance</summary>
        public Slider(Dialog parent): base(parent)
        {
            this.controlType = ControlType.Slider;
            this.parentDialog = parent;

            this.isPressed = false;
            this.minValue = 0;
            this.maxValue = 100;
            this.currentValue = 50;
        }

        /// <summary>Does the control contain this point?</summary>
        public override bool ContainsPoint(System.Drawing.Point pt)
        {
            return this.boundingBox.Contains(pt) || this.buttonRect.Contains(pt);
        }

        /// <summary>Update the rectangles for the control</summary>
        protected override void UpdateRectangles()
        {
            // First get the bounding box
            base.UpdateRectangles ();

            // Create the button rect
            this.buttonRect = this.boundingBox;
            this.buttonRect.Width = this.buttonRect.Height; // Make it square

            // Offset it 
            this.buttonRect.Offset(-this.buttonRect.Width / 2, 0);
            this.buttonX = (int)((this.currentValue - this.minValue) * (float)this.boundingBox.Width / (this.maxValue - this.minValue) );
            this.buttonRect.Offset(this.buttonX, 0);
        }

        /// <summary>Gets a value from a position</summary>
        public int ValueFromPosition(int x)
        {
            float valuePerPixel = ((float)(this.maxValue - this.minValue) / (float)this.boundingBox.Width);
            return (int)(0.5f + this.minValue + valuePerPixel * (x - this.boundingBox.Left));
        }

        /// <summary>Handle mouse input input</summary>
        public override bool HandleMouse(Microsoft.Samples.DirectX.UtilityToolkit.NativeMethods.WindowMessage msg, System.Drawing.Point pt, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            switch(msg)
            {
                case NativeMethods.WindowMessage.LeftButtonDoubleClick:
                case NativeMethods.WindowMessage.LeftButtonDown:
                    {
                        if (this.buttonRect.Contains(pt))
                        {
                            // Pressed while inside the control
                            this.isPressed = true;
                            this.Parent.SampleFramework.Window.Capture = true;

                            this.dragX = pt.X;
                            this.dragOffset = this.buttonX - this.dragX;
                            if (!this.hasFocus)
                                Dialog.RequestFocus(this);
                        
                            return true;
                        }
                        if (this.boundingBox.Contains(pt))
                        {
                            if (pt.X > this.buttonX + this.controlX)
                            {
                                this.SetValueInternal(this.currentValue + 1, true);
                                return true;
                            }
                            if (pt.X < this.buttonX + this.controlX)
                            {
                                this.SetValueInternal(this.currentValue - 1, true);
                                return true;
                            }
                        }

                        break;
                    }
                case NativeMethods.WindowMessage.LeftButtonUp:
                    {
                        if (this.isPressed)
                        {
                            this.isPressed = false;
                            this.Parent.SampleFramework.Window.Capture = false;
                            Dialog.ClearFocus();
                            this.RaiseValueChanged(this, true);
                            return true;
                        }
                        break;
                    }
                case NativeMethods.WindowMessage.MouseMove:
                    {
                        if (this.isPressed)
                        {
                            this.SetValueInternal(this.ValueFromPosition(this.controlX + pt.X + this.dragOffset), true);
                            return true;
                        }
                        break;
                    }
            }
            return false;
        }

        /// <summary>Handle keyboard input</summary>
        public override bool HandleKeyboard(Microsoft.Samples.DirectX.UtilityToolkit.NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            if (msg == NativeMethods.WindowMessage.KeyDown)
            {
                switch((System.Windows.Forms.Keys)wParam.ToInt32())
                {
                    case System.Windows.Forms.Keys.Home:
                        this.SetValueInternal(this.minValue, true);
                        return true;
                    case System.Windows.Forms.Keys.End:
                        this.SetValueInternal(this.maxValue, true);
                        return true;
                    case System.Windows.Forms.Keys.Prior:
                    case System.Windows.Forms.Keys.Left:
                    case System.Windows.Forms.Keys.Up:
                        this.SetValueInternal(this.currentValue - 1, true);
                        return true;
                    case System.Windows.Forms.Keys.Next:
                    case System.Windows.Forms.Keys.Right:
                    case System.Windows.Forms.Keys.Down:
                        this.SetValueInternal(this.currentValue + 1, true);
                        return true;
                }
            }

            return false;
        }

    
        /// <summary>Render the slider</summary>
        public override void Render(Device device, float elapsedTime)
        {
            ControlState state = ControlState.Normal;
            if (this.IsVisible == false)
            {
                state = ControlState.Hidden;
            }
            else if (this.IsEnabled == false)
            {
                state = ControlState.Disabled;
            }
            else if (this.isPressed)
            {
                state = ControlState.Pressed;
            }
            else if (this.isMouseOver)
            {
                state = ControlState.MouseOver;
            }
            else if (this.hasFocus)
            {
                state = ControlState.Focus;
            }

            float blendRate = (state == ControlState.Pressed) ? 0.0f : 0.8f;

            Element e = this.elementList[Slider.TrackLayer] as Element;
            
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            this.parentDialog.DrawSprite(e, this.boundingBox);

            e = this.elementList[Slider.ButtonLayer] as Element;
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            this.parentDialog.DrawSprite(e, this.buttonRect);
        }
    }
}