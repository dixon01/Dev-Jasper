namespace MeshTextDirectXTest
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// An argument to be passed by the NewVideoTexture Event.
    /// </summary>
    public class NewVideoTextureEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Texture2D that is passed to subscribers of the NewVideoTexture event.
        /// </summary>
        public Texture VideoTexture2D { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="videoTexture2D">The Texture2D object that is passed to subscribers.</param>
        public NewVideoTextureEventArgs(Texture videoTexture2D)
        {
            this.VideoTexture2D = videoTexture2D;
        }
    }
}