// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;

    /// <summary>
    /// The descriptor for a digital input.
    /// </summary>
    [Serializable]
    public class InputDescriptor : IoDescriptorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputDescriptor"/> class.
        /// </summary>
        public InputDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDescriptor"/> class.
        /// </summary>
        /// <param name="index">
        /// The <see cref="IoDescriptorBase.Index"/>.
        /// </param>
        /// <param name="name">
        /// The <see cref="IoDescriptorBase.Name"/>.
        /// </param>
        public InputDescriptor(int index, string name)
        {
            this.Index = index;
            this.Name = name;
        }
    }
}