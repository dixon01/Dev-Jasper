// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralBaseMessage.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Types;

    public interface IPeripheralBaseMessage : IPeripheralBaseMessageType<PeripheralMessageType>
    {
    }
}