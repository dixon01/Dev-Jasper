// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pc2InputOutputManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Pc2InputOutputManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    using System;
    using System.Collections.Generic;

    using Kontron.Jida32.I2C;

    /// <summary>
    /// The input and output manager for the MGI PC-2.
    /// This device has 4 inputs followed by 4 outputs and
    /// has both the Update LED and the Button.
    /// </summary>
    internal class Pc2InputOutputManager : InputOutputManagerBase
    {
        private const byte MultiprotocolTransceiverAddress = 0x42;
        private const byte ProtocolOutputsCount = 6;
        private const byte ButtonPin = 6;
        private const byte UpdateLedPin = 7;
        private const byte MultiprotocolTransceiverInputMask = 1 << ButtonPin;

        private const byte InputOutputAddress = 0x44;
        private const byte InputCount = 4;
        private const byte OutputCount = 4;
        private const byte InputOutputInputMask = (1 << InputCount) - 1;

        private const byte GraphicControlAddress = 0x40;
        private const byte GraphicControlPinCount = 8;
        private const byte GraphicControlInputMask = 0x00;

        private const byte GpsAntennaAddress = 0x46;
        private const byte ShortCircuitPin = 0;
        private const byte CurrentDetectionPin = 1;
        private const byte GpsAntennaInputMask = 0xFF; // 0x03, but all others are unused, so we assume they are inputs

        private readonly List<IOBase> gpios;

        private readonly List<Output> graphicControlOutputs;

        private readonly List<Output> multiprotocolOutputs;

        private Input button;
        private Output updateLed;

        private Input gpsShortCircuit;
        private Input gpsCurrentDetection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pc2InputOutputManager"/> class.
        /// </summary>
        public Pc2InputOutputManager()
        {
            this.gpios = new List<IOBase>(8);
            this.graphicControlOutputs = new List<Output>(GraphicControlPinCount);
            this.multiprotocolOutputs = new List<Output>(ProtocolOutputsCount);
        }

        /// <summary>
        /// Gets the general button input.
        /// </summary>
        public override Input Button
        {
            get
            {
                return this.button;
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
        /// Gets the antenna short input.
        /// </summary>
        public override Input GpsShortCircuit
        {
            get
            {
                return this.gpsShortCircuit;
            }
        }

        /// <summary>
        /// Gets the antenna detect input.
        /// </summary>
        public override Input GpsCurrentDetection
        {
            get
            {
                return this.gpsCurrentDetection;
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
        /// Gets the graphic control output fir the given index.
        /// </summary>
        /// <param name="index">
        /// The index (from 0).
        /// </param>
        /// <param name="pin">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IOBase"/>.
        /// </returns>
        public override Output GetGraphicControlOutput(int index, GraphicControlPin pin)
        {
            if (index < 0 || index > 1)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return this.graphicControlOutputs[(index * 4) + (int)pin];
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
            if (index < 0 || index > 1)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return this.multiprotocolOutputs[(index * 3) + (int)pin];
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
                case MultiprotocolTransceiverAddress:
                    return MultiprotocolTransceiverInputMask;
                case GraphicControlAddress:
                    return GraphicControlInputMask;
                case GpsAntennaAddress:
                    return GpsAntennaInputMask;
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
            this.button = new Input(MultiprotocolTransceiverAddress, ButtonPin, this);
            this.updateLed = new Output(MultiprotocolTransceiverAddress, UpdateLedPin, this);

            for (byte i = 0; i < ProtocolOutputsCount; i++)
            {
                this.multiprotocolOutputs.Add(new Output(MultiprotocolTransceiverAddress, i, this));
            }

            for (byte i = 0; i < InputCount; i++)
            {
                this.gpios.Add(new Input(InputOutputAddress, i, this));
            }

            for (byte i = InputCount; i < InputCount + OutputCount; i++)
            {
                this.gpios.Add(new Output(InputOutputAddress, i, this));
            }

            this.gpsShortCircuit = new Input(GpsAntennaAddress, ShortCircuitPin, this);
            this.gpsCurrentDetection = new Input(GpsAntennaAddress, CurrentDetectionPin, this);

            for (byte i = 0; i < GraphicControlPinCount; i++)
            {
                this.graphicControlOutputs.Add(new Output(GraphicControlAddress, i, this));
            }
        }
    }
}