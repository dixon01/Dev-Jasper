namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Button control
    /// </summary>
    public class Button : StaticText
    {
        public const int ButtonLayer = 0;
        public const int FillLayer = 1;
        protected bool isPressed;
        #region Event code
        public event EventHandler Click;
        /// <summary>Create new button instance</summary>
        protected void RaiseClickEvent(Button sender, bool wasTriggeredByUser)
        {
            // Discard events triggered programatically if these types of events haven't been
            // enabled
            if (!this.Parent.IsUsingNonUserEvents && !wasTriggeredByUser)
                return;

            if (this.Click != null)
                this.Click(sender, EventArgs.Empty);
        }
        #endregion

        /// <summary>Create new button instance</summary>
        public Button(Dialog parent) : base(parent)
        {
            this.controlType = ControlType.Button;
            this.parentDialog = parent;
            this.isPressed = false;
            this.hotKey = 0;
        }

        /// <summary>Can the button have focus</summary>
        public override bool CanHaveFocus { get { return this.IsVisible && this.IsEnabled; } }
        /// <summary>The hotkey for this button was pressed</summary>
        public override void OnHotKey()
        {
            this.RaiseClickEvent(this, true);
        }

        /// <summary>
        /// Will handle the keyboard strokes
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
                        this.isPressed = false;
                        this.RaiseClickEvent(this, true);

                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Handle mouse messages from the buttons
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
                            if (!this.hasFocus)
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
                            if (!this.parentDialog.IsUsingKeyboardInput)
                                Dialog.ClearFocus();

                            // Button click
                            if (this.ContainsPoint(pt))
                                this.RaiseClickEvent(this, true);
                        }
                    }
                    break;
            }

            return false;
        }

        /// <summary>Render the button</summary>
        public override void Render(Device device, float elapsedTime)
        {
            int offsetX = 0;
            int offsetY = 0;

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
            {
                state = ControlState.Focus;
            }

            // Background fill layer
            Element e = this.elementList[Button.ButtonLayer] as Element;
            float blendRate = (state == ControlState.Pressed) ? 0.0f : 0.8f;
            
            System.Drawing.Rectangle buttonRect = this.boundingBox;
            buttonRect.Offset(offsetX, offsetY);
            
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            e.FontColor.Blend(state, elapsedTime, blendRate);

            // Draw sprite/text of button
            this.parentDialog.DrawSprite(e, buttonRect);
            this.parentDialog.DrawText(this.textData, e, buttonRect);

            // Main button
            e = this.elementList[Button.FillLayer] as Element;
            
            // Blend current color
            e.TextureColor.Blend(state, elapsedTime, blendRate);
            e.FontColor.Blend(state, elapsedTime, blendRate);

            this.parentDialog.DrawSprite(e, buttonRect);
            this.parentDialog.DrawText(this.textData, e, buttonRect);
        }

    }
}