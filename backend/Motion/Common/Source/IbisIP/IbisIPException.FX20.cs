// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisIPException.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisIPException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown if something goes wrong in IBIS-IP.
    /// </summary>
    public partial class IbisIPException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisIPException"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The streaming context.
        /// </param>
        protected IbisIPException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
