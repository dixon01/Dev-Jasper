// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryEnumMgiPort.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BinaryEnumMgiPort type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Mgi
{
    using System;

    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Motion.Common.Mgi.IO;

    /// <summary>
    /// Port for the DVI Level Shifter's <see cref="GraphicControlPin.Trim"/> pin.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the enum.
    /// </typeparam>
    internal partial class BinaryEnumMgiPort<T> : MgiPortBase<T>
        where T : struct, IConvertible
    {
        private readonly Output output;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryEnumMgiPort{T}"/> class.
        /// </summary>
        /// <param name="name">
        /// The port name.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        public BinaryEnumMgiPort(string name, Output output)
            : base(name, true, true, EnumValues.FromEnum<T>(), 0)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            this.output = output;
        }

        /// <summary>
        /// Converts the value of the port to an <see cref="IOValue"/>.
        /// </summary>
        /// <param name="value">
        /// The original value.
        /// </param>
        /// <returns>
        /// The converted <see cref="IOValue"/>.
        /// </returns>
        protected override IOValue ToIOValue(T value)
        {
            return this.CreateValue(value.ToInt32(null));
        }

        /// <summary>
        /// Updates the port with the given <see cref="IOValue"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be set to the port.
        /// </param>
        protected override void UpdateIO(IOValue value)
        {
            this.output.Write(value.Value != 0);
        }
    }
}