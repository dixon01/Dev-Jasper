// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralHandler.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <summary>
//   The PeripheralHandler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Interfaces
{
    using System.IO;

    /// <summary>The PeripheralHandler interface.</summary>
    public interface IPeripheralHandler
    {
        #region Public Methods and Operators

        /// <summary>The calculate check sum.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="int"/>.</returns>
        int CalculateCheckSum(byte[] bytes);

        /// <summary>The read.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="userNetworkToHostByteOrder">The user network to host byte order.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="object"/>.</returns>
        object Read<T>(byte[] bytes, Stream stream, bool userNetworkToHostByteOrder) where T : class, IPeripheralBaseMessage;

        /// <summary>The verify checksum.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool VerifyChecksum(byte[] bytes);

        /// <summary>The write.</summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int"/>.</returns>
        int Write<T>(T model, Stream stream) where T : IPeripheralBaseMessage;

        #endregion
    }
}