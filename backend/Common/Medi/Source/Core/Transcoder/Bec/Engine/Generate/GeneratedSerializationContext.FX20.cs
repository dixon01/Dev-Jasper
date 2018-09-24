// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedSerializationContext.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GeneratedSerializationContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    /// <summary>
    /// This class is not meant for use outside this assembly.
    /// Serialization context for generated (de)serialization.
    /// Wraps the internal <see cref="SerializationContext"/>,
    /// so this context can be public.
    /// </summary>
    public partial class GeneratedSerializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedSerializationContext"/> class.
        /// </summary>
        /// <param name="context">
        /// The context to be wrapped.
        /// </param>
        internal GeneratedSerializationContext(SerializationContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets the wrapped context.
        /// </summary>
        internal SerializationContext Context { get; private set; }
    }
}
