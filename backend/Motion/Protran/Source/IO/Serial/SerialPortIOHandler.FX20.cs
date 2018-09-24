// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortIOHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortIOHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Serial
{
    using System;
    using System.IO.Ports;

    /// <summary>
    /// Default implementation for <see cref="ISerialPortIOs"/> that
    /// uses a <see cref="SerialPort"/> to provide the states.
    /// </summary>
    public partial class SerialPortIOHandler : ISerialPortIOs, IDisposable
    {
        private SerialPort serialPort;

        /// <summary>
        /// Event that is risen when the <see cref="CtsHolding"/> changes.
        /// </summary>
        public event EventHandler CtsChanged;

        /// <summary>
        /// Event that is risen when the <see cref="DsrHolding"/> changes.
        /// </summary>
        public event EventHandler DsrChanged;

        /// <summary>
        /// Event that is risen when the <see cref="DtrEnable"/> changes.
        /// </summary>
        public event EventHandler DtrChanged;

        /// <summary>
        /// Event that is risen when the <see cref="RtsEnable"/> changes.
        /// </summary>
        public event EventHandler RtsChanged;

        /// <summary>
        /// Gets a value indicating whether the CTS line (Clear To Send) is held.
        /// </summary>
        public bool CtsHolding
        {
            get
            {
                return this.IsAvailable && this.SerialPort.CtsHolding;
            }
        }

        /// <summary>
        /// Gets a value indicating whether DSR line (Data Set Ready) is held.
        /// </summary>
        public bool DsrHolding
        {
            get
            {
                return this.IsAvailable && this.SerialPort.DsrHolding;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether DTR line (Data Terminal Ready) is enabled.
        /// </summary>
        public bool DtrEnable
        {
            get
            {
                return this.IsAvailable && this.SerialPort.DtrEnable;
            }

            set
            {
                if (this.DtrEnable == value || !this.IsAvailable)
                {
                    return;
                }

                this.SerialPort.DtrEnable = value;
                this.RaiseDtrChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether RTS line (Request To Send) is enabled.
        /// </summary>
        public bool RtsEnable
        {
            get
            {
                return this.IsAvailable && this.SerialPort.RtsEnable;
            }

            set
            {
                if (this.RtsEnable == value || !this.IsAvailable)
                {
                    return;
                }

                this.SerialPort.RtsEnable = value;
                this.RaiseRtsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the serial port. This object is not owned by this class, you need to close
        /// it manually, <see cref="Dispose"/> of this class doesn't do this for you.
        /// Setting the serial port can also trigger events in this class.
        /// </summary>
        public SerialPort SerialPort
        {
            get
            {
                return this.serialPort;
            }

            set
            {
                if (this.serialPort == value)
                {
                    return;
                }

                if (this.serialPort != null)
                {
                    this.serialPort.PinChanged -= this.SerialPortOnPinChanged;
                }

                this.serialPort = value;

                if (this.serialPort == null)
                {
                    return;
                }

                this.serialPort.PinChanged += this.SerialPortOnPinChanged;

                if (this.CtsHolding)
                {
                    this.RaiseCtsChanged(EventArgs.Empty);
                }

                if (this.DsrHolding)
                {
                    this.RaiseDsrChanged(EventArgs.Empty);
                }

                if (this.DtrEnable)
                {
                    this.RaiseDtrChanged(EventArgs.Empty);
                }

                if (this.RtsEnable)
                {
                    this.RaiseRtsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the serial port is available.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return this.serialPort != null && this.serialPort.IsOpen;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.SerialPort = null;
        }

        /// <summary>
        /// Raises the <see cref="CtsChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCtsChanged(EventArgs e)
        {
            var handler = this.CtsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DsrChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseDsrChanged(EventArgs e)
        {
            var handler = this.DsrChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DtrChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseDtrChanged(EventArgs e)
        {
            var handler = this.DtrChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RtsChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseRtsChanged(EventArgs e)
        {
            var handler = this.RtsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SerialPortOnPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            // SerialPinChange is a flag enum, but not marked as one:
            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            if ((e.EventType | SerialPinChange.CtsChanged) == SerialPinChange.CtsChanged)
            {
                this.RaiseCtsChanged(e);
            }

            if ((e.EventType | SerialPinChange.DsrChanged) == SerialPinChange.DsrChanged)
            {
                this.RaiseDsrChanged(e);
            }

            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
        }
    }
}