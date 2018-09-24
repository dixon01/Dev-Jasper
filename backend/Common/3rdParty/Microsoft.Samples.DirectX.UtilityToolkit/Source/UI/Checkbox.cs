namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Checkbox control
    /// </summary>
    public class Checkbox : Button
    {
        public const int BoxLayer = 0;
        public const int CheckLayer = 1;
        #region Event code
        public event EventHandler Changed;
        /// <summary>Create new button instance</summary>
        protected void RaiseChangedEvent(Checkbox sender, bool wasTriggeredByUser)
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
        protected System.Drawing.Rectangle buttonRect;
        protected System.Drawing.Rectangle textRect;
        protected bool isBoxChecked;

        /// <summary>
        /// Create new checkbox instance
        /// </summary>
        public Checkbox(Dialog parent) : base(parent)
        {
            this.controlType = ControlType.CheckBox;
            this.isBoxChecked = false;
            this.parentDialog = parent;
        }

        /// <summary>
        /// Checked property
        /// </summary>
        public virtual bool IsChecked
        {
            get { return this.isBoxChecked; }
            set { this.SetCheckedInternal(value, false); }
        }
        /// <summary>
        /// Sets the checked state and fires the event if necessary
        /// </summary>
        protected virtual void SetCheckedInternal(bool ischecked, bool fromInput)
        {
            this.isBoxChecked = ischecked;
            this.RaiseChangedEvent(this, fromInput);
        }

        /// <summary>
        /// Override hotkey to fire event
        /// </summary>
        public override void OnHotKey()
        {
            this.SetCheckedInternal(!this.isBoxChecked, true);
        }

        /// <summary>
        /// Does the control contain the point?
        /// </summary>
        public override bool ContainsPoint(System.Drawing.Point pt)
        {
            return (this.boundingBox.Contains(pt) || this.buttonRect.Contains(pt));
        }
        /// <summary>
        /// Update the rectangles
        /// </summary>
        protected override void UpdateRectangles()
        {
            // Update base first
            base.UpdateRectangles();

            // Update the two rects
            this.buttonRect = this.boundingBox;
            this.buttonRect = new System.Drawing.Rectangle(this.boundingBox.Location,
                new System.Drawing.Size(this.boundingBox.Height, this.boundingBox.Height));

            this.textRect = this.boundingBox;
            this.textRect.Offset((int) (1.25f * this.buttonRect.Width), 0);
        }

        /// <summary>
        /// Render the checkbox control
        /// </summary>
        public override void Render(Device device, float elapsedTime)
        {
            ControlState state = ControlState.Normal;
            if (this.IsVisible == false)
                state = ControlState.Hidden;
            else if (this.IsEnabled == false)
                state = ControlState.Disabled;
            else if (this.isPressed)
                state = ControlState.Pressed;
            else if (this.isMouseOver)
                state = ControlState.MouseOver;
            else if (this.hasFocus)
                state = ControlState.Focus;
 
            Element e = this.elementList[Checkbox.BoxLayer] as Element;
            float blendRate = (state == ControlState.Pressed) ? 0.0f : 0.8f;
            
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            e.FontColor.Blend(state, elapsedTime, blendRate);

            // Draw sprite/text of checkbox
            this.parentDialog.DrawSprite(e, this.buttonRect);
            this.parentDialog.DrawText(this.textData, e, this.textRect);

            if (!this.isBoxChecked)
                state = ControlState.Hidden;

            e = this.elementList[Checkbox.CheckLayer] as Element;
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);

            // Draw sprite of checkbox
            this.parentDialog.DrawSprite(e, this.buttonRect);
        }

        /// <summary>
        /// Handle the keyboard for the checkbox
        /// </summary>
        public override bool HandleKeyboard(NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            switch(msg)
            {
                case NativeMethods.WindowMessage.KeyDown:
                    if ((System.Windows.Forms.Keys)wParam.ToInt32() == System.Windows.Forms.Keys.Space)
                    {
                        this.isPressed = true;
                        return true;
                    }
                    break;
                case NativeMethods.WindowMessage.KeyUp:
                    if ((System.Windows.Forms.Keys)wParam.ToInt32() == System.Windows.Forms.Keys.Space)
                    {
                        if (this.isPressed)
                        {
                            this.isPressed = false;
                            this.SetCheckedInternal(!this.isBoxChecked, true);
                        }
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Handle mouse messages from the checkbox
        /// </summary>
        public override bool HandleMouse(NativeMethods.WindowMessage msg, System.Drawing.Point pt, IntPtr wParam, IntPtr lParam)
        {
            if (!this.IsEnabled || !this.IsVisible)
                return false;

            switch(msg)
            {
                case NativeMethods.WindowMessage.LeftButtonDoubleClick:
                case NativeMethods.WindowMessage.LeftButtonDown:
                    {
                        if (this.ContainsPoint(pt))
                        {
                            // Pressed while inside the control
                            this.isPressed = true;
                            this.Parent.SampleFramework.Window.Capture = true;
                            if ( (!this.hasFocus) && (this.parentDialog.IsUsingKeyboardInput) )
                                Dialog.RequestFocus(this);

                            return true;
                        }
                    }
                    break;
                case NativeMethods.WindowMessage.LeftButtonUp:
                    {
                        if (this.isPressed)
                        {
                            this.isPressed = false;
                            this.Parent.SampleFramework.Window.Capture = false;

                            // Button click
                            if (this.ContainsPoint(pt))
                            {
                                this.SetCheckedInternal(!this.isBoxChecked, true);
                            }
                        
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }
    }
}