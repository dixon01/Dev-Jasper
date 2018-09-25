// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputResponseFrame.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OutputResponseFrame type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    /// <summary>
    /// Response frame to an output command (0x02).
    /// This frame is sent from the slave to the master.
    /// </summary>
    public class OutputResponseFrame : ShortFrameBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputResponseFrame"/> class.
        /// </summary>
        public OutputResponseFrame()
            : base(FunctionCode.OutputResponse)
        {
        }
    }
}