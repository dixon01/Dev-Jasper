namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// Radio button control
    /// </summary>
    public class RadioButton : Checkbox
    {
        protected uint buttonGroupIndex;
        /// <summary>
        /// Create new radio button instance
        /// </summary>
        public RadioButton(Dialog parent) : base(parent)
        {
            this.controlType = ControlType.RadioButton;
            this.parentDialog = parent;
        }

        /// <summary>
        /// Button Group property
        /// </summary>
        public uint ButtonGroup
        {
            get { return this.buttonGroupIndex; }
            set { this.buttonGroupIndex = value; }
        }

        /// <summary>
        /// Sets the check state and potentially clears the group
        /// </summary>
        public void SetChecked(bool ischecked, bool clear)
        {
            this.SetCheckedInternal(ischecked, clear, false); 
        }

        /// <summary>
        /// Sets the checked state and fires the event if necessary
        /// </summary>
        protected virtual void SetCheckedInternal(bool ischecked, bool clearGroup, bool fromInput)
        {
            this.isBoxChecked = ischecked;
            this.RaiseChangedEvent(this, fromInput);
        }

        /// <summary>
        /// Override hotkey to fire event
        /// </summary>
        public override void OnHotKey()
        {
            this.SetCheckedInternal(true, true);
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
                            this.parentDialog.ClearRadioButtonGroup(this.buttonGroupIndex);
                            this.isBoxChecked = !this.isBoxChecked;

                            this.RaiseChangedEvent(this, true);
                        }
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Handle mouse messages from the radio button
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
                                this.parentDialog.ClearRadioButtonGroup(this.buttonGroupIndex);
                                this.isBoxChecked = !this.isBoxChecked;

                                this.RaiseChangedEvent(this, true);
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