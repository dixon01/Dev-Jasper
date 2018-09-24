// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JidaBoard.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JidaBoard type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32
{
    using System;

    using Kontron.Jida32.I2C;
    using Kontron.Jida32.IO;
    using Kontron.Jida32.WD;

    /// <summary>
    /// A hardware board in the JIDA API.
    /// </summary>
    public class JidaBoard : IDisposable
    {
        private IntPtr handle;

        private Watchdog watchdog;

        /// <summary>
        /// Initializes a new instance of the <see cref="JidaBoard"/> class.
        /// </summary>
        /// <param name="handle">
        /// The handle to the board.
        /// </param>
        internal JidaBoard(IntPtr handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets the number of I2C busses on this board.
        /// </summary>
        /// <exception cref="JidaException">
        /// if <see cref="Close"/> was called before.
        /// </exception>
        public int I2CCount
        {
            get
            {
                this.CheckInitialized();
                return NativeMethods.JidaI2CCount(this.handle);
            }
        }

        /// <summary>
        /// Gets the number of I/O ports on this board.
        /// A port consists of 8 inputs and/or outputs.
        /// </summary>
        /// <exception cref="JidaException">
        /// if <see cref="Close"/> was called before.
        /// </exception>
        public int IOCount
        {
            get
            {
                this.CheckInitialized();
                return NativeMethods.JidaIOCount(this.handle);
            }
        }

        /// <summary>
        /// Gets the watchdog or null if none is available.
        /// </summary>
        public Watchdog Watchdog
        {
            get
            {
                this.CheckInitialized();
                if (this.watchdog != null)
                {
                    return this.watchdog;
                }

                if (NativeMethods.JidaWDogCount(this.handle) == 0)
                {
                    return null;
                }

                if (!NativeMethods.JidaWDogIsAvailable(this.handle, 0))
                {
                    return null;
                }

                this.watchdog = new Watchdog(this.handle, 0);
                return this.watchdog;
            }
        }

        /// <summary>
        /// Gets the <see cref="I2CBus"/> with the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="I2CBus"/>.
        /// </returns>
        /// <exception cref="JidaException">
        /// if <see cref="Close"/> was called before.
        /// </exception>
        public I2CBus GetI2CBus(int index)
        {
            this.CheckInitialized();
            if (!NativeMethods.JidaI2CIsAvailable(this.handle, index))
            {
                return null;
            }

            return new I2CBus(this.handle, index);
        }

        /// <summary>
        /// Gets the inputs and outputs of a given port.
        /// </summary>
        /// <param name="index">
        /// The index (value between 0 and <see cref="IOCount"/> - 1).
        /// </param>
        /// <returns>
        /// The <see cref="InputOutputs"/>.
        /// </returns>
        /// <exception cref="JidaException">
        /// if <see cref="Close"/> was called before.
        /// </exception>
        public InputOutputs GetIOs(int index)
        {
            this.CheckInitialized();
            int data = 0;
            if (!NativeMethods.JidaIOGetDirection(this.handle, index, ref data))
            {
                return null;
            }

            return new InputOutputs(this.handle, index, this);
        }

        /// <summary>
        /// Closes this board. Any subsequent calls to this class will result in
        /// an <see cref="JidaException"/> being thrown.
        /// </summary>
        public void Close()
        {
            NativeMethods.JidaBoardClose(this.handle);
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Checks if this board was initialized properly.
        /// </summary>
        /// <exception cref="JidaException">
        /// if <see cref="Close"/> was called before.
        /// </exception>
        internal void CheckInitialized()
        {
            if (this.handle == IntPtr.Zero)
            {
                throw new JidaException("Board is already closed");
            }
        }
    }
}