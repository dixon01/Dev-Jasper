namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using System.IO.Ports;

    public interface ISerialPortSettings
    {
        /// <summary>
        ///     Gets or sets the COM port name.
        /// </summary>
        string ComPort { get; set; }

        /// <summary>
        ///     Gets or sets the baud rate.
        /// </summary>
        int BaudRate { get; set; }

        /// <summary>
        ///     Gets or sets the data bits.
        /// </summary>
        int DataBits { get; set; }

        /// <summary>
        ///     Gets or sets the parity.
        /// </summary>
        Parity Parity { get; set; }

        /// <summary>
        ///     Gets or sets the stop bits.
        /// </summary>
        StopBits StopBits { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether DTR control is enabled.
        /// </summary>
        bool DtrControl { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether parity should be checked.
        /// </summary>
        //bool FParity { get; set; }

        /// <summary>Gets or sets the refresh interval mili seconds.</summary>
        /// <summary>
        ///     Gets or sets a value indicating whether RTS control is enabled.
        /// </summary>
        bool RtsControl { get; set; }
    }
}