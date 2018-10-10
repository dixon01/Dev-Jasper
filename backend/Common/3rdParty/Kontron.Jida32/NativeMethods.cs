// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NativeMethods type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The native methods of the JIDA API.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// The JIDA DLL version major number.
        /// </summary>
        public static readonly int JidaDllVersionMajor = 4;

        /// <summary>
        /// This function returns the version number of the JIDA API.
        /// This is the only function that can be called before <see cref="JidaDllInitialize"/>.
        /// To check for version compliance you can compare the major number of the
        /// <see cref="JidaDllVersionMajor"/>.
        /// </summary>
        /// <returns>
        /// The major version number of the API is located in the upper 16 bits and
        /// the minor version number of the API in the lower 16 bits of the return value.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern int JidaDllGetVersion();

        /// <summary>
        /// This initializes the JIDA API for use by the application. A failure indicates that the driver is not
        /// properly installed or outdated. Calls to <see cref="JidaDllInitialize"/> and
        /// <see cref="JidaDllUninitialize"/> can be nested but must be balanced.
        /// </summary>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaDllInitialize();

        /// <summary>
        /// This function must be called before the application terminates after it has successfully called
        /// <see cref="JidaDllInitialize"/>. Calls to <see cref="JidaDllInitialize"/> and
        /// <see cref="JidaDllUninitialize"/> can be nested but must be balanced.
        /// </summary>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaDllUninitialize();

        /// <summary>
        /// This function installs or uninstalls the underlying JIDA driver. You can call this function from your
        /// setup program or if <see cref="JidaDllInitialize"/> fails. Note that the system may need to be rebooted
        /// for the changes to become effective. Under NT you need administrative rights to use this function
        /// successfully. After this function succeeds you can call <see cref="JidaDllInitialize"/> a second time. If
        /// this function fails again then a reboot is required to load the drivers (under Windows 9x). If it
        /// succeeds then the drivers have been loaded dynamically and JIDA is ready to be used (under
        /// Windows NT and Windows CE).
        /// </summary>
        /// <param name="install">
        /// TRUE (1) on for install. FALSE (0) for uninstall.
        /// </param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaDllInstall(bool install);

        /// <summary>
        /// Gets the number of boards that fit the given class.
        /// </summary>
        /// <param name="boardClassName">
        /// The class name of the board.
        /// This value can be NULL in which case the total number of boards will be returned.
        /// </param>
        /// <param name="flags">
        /// Can be any combination of the following flags:
        /// JIDA_BOARD_OPEN_FLAGS_PRIMARYONLY
        /// Count only boards that do have the given class name as a primary class.
        /// Otherwise any boards that fit the given class in any way will be returned.
        /// </param>
        /// <returns>
        /// Number of available boards.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern int JidaBoardCount(string boardClassName, int flags);

        /// <summary>
        /// Opens a board that fits the given class.
        /// </summary>
        /// <param name="boardClassName">
        /// The class name of the board.
        /// This value can be NULL in which case the total number of boards will be returned.
        /// </param>
        /// <param name="boardNumber">
        /// See <see cref="JidaBoardCount"/>.
        /// </param>
        /// <param name="flags">
        /// See above <see cref="JidaBoardCount"/>.
        /// </param>
        /// <param name="pointerToBoardHandle">
        /// Pointer to a location the will receive the handle to the board.
        /// </param>
        /// <returns>
        /// Number of available boards.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaBoardOpen(
            string boardClassName, int boardNumber, int flags, ref IntPtr pointerToBoardHandle);

        /// <summary>
        /// Closes the connection to a board.
        /// </summary>
        /// <param name="boardHandle">
        /// The board handle.
        /// </param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaBoardClose(IntPtr boardHandle);

        /// <summary>
        /// Retrieves the number of I2C buses on the board.
        /// </summary>
        /// <param name="boardHandle">
        /// The board handle.
        /// </param>
        /// <returns>
        /// Number of available I2C buses.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern int JidaI2CCount(IntPtr boardHandle);

        /// <summary>
        /// Queries whether the I2C bus of the given type is available.
        /// </summary>
        /// <param name="boardHandle">
        /// The board handle.
        /// </param>
        /// <param name="busNumber">
        /// Zero-based number of the I2C bus.
        /// </param>
        /// <returns>
        /// TRUE (1) if the give type of I2C bus is present. FALSE (0) otherwise
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaI2CIsAvailable(IntPtr boardHandle, int busNumber);

        /// <summary>
        /// Queries the bus type of the given I2C bus.
        /// </summary>
        /// <param name="boardHandle">
        /// The board handle.
        /// </param>
        /// <param name="busNumber">
        /// Zero-based number of the I2C bus.
        /// </param>
        /// <returns>
        /// The type:
        /// JIDA_I2C_TYPE_UNKNOWN  unknown or special purposes
        /// JIDA_I2C_TYPE_PRIMARY  primary I2C bus
        /// JIDA_I2C_TYPE_SMB      system management bus
        /// JIDA_I2C_TYPE_JILI     JILI interface
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern int JidaI2CType(IntPtr boardHandle, int busNumber);

        /// <summary>
        /// Reads bytes from a device on the I2C bus.
        /// </summary>
        /// <param name="boardHandle">
        /// The board handle.
        /// </param>
        /// <param name="busNumber">
        /// Zero-based number of the I2C bus.
        /// </param>
        /// <param name="address">
        /// Address of the device on the I2C bus, the full 8 bits as it is written to the bus.
        /// Bit 0 should be always 1 to read from regular I2C devices.
        /// </param>
        /// <param name="bytes">
        /// Pointer to location that will receive the bytes.
        /// </param>
        /// <param name="length">
        /// Number of bytes to read.
        /// </param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern unsafe bool JidaI2CRead(
            IntPtr boardHandle, int busNumber, byte address, byte* bytes, int length);

        /// <summary>
        /// Writes bytes to a device on the I2C bus.
        /// WARNING: Improperly using this function with certain buses and devices may cause
        /// PERMANENT DAMAGE to your system and may prevent your board from booting. The
        /// most likely scenario is to accidentally overwrite the configuration data in the EEPROM that is
        /// attached to the SMBus and located on the RAM module. This may make the RAM module
        /// permanently inaccessible to the system and will therefore stop the boot process.
        /// </summary>
        /// <param name="boardHandle">
        /// The board handle.
        /// </param>
        /// <param name="busNumber">
        /// Zero-based number of the I2C bus.
        /// </param>
        /// <param name="address">
        /// Address of the device on the I2C bus, the full 8 bits as it is written to the bus.
        /// Bit 0 should be always 0 for regular I2C devices.
        /// </param>
        /// <param name="bytes">
        /// Pointer to location that contains the bytes.
        /// </param>
        /// <param name="length">
        /// Number of bytes to write.
        /// </param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern unsafe bool JidaI2CWrite(
            IntPtr boardHandle, int busNumber, byte address, byte* bytes, int length);

        /// <summary>
        /// Writes bytes to a device on the I2C bus, then reads the specified number of bytes in combined
        /// mode. The difference between using this function instead of the separate write and read
        /// functions is that at the end of the write this function does not include a STOP condition. The
        /// second START condition for the read is present.
        /// WARNING: Improperly using this function with certain buses and devices may cause
        /// PERMANENT DAMAGE to your system and may prevent your board from booting. The
        /// most likely scenario is to accidentally overwrite the configuration data in the EEPROM that is
        /// attached to the SMBus and located on the RAM module. This may make the RAM module
        /// permanently inaccessible to the system and will therefore stop the boot process.
        /// </summary>
        /// <param name="boardHandle">
        /// Board handle.
        /// </param>
        /// <param name="busNumber">
        /// Zero-based number of the I2C bus.
        /// </param>
        /// <param name="address">
        /// Address of the device on the I2C bus, the full 8 bits as it is written to the bus.
        /// Bit 0 should be always 0 for regular I2C devices. During the read cycle this functions
        /// sets Bit 0 automatically.
        /// </param>
        /// <param name="writeBytes">
        /// Pointer to location that contains the bytes.
        /// </param>
        /// <param name="writeLength">
        /// Number of bytes to write.
        /// </param>
        /// <param name="readBytes">
        /// Pointer to location that will receive the bytes.
        /// </param>
        /// <param name="readLength">
        /// Number of bytes to read.
        /// </param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern unsafe bool JidaI2CWriteReadCombined(
            IntPtr boardHandle,
            int busNumber,
            byte address,
            byte* writeBytes,
            int writeLength,
            byte* readBytes,
            int readLength);

        /// <summary>
        /// On many boards this returns 0.
        /// </summary>
        /// <param name="boardHandle">
        /// Board handle.
        /// </param>
        /// <returns>
        /// Number of available IO Ports.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern int JidaIOCount(IntPtr boardHandle);

        /// <summary>
        /// Reads the current state of the IO Port. This includes the input and output values.
        /// </summary>
        /// <param name="boardHandle">
        /// Board handle.
        /// </param>
        /// <param name="portNumber">
        /// Zero-based number of IO Ports.
        /// </param>
        /// <param name="pointerToData">
        /// Pointer to read value.
        /// </param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaIORead(IntPtr boardHandle, int portNumber, ref int pointerToData);

        /// <summary>
        /// Writes to the output pins of the IO Port.
        /// </summary>
        /// <param name="boardHandle">
        /// Board handle.
        /// </param>
        /// <param name="portNumber">
        /// Zero-based number of IO Ports.
        /// </param>
        /// <param name="data">
        /// Value to write to the port.
        /// </param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaIOWrite(IntPtr boardHandle, int portNumber, int data);

        /// <summary>
        /// Reads the current direction of the IO Port pins.
        /// </summary>
        /// <param name="boardHandle">
        /// Board handle.
        /// </param>
        /// <param name="portNumber">
        /// Zero-based number of IO Ports.
        /// </param>
        /// <param name="pointerToData">
        /// Pointer to the location that will receive the current direction of the port pins. A 0 bit
        /// indicates an OUTPUT, a 1 bit indicates an INPUT pin in the corresponding bit position.
        /// </param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaIOGetDirection(IntPtr boardHandle, int portNumber, ref int pointerToData);

        /// <summary>
        /// Always returns 0 or 1 in the current version of JIDA.
        /// </summary>
        /// <param name="boardHandle">
        /// Board handle.
        /// </param>
        /// <returns>
        /// Number of available watchdogs on this board.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern int JidaWDogCount(IntPtr boardHandle);

        /// <summary>
        /// Queries whether the watchdog of the given type is available.
        /// Currently only type 0 is implemented.
        /// </summary>
        /// <param name="boardHandle">Board handle.</param>
        /// <param name="type">Must be 0.</param>
        /// <returns>
        /// TRUE (1) if the given type of watchdog is present. FALSE (0) otherwise.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaWDogIsAvailable(IntPtr boardHandle, int type);

        /// <summary>
        /// This function indicates that the application is still working properly
        /// and must be called on a continues basis by the application to ensure
        /// that the system will not be restarted.
        /// This applies only after that watchdog has been activated.
        /// </summary>
        /// <param name="boardHandle">Board handle.</param>
        /// <param name="type">Must be 0.</param>
        /// <returns>
        /// TRUE (1) on success. FALSE (0) on failure.
        /// </returns>
        [DllImport("jida.dll")]
        internal static extern bool JidaWDogTrigger(IntPtr boardHandle, int type);
    }
}