// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputOutputManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputOutputManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    using Kontron.Jida32;
    using Kontron.Jida32.I2C;

    /// <summary>
    /// Base class for the input and output manager for MGI hardware.
    /// </summary>
    internal abstract class InputOutputManagerBase : IInputOutputManager
    {
        private readonly JidaApi jida;

        private readonly JidaBoard board;

        private readonly I2CBus busI2C;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputOutputManagerBase"/> class.
        /// </summary>
        /// <exception cref="JidaException">
        /// if JIDA was not available or the first board with I/O was not found.
        /// </exception>
        protected InputOutputManagerBase()
        {
            this.jida = new JidaApi();
            if (!this.jida.Initialize())
            {
                throw new JidaException("Couldn't initialize JIDA");
            }

            try
            {
                this.board = this.jida.OpenBoard(JidaApi.BoardClassIO, 0);
                if (this.board == null)
                {
                    throw new JidaException("Couldn't find I/O board #0");
                }

                this.busI2C = this.board.GetI2CBus(0);
                if (this.busI2C == null)
                {
                    throw new JidaException("Couldn't find I2C bus #0");
                }
            }
            catch
            {
                this.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Gets the general button input.
        /// </summary>
        public abstract Input Button { get; }

        /// <summary>
        /// Gets the update LED output.
        /// </summary>
        public abstract Output UpdateLed { get; }

        /// <summary>
        /// Gets the number of general programmable inputs and outputs in the system.
        /// </summary>
        public abstract int GpioCount { get; }

        /// <summary>
        /// Gets the antenna short input.
        /// </summary>
        public abstract Input GpsShortCircuit { get; }

        /// <summary>
        /// Gets the antenna detect input.
        /// </summary>
        public abstract Input GpsCurrentDetection { get; }

        /// <summary>
        /// Gets the given general programmable I/O.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="IOBase"/>.
        /// </returns>
        public abstract IOBase GetGpio(int index);

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
        public abstract Output GetGraphicControlOutput(int index, GraphicControlPin pin);

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
        public abstract Output GetMultiprotocolTransceiverOutput(int index, MultiprotocolTransceiverPin pin);

        /// <summary>
        /// Reads all general programmable I/O's and returns them in an array of flags.
        /// This method is considerably faster than querying every single I/O for its value
        /// using <see cref="IOBase.Read"/>.
        /// A single call to this method does the same amount of I/O operations as
        /// one call to <see cref="IOBase.Read"/>.
        /// </summary>
        /// <returns>
        /// An array with <see cref="IInputOutputManager.GpioCount"/> values. The values correspond to the
        /// I/O returned by <see cref="IInputOutputManager.GetGpio"/> with the given index.
        /// </returns>
        public bool[] ReadAllGpios()
        {
            var gpios = new IOBase[this.GpioCount];
            var allValues = new bool[this.GpioCount];

            for (int i = 0; i < gpios.Length; i++)
            {
                gpios[i] = this.GetGpio(i);
            }

            var address = gpios[0].Address;
            var values = this.ReadValues(address);

            for (int i = 0; i < allValues.Length; i++)
            {
                allValues[i] = (values & (1 << gpios[i].Pin)) == 0;
            }

            return allValues;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.board != null)
            {
                this.board.Dispose();
            }

            this.jida.Dispose();
        }

        /// <summary>
        /// Initializes this manager.
        /// This method is called by the factory before returning the object.
        /// </summary>
        internal void Initialize()
        {
            this.Initialize(this.busI2C);
        }

        /// <summary>
        /// Reads the given I/O.
        /// </summary>
        /// <param name="io">
        /// The I/O.
        /// </param>
        /// <returns>
        /// True if the I/O is set, otherwise false.
        /// </returns>
        internal virtual bool Read(IOBase io)
        {
            var values = this.ReadValues(io.Address);
            return (values & (1 << io.Pin)) == 0;
        }

        /// <summary>
        /// Writes to the given I/O.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        internal virtual void Write(Output output, bool value)
        {
            var inputMask = this.GetInputMask(output.Address);
            lock (this.busI2C)
            {
                var mask = (byte)(1 << output.Pin);
                var values = this.busI2C.ReadByte(output.Address);
                if (value)
                {
                    values &= (byte)~mask;
                }
                else
                {
                    values |= mask;
                }

                this.busI2C.WriteByte(output.Address, (byte)(values | inputMask));
            }
        }

        /// <summary>
        /// Initializes this manager with the given I2C bus.
        /// </summary>
        /// <param name="busI2C">
        /// The I2C bus.
        /// </param>
        protected abstract void Initialize(I2CBus busI2C);

        /// <summary>
        /// Gets the input mask for a certain I2C address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// A byte with all the input bits set to 1 and all output bits set to 0.
        /// </returns>
        protected abstract byte GetInputMask(byte address);

        private byte ReadValues(byte address)
        {
            var mask = this.GetInputMask(address);
            byte values;
            lock (this.busI2C)
            {
                var outputValues = this.busI2C.ReadByte(address);
                values = this.busI2C.WriteReadByte(address, (byte)(outputValues | mask));
            }

            return values;
        }
    }
}