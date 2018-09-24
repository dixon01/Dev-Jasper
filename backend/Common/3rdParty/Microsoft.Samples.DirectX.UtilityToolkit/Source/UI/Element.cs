namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Contains all the display tweakables for a sub-control
    /// </summary>
    public class Element : ICloneable
    {
        #region Magic Numbers
        #endregion

        #region Instance Data
        public uint TextureIndex; // Index of the texture for this Element 
        public uint FontIndex; // Index of the font for this Element 
        public DrawTextFormat textFormat; // The Format argument to draw text

        public System.Drawing.Rectangle textureRect; // Bounding rectangle of this element on the composite texture

        public BlendColor TextureColor;
        public BlendColor FontColor;
        #endregion

        /// <summary>Set the texture</summary>
        public void SetTexture(uint tex, System.Drawing.Rectangle texRect, ColorValue defaultTextureColor)
        {
            // Store data
            this.TextureIndex = tex;
            this.textureRect = texRect;
            this.TextureColor.Initialize(defaultTextureColor);
        }
        /// <summary>Set the texture</summary>
        public void SetTexture(uint tex, System.Drawing.Rectangle texRect) { this.SetTexture(tex, texRect, Dialog.WhiteColorValue); }
        /// <summary>Set the font</summary>
        public void SetFont(uint font, ColorValue defaultFontColor, DrawTextFormat format)
        {
            // Store data
            this.FontIndex = font;
            this.textFormat = format;
            this.FontColor.Initialize(defaultFontColor);
        }
        /// <summary>Set the font</summary>
        public void SetFont(uint font){ this.SetFont(font, Dialog.WhiteColorValue, DrawTextFormat.Center | DrawTextFormat.VerticalCenter ); }
        /// <summary>
        /// Refresh this element
        /// </summary>
        public void Refresh()
        {
            if (this.TextureColor.States != null) 
                this.TextureColor.Current = this.TextureColor.States[(int)ControlState.Hidden];
            if (this.FontColor.States != null) 
                this.FontColor.Current = this.FontColor.States[(int)ControlState.Hidden];
        }

        #region ICloneable Members
        /// <summary>Clone an object</summary>
        public Element Clone() 
        { 
            Element e = new Element();
            e.TextureIndex = this.TextureIndex;
            e.FontIndex = this.FontIndex;
            e.textFormat = this.textFormat;
            e.textureRect = this.textureRect; 
            e.TextureColor = this.TextureColor;
            e.FontColor = this.FontColor;

            return e;
        }
        /// <summary>Clone an object</summary>
        object ICloneable.Clone() { throw new NotSupportedException("Use the strongly typed clone.");}

        #endregion
    }
}