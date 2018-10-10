// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evIraLoginAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evIraLoginAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The IRA login acknowledge event.
    /// </summary>
    public class evIraLoginAck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evIraLoginAck"/> class.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        public evIraLoginAck(int response)
        {
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evIraLoginAck"/> class.
        /// </summary>
        public evIraLoginAck()
        {
        }

        /// <summary>
        /// Gets or sets the IRA response code.
        /// </summary>
        public int Response { get; set; }
    }
}