// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralImageUpdateStart.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPeripheralImageUpdateStart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    public interface IPeripheralImageUpdateStart : IPeripheralBaseMessage
    {
        ushort Records { get; set; }
    }

    public interface IPeripheralImageStatus : IPeripheralBaseMessage
    {
        PeripheralImageStatusType Status { get; set; }
    }
}