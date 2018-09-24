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

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral request for version. Response is PeripheralVersionInfo.</summary>
    /// <typeparam name="TMessageType"></typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralRequestVersion<TMessageType> : PeripheralBaseMessage<TMessageType>, IPeripheralRequestVersion<TMessageType>
    {        
        /// <summary>Initializes a new instance of the <see cref="PeripheralRequestVersion{TMessageType}"/> class.</summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="systemMessageType">The system message type.</param>
        public PeripheralRequestVersion(TMessageType messageType, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Unknown)
            : base(messageType, systemMessageType)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralRequestVersion{TMessageType}"/> class.</summary>
        /// <param name="header">The header.</param>
        public PeripheralRequestVersion(PeripheralHeader<TMessageType> header)
            : base(header)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralRequestVersion{TMessageType}"/> class.</summary>
        public PeripheralRequestVersion()
        {
        }
    }
}