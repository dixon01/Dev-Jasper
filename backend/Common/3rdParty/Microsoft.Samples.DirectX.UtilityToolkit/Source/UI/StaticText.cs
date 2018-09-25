namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// StaticText text control
    /// </summary>
    public class StaticText : Control
    {
        public const int TextElement = 0;
        protected string textData; // Window text

        /// <summary>
        /// Create a new instance of a static text control
        /// </summary>
        public StaticText(Dialog parent) : base(parent)
        {
            this.controlType = ControlType.StaticText;
            this.parentDialog = parent;
            this.textData = string.Empty;
            this.elementList.Clear();
        }

        /// <summary>
        /// Render this control
        /// </summary>
        public override void Render(Device device, float elapsedTime)
        {
            if (!this.IsVisible)
                return; // Nothing to do here

            ControlState state = ControlState.Normal;
            if (!this.IsEnabled)
                state = ControlState.Disabled;

            // Blend the element colors
            Element e = this.elementList[TextElement] as Element;
            e.FontColor.Blend(state, elapsedTime);

            // Render with a shadow
            this.parentDialog.DrawText(this.textData, e, this.boundingBox, true);
        }

        /// <summary>
        /// Return a copy of the string
        /// </summary>
        public string GetTextCopy()
        {
            return string.Copy(this.textData);
        }

        /// <summary>
        /// Sets the updated text for this control
        /// </summary>
        public void SetText(string newText)
        {
            this.textData = newText;
        }

    }
}