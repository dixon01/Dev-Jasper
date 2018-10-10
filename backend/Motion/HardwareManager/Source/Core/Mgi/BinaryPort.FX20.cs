// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryPort.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BinaryPort type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Mgi
{
    using System;

    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Motion.Common.Mgi.IO;

    /// <summary>
    /// A GIOoM port that represents an <see cref="IOBase"/> input or output.
    /// </summary>
    internal partial class BinaryPort : MgiPortBase<bool>
    {
        private readonly Output output;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryPort"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the GIOoM port.
        /// </param>
        /// <param name="io">
        /// The I/O that is represented by this class.
        /// </param>
        public BinaryPort(string name, IOBase io)
            : base(name, true, io is Output, new FlagValues(), FlagValues.False)
        {
            if (io == null)
            {
                throw new ArgumentNullException("io");
            }

            this.IO = io;
            this.output = io as Output;
        }

        /// <summary>
        /// Gets the I/O that is represented by this class.
        /// </summary>
        public IOBase IO { get; private set; }

        /// <summary>
        /// Converts the value of the port to an <see cref="IOValue"/>.
        /// </summary>
        /// <param name="value">
        /// The original value.
        /// </param>
        /// <returns>
        /// The converted <see cref="IOValue"/>.
        /// </returns>
        protected override IOValue ToIOValue(bool value)
        {
            return FlagValues.GetValue(value);
        }

        /// <summary>
        /// Updates the port with the given <see cref="IOValue"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be set to the port.
        /// </param>
        protected override void UpdateIO(IOValue value)
        {
            if (this.output != null)
            {
                this.output.Write(value.Equals(FlagValues.True));
            }
        }
    }
}