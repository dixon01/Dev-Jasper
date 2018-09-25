// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateException.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown in the update system if something went wrong.
    /// </summary>
    public partial class UpdateException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateException"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The streaming context.
        /// </param>
        protected UpdateException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
