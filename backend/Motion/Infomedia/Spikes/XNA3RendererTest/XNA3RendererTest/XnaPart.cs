// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XnaPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XnaPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// </summary>
    public abstract class XnaPart : IPart
    {
        protected XnaPart(bool blink)
        {
            this.Blink = blink;
        }

        public bool Blink { get; private set; }

        public Rectangle Bounds { get; set; }

        public int Ascent { get; set; }

        public virtual bool IsNewLine
        {
            get
            {
                return false;
            }
        }

        public abstract int UpdateBounds(SpriteBatch sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline);

        public abstract void Render(SpriteBatch sprite, int x, int y, int alpha, IXnaRenderContext context);

        public virtual void OnResetDevice()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }
    }
}
