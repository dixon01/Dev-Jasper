// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupResponseFrame.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SetupResponseFrame type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    /// <summary>
    /// Response frame to a setup command (0x01).
    /// This frame is sent from the slave to the master.
    /// </summary>
    public class SetupResponseFrame : ShortFrameBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupResponseFrame"/> class.
        /// </summary>
        public SetupResponseFrame()
            : base(FunctionCode.SetupResponse)
        {
        }
    }
}