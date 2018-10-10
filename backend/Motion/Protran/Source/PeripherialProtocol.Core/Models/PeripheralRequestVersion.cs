// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralRequestVersion.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral request for version. Response is PeripheralVersionInfo.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralRequestVersion : PeripheralBaseMessage
    {
        /// <summary>The size.</summary>
        public new const int Size = PeripheralBaseMessage.Size;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PeripheralRequestVersion" /> class. Initializes a new instance of
        ///     the <see cref="PeripheralVersionInfo" /> class.
        /// </summary>
        public PeripheralRequestVersion()
            : base(PeripheralMessageType.RequestVersion)
        {
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralRequestVersion"/> class.</summary>
        /// <param name="address">The header address.</param>
        public PeripheralRequestVersion(ushort address)
            : base(PeripheralMessageType.RequestVersion)
        {
            this.Header.Address = address;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }
    }
}