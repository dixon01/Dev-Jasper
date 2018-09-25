// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OutputDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;

    /// <summary>
    /// The descriptor for a digital output.
    /// </summary>
    [Serializable]
    public class OutputDescriptor : IoDescriptorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputDescriptor"/> class.
        /// </summary>
        public OutputDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputDescriptor"/> class.
        /// </summary>
        /// <param name="index">
        /// The <see cref="IoDescriptorBase.Index"/>.
        /// </param>
        /// <param name="name">
        /// The <see cref="IoDescriptorBase.Name"/>.
        /// </param>
        public OutputDescriptor(int index, string name)
        {
            this.Index = index;
            this.Name = name;
        }
    }
}