// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralImageRecord.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPeripheralImageRecord type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    public interface IPeripheralImageRecord : IPeripheralBaseMessage
    {
        byte[] Buffer { get; set; }
    }
}