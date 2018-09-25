// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LevelShifterLevelPort.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LevelShifterLevelPort type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Mgi
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Motion.Common.Mgi.IO;

    /// <summary>
    /// Port for the DVI Level Shifter's <see cref="GraphicControlPin.Cct1"/>
    /// and <see cref="GraphicControlPin.Cct2"/> pins.
    /// </summary>
    internal partial class LevelShifterLevelPort : MgiPortBase<int>
    {
        private static readonly EnumValues ValidValues =
            new EnumValues(new Dictionary<int, string> { { 0, "0" }, { 2, "2" }, { 4, "4" }, { 6, "6" }, });

        private readonly Output cct1;
        private readonly Output cct2;

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelShifterLevelPort"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the port.
        /// </param>
        /// <param name="cct1">
        /// The CCT 1 output.
        /// </param>
        /// <param name="cct2">
        /// The CCT 2 output.
        /// </param>
        public LevelShifterLevelPort(string name, Output cct1, Output cct2)
            : base(name, true, true, ValidValues, 0)
        {
            if (cct1 == null)
            {
                throw new ArgumentNullException("cct1");
            }

            if (cct2 == null)
            {
                throw new ArgumentNullException("cct2");
            }

            this.cct1 = cct1;
            this.cct2 = cct2;
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
        protected override IOValue ToIOValue(int value)
        {
            return this.CreateValue(value);
        }

        /// <summary>
        /// Updates the port with the given <see cref="IOValue"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be set to the port.
        /// </param>
        protected override void UpdateIO(IOValue value)
        {
            this.cct1.Write((value.Value & 0x04) == 0x04);
            this.cct2.Write((value.Value & 0x02) == 0x02);
        }
    }
}