namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System.IO;

    /// <summary>
    /// The goodbye rtcp packet.
    /// </summary>
    public class GoodbyeRtcpPacket : RtcpPacket
    {
        #region Constants and Fields

        private readonly int[] sources;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GoodbyeRtcpPacket"/> class.
        /// </summary>
        /// <param name="sources">
        /// The sources.
        /// </param>
        public GoodbyeRtcpPacket(int[] sources)
            : base(false, sources.Length, 203)
        {
            this.sources = (int[])sources.Clone();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add body.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void AddBody(BinaryWriter writer)
        {
            foreach (int src in this.sources)
            {
                writer.Write((byte)(src >> 24));
                writer.Write((byte)(src >> 16));
                writer.Write((byte)(src >> 8));
                writer.Write((byte)src);
            }
        }

        #endregion
    }
}