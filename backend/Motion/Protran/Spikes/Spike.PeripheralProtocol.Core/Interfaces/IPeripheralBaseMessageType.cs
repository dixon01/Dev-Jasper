namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The PeripheralBaseMessage interface.</summary>
    /// <typeparam name="TMessageType"></typeparam>
    public interface IPeripheralBaseMessageType<TMessageType> : IChecksum 
    {
        #region Public Properties

        /// <summary>Gets or sets the header.</summary>
        PeripheralHeader<TMessageType> Header { get; set; }

        #endregion
    }
}