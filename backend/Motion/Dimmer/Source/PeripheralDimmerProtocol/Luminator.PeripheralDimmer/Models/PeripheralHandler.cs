// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralHandler.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The peripheral handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.IO;
    using System.Linq;

    using Luminator.PeripheralDimmer.Interfaces;

    /// <summary>The peripheral handler.</summary>
    [Obsolete]
    internal class PeripheralHandler : IPeripheralHandler
    {
        #region Public Methods and Operators

        /// <summary>The calculate check sum.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="int"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CalculateCheckSum(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>The read.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="userNetworkToHostByteOrder">The user network to host byte order.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="object"/>.</returns>
        public object Read<T>(byte[] bytes, Stream stream, bool userNetworkToHostByteOrder) where T : class, IPeripheralBaseMessage
        {
            var model = default(T);
            if (bytes != null && bytes.Length > PeripheralHeader.HeaderSize)
            {
                var bytesWithOutFrame = bytes.SkipWhile(m => m.Equals(DimmerConstants.PeripheralFramingByte)).ToArray();
                var header = bytesWithOutFrame.ToPeripheralHeader(userNetworkToHostByteOrder);
                if (header != null)
                {
                    // include checksum byte or not 
                    var fullMessageSize = bytesWithOutFrame.Length == header.Length ? header.Length : header.Length + 1;
                    var byteArray = bytes.Take(fullMessageSize).ToArray();
                    var checksumByte = byteArray.Last();

                    // var checksum = VerifyChecksum(bytes);
                    // if (checksum == checksumByte)
                    // {
                    // model = byteArray.FromByteArray<T>();
                    // }
                }
            }

            return model;
        }

        /// <summary>The verify checksum.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool VerifyChecksum(byte[] bytes)
        {
            // TODO
            return true;
        }

        /// <summary>The write.</summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Write<T>(T model, Stream stream) where T : IPeripheralBaseMessage
        {
            // var bytes = model.ToByteArray<T>();
            throw new NotImplementedException();
        }

        #endregion
    }
}