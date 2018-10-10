// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Base class for all AHDLC frames.
    /// </summary>
    public abstract class FrameBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameBase"/> class.
        /// </summary>
        /// <param name="functionCode">
        /// The function code.
        /// </param>
        protected FrameBase(FunctionCode functionCode)
        {
            this.FunctionCode = functionCode;
        }

        /// <summary>
        /// Gets or sets the AHDLC address (1..15).
        /// </summary>
        [Category("Telegram")]
        public int Address { get; set; }

        /// <summary>
        /// Gets the function code including the direction.
        /// </summary>
        [Category("Telegram")]
        public FunctionCode FunctionCode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this frame is coming from the master.
        /// </summary>
        [Category("Telegram")]
        public bool IsFromMaster
        {
            get
            {
                return ((int)this.FunctionCode & 0x80) != 0;
            }
        }

        /// <summary>
        /// Creates the correct frame from its function code.
        /// </summary>
        /// <param name="functionCode">
        /// The function code.
        /// </param>
        /// <returns>
        /// The <see cref="FrameBase"/> implementation.
        /// </returns>
        public static FrameBase Create(FunctionCode functionCode)
        {
            switch (functionCode)
            {
                case FunctionCode.StatusRequest:
                    return new StatusRequestFrame();
                case FunctionCode.StatusResponse:
                    return new StatusResponseFrame();
                case FunctionCode.SetupCommand:
                    return new SetupCommandFrame(DisplayMode.StaticBitmap);
                case FunctionCode.SetupResponse:
                    return new SetupResponseFrame();
                case FunctionCode.OutputCommand:
                    return new OutputCommandFrame();
                case FunctionCode.OutputResponse:
                    return new OutputResponseFrame();
                default:
                    throw new ArgumentOutOfRangeException("functionCode");
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.FunctionCode.ToString();
        }

        /// <summary>
        /// Gets the <see cref="T:Gorba.Common.Protocols.Ahdlc.Frames.FunctionCode"/>
        /// of the expected response for this request.
        /// </summary>
        /// <returns>
        /// The response <see cref="FunctionCode"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// If this is not a request.
        /// </exception>
        internal FunctionCode GetResponseCode()
        {
            if (!this.IsFromMaster)
            {
                throw new NotSupportedException("Can't get response code from a response");
            }

            return this.FunctionCode & (FunctionCode)0x70;
        }

        /// <summary>
        /// Writes the entire frame including the two boundary bytes to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal virtual void WriteTo(FrameWriter writer)
        {
            writer.WriteFrameBoundary();
            var commandByte = (this.Address & 0x0F) | (int)this.FunctionCode & 0xF0;
            writer.WriteByte((byte)commandByte);

            this.WriteContents(writer);

            writer.WriteFrameBoundary();
        }

        /// <summary>
        /// Writes the contents (everything after the command byte and before the end boundary)
        /// of this frame to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal abstract void WriteContents(FrameWriter writer);
    }
}
