// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiRegister.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiRegister type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi.Messages
{
    /// <summary>
    /// ISI protocol message IsiRegister.
    /// </summary>
    public class IsiRegister : IsiMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsiRegister"/> class.
        /// The <see cref="Commands"/> list is initialized to an empty list.
        /// </summary>
        public IsiRegister()
        {
            this.Commands = new DataItemRequestList();
        }

        /// <summary>
        /// Gets or sets the requested commands.
        /// </summary>
        public DataItemRequestList Commands { get; set; }
    }
}
