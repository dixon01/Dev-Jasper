namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>Base class for all controls</summary>
    public abstract class Control
    {
        #region Instance data
        protected Dialog parentDialog; // Parent container
        public uint index; // Index within the control list
        public bool isDefault;

        // Protected members
        protected object localUserData; // User specificied data
        protected bool visible;
        protected bool isMouseOver;
        protected bool hasFocus;
        protected int controlId; // ID Number
        protected ControlType controlType; // Control type, set in constructor
        protected System.Windows.Forms.Keys hotKey; // Controls hotkey
        protected bool enabled; // Enabled/disabled flag
        protected System.Drawing.Rectangle boundingBox; // Rectangle defining the active region of the control

        protected int controlX,controlY,width,height; // Size, scale, and positioning members

        protected ArrayList elementList = new ArrayList(); // All display elements
        #endregion

        /// <summary>Initialize the control</summary>
        public virtual void OnInitialize() {} // Nothing to do here
        /// <summary>Render the control</summary>
        public virtual void Render(Device device, float elapsedTime) {} // Nothing to do here
        /// <summary>Message Handler</summary>
        public virtual bool MsgProc(IntPtr hWnd, NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam) {return false;} // Nothing to do here
        /// <summary>Handle the keyboard data</summary>
        public virtual bool HandleKeyboard(NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam) {return false;} // Nothing to do here
        /// <summary>Handle the mouse data</summary>
        public virtual bool HandleMouse(NativeMethods.WindowMessage msg, System.Drawing.Point pt, IntPtr wParam, IntPtr lParam) {return false;} // Nothing to do here

        /// <summary>User specified data</summary>
        public object UserData { get { return this.localUserData; } set { this.localUserData = value; } }
        /// <summary>The parent dialog of this control</summary>
        public Dialog Parent { get { return this.parentDialog; } }
        /// <summary>Can the control have focus</summary>
        public virtual bool CanHaveFocus { get { return false; } }
        /// <summary>Called when control gets focus</summary>
        public virtual void OnFocusIn() { this.hasFocus = true;}
        /// <summary>Called when control loses focus</summary>
        public virtual void OnFocusOut() { this.hasFocus = false;}
        /// <summary>Called when mouse goes over the control</summary>
        public virtual void OnMouseEnter() { this.isMouseOver = true;}
        /// <summary>Called when mouse leaves the control</summary>
        public virtual void OnMouseExit() { this.isMouseOver = false;}
        /// <summary>Called when the control's hotkey is hit</summary>
        public virtual void OnHotKey() {} // Nothing to do here
        /// <summary>Does the control contain this point</summary>
        public virtual bool ContainsPoint(System.Drawing.Point pt) { return this.boundingBox.Contains(pt); }
        /// <summary>Is the control enabled</summary>
        public virtual bool IsEnabled { get { return this.enabled; } set { this.enabled = value; } }
        /// <summary>Is the control visible</summary>
        public virtual bool IsVisible { get { return this.visible; } set { this.visible = value; } }
        /// <summary>Type of the control</summary>
        public virtual ControlType ControlType { get { return this.controlType; } }
        /// <summary>Unique ID of the control</summary>
        public virtual int ID { get { return this.controlId; } set { this.controlId = value; } }
        /// <summary>Called to set control's location</summary>
        public virtual void SetLocation(int x, int y) { this.controlX = x; this.controlY = y; this.UpdateRectangles(); }
        /// <summary>Called to set control's size</summary>
        public virtual void SetSize(int w, int h) { this.width = w; this.height = h; this.UpdateRectangles(); }
        /// <summary>The controls hotkey</summary>
        public virtual System.Windows.Forms.Keys Hotkey { get { return this.hotKey; } set { this.hotKey = value; } }

        /// <summary>
        /// Index for the elements this control has access to
        /// </summary>
        public Element this[uint index]
        {
            get { return this.elementList[(int)index] as Element; }
            set 
            { 
                if (value == null)
                    throw new ArgumentNullException("ControlIndexer", "You cannot set a null element.");
                
                // Is the collection big enough?
                for(uint i = (uint)this.elementList.Count; i <= index; i++)
                {
                    // Add a new one
                    this.elementList.Add(new Element());
                }
                // Update the data (with a clone)
                this.elementList[(int)index] = value.Clone();
            }
        }
        /// <summary>
        /// Create a new instance of a control
        /// </summary>
        protected Control(Dialog parent)
        {
            this.controlType = ControlType.Button;
            this.parentDialog = parent;
            this.controlId = 0;
            this.index = 0;

            this.enabled = true;
            this.visible = true;
            this.isMouseOver = false;
            this.hasFocus = false;
            this.isDefault = false;

            this.controlX = 0; this.controlY = 0; this.width = 0; this.height = 0;
        }

        /// <summary>
        /// Refreshes the control
        /// </summary>
        public virtual void Refresh()
        {
            this.isMouseOver = false;
            this.hasFocus = false;
            for(int i = 0; i < this.elementList.Count; i++)
            {
                (this.elementList[i] as Element).Refresh();
            }
        }

        /// <summary>
        /// Updates the rectangles
        /// </summary>
        protected virtual void UpdateRectangles()
        {
            this.boundingBox = new System.Drawing.Rectangle(this.controlX, this.controlY, this.width, this.height);
        }
    }
}