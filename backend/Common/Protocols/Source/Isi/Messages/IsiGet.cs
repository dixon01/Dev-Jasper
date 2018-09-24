// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiGet.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiGet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi.Messages
{
    using System.ComponentModel;

    /// <summary>
    /// ISI protocol message IsiGet.
    /// </summary>
    public class IsiGet : IsiMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsiGet"/> class.
        /// <see cref="IsiId"/> and <see cref="Cyclic"/> are set to -1 (their default value).
        /// </summary>
        public IsiGet()
        {
            this.IsiId = -1;
            this.Cyclic = -1;
        }

        /// <summary>
        /// Gets or sets the IsiId of this message.
        /// By default the value is -1.
        /// </summary>
        [DefaultValue(-1)]
        public int IsiId { get; set; }

        /// <summary>
        /// Gets or sets the requested items.
        /// By default this list is null.
        /// </summary>
        public DataItemRequestList Items { get; set; }

        /// <summary>
        /// Gets or sets the requested OnChange items.
        /// By default this list is null.
        /// </summary>
        public DataItemRequestList OnChange { get; set; }

        /// <summary>
        /// Gets or sets the Cyclic number of seconds of this message.
        /// By default the value is -1.
        /// </summary>
        [DefaultValue(-1)]
        public int Cyclic { get; set; }
    }
}
