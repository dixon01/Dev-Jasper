// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputOutputs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputOutputs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32.IO
{
    using System;

    /// <summary>
    /// The inputs and outputs of a <see cref="JidaBoard"/>.
    /// </summary>
    public class InputOutputs
    {
        private readonly IntPtr boardHandle;

        private readonly int index;

        private readonly JidaBoard board;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputOutputs"/> class.
        /// </summary>
        /// <param name="boardHandle">
        /// The board handle.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="board">
        /// The board.
        /// </param>
        internal InputOutputs(IntPtr boardHandle, int index, JidaBoard board)
        {
            this.boardHandle = boardHandle;
            this.index = index;
            this.board = board;
        }

        /// <summary>
        /// Gets the directions of the IO ports.
        /// </summary>
        /// <exception cref="JidaException">
        /// if the underlying <see cref="JidaBoard"/> was closed or the direction couldn't be read.
        /// </exception>
        public IOValues<IODirection> Directions
        {
            get
            {
                this.board.CheckInitialized();
                var data = 0;
                if (!NativeMethods.JidaIOGetDirection(this.boardHandle, this.index, ref data))
                {
                    throw new JidaException("Couldn't get I/O direction");
                }

                var directions = new IOValues<IODirection>();
                for (int i = 0; i < 8; i++)
                {
                    directions[i] = (IODirection)((data >> i) & 1);
                }

                return directions;
            }
        }

        /// <summary>
        /// Reads all input and output values.
        /// </summary>
        /// <returns>
        /// The values of the inputs and outputs.
        /// </returns>
        /// <exception cref="JidaException">
        /// if the underlying <see cref="JidaBoard"/> was closed or the I/O port couldn't be read.
        /// </exception>
        public IOValues<bool> Read()
        {
            this.board.CheckInitialized();
            var data = 0;
            if (!NativeMethods.JidaIORead(this.boardHandle, this.index, ref data))
            {
                throw new JidaException("Couldn't read I/O values");
            }

            var values = new IOValues<bool>();
            for (int i = 0; i < 8; i++)
            {
                values[i] = (data & (1 << i)) != 0;
            }

            return values;
        }

        /// <summary>
        /// Writes all output values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// if <see cref="values"/> is null.
        /// </exception>
        /// <exception cref="JidaException">
        /// if the underlying <see cref="JidaBoard"/> was closed or the I/O port couldn't be written.
        /// </exception>
        public void Write(IOValues<bool> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            this.board.CheckInitialized();

            var data = 0;
            for (int i = 0; i < 8; i++)
            {
                if (values[i])
                {
                    data |= 1 << i;
                }
            }

            if (!NativeMethods.JidaIOWrite(this.boardHandle, this.index, data))
            {
                throw new JidaException("Couldn't write I/O values");
            }
        }
    }
}