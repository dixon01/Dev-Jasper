// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompactInputOutputManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompactInputOutputManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    using System;
    using System.Collections.Generic;

    using Kontron.Jida32.I2C;

    /// <summary>
    /// The input and output manager for the MGI Compact device.
    /// This device has 6 inputs followed by 2 outputs and
    /// doesn't have the Update LED nor the Button.
    /// </summary>
    internal class CompactInputOutputManager : InputOutputManagerBase
    {
        private const byte InputOutputAddress = 0x44;
        private const byte InputCount = 6;
        private const byte OutputCount = 2;
        private const byte InputOutputInputMask = (1 << InputCount) - 1;

        private const byte GraphicControlAddress = 0x40;
        private const byte GraphicControlUpdateLedPin = 0;
        private const byte GraphicControlPinOffset = 3;
        private const byte GraphicControlPinCount = 5;
        private const byte GraphicControlInputMask = 0x06; // bits 1 and 2 are unused, we assume they are inputs

        private readonly List<IOBase> gpios;

        private readonly List<Output> graphicControlOutputs;

        private Output updateLed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactInputOutputManager"/> class.
        /// </summary>
        public CompactInputOutputManager()
        {
            this.gpios = new List<IOBase>(8);

            this.graphicControlOutputs = new List<Output>(GraphicControlPinCount);
        }

        /// <summary>
        /// Gets the general button input (always null).
        /// </summary>
        public override Input Button
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the update LED output.
        /// </summary>
        public override Output UpdateLed
        {
            get
            {
                return this.updateLed;
            }
        }

        /// <summary>
        /// Gets the number of general programmable inputs and outputs in the system.
        /// </summary>
        public override int GpioCount
        {
            get
            {
                return this.gpios.Count;
            }
        }

        /// <summary>
        /// Gets the antenna short input.
        /// </summary>
        public override Input GpsShortCircuit
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the antenna detect input.
        /// </summary>
        public override Input GpsCurrentDetection
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the given general programmable I/O.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="IOBase"/>.
        /// </returns>
        public override IOBase GetGpio(int index)
        {
            return this.gpios[index];
        }

        /// <summary>
        /// Gets the graphic control output for the given index.
        /// </summary>
        /// <param name="index">
        /// The index (from 0).
        /// </param>
        /// <param name="pin">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Output"/>.
        /// </returns>
        public override Output GetGraphicControlOutput(int index, GraphicControlPin pin)
        {
            if (index < 0 || index > 1)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var pos = (index * 4) + (int)pin - GraphicControlPinOffset;
            if (pos < 0)
            {
                // Compact has not Trim and CCT1/2 for the internal display (only HPD is available)
                return null;
            }

            return this.graphicControlOutputs[pos];
        }

        /// <summary>
        /// Gets the multiprotocol transceiver port.
        /// </summary>
        /// <param name="index">
        ///     The index (from 0).
        /// </param>
        /// <param name="pin">
        ///     The type.
        /// </param>
        /// <returns>
        /// The <see cref="IOBase"/>.
        /// </returns>
        public override Output GetMultiprotocolTransceiverOutput(int index, MultiprotocolTransceiverPin pin)
        {
            return null;
        }

        /// <summary>
        /// Gets the input mask for a certain I2C address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// A byte with all the input bits set to 1 and all output bits set to 0.
        /// </returns>
        protected override byte GetInputMask(byte address)
        {
            switch (address)
            {
                case InputOutputAddress:
                    return InputOutputInputMask;
                case GraphicControlAddress:
                    return GraphicControlInputMask;
                default:
                    throw new ArgumentException("Unknown address", "address");
            }
        }

        /// <summary>
        /// Initializes this manager with the given I2C bus.
        /// </summary>
        /// <param name="busI2C">
        /// The I2C bus.
        /// </param>
        protected override void Initialize(I2CBus busI2C)
        {
            for (byte i = 0; i < InputCount; i++)
            {
                this.gpios.Add(new Input(InputOutputAddress, i, this));
            }

            for (byte i = InputCount; i < InputCount + OutputCount; i++)
            {
                this.gpios.Add(new Output(InputOutputAddress, i, this));
            }

            this.updateLed = new Output(GraphicControlAddress, GraphicControlUpdateLedPin, this);

            for (byte i = 0; i < GraphicControlPinCount; i++)
            {
                this.graphicControlOutputs.Add(
                    new Output(GraphicControlAddress, (byte)(i + GraphicControlPinOffset), this));
            }
        }
    }
}