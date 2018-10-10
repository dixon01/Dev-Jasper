// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceClass.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Vdv301
{
    /// <summary>
    /// The types of device class in VDV301
    /// </summary>
    public enum DeviceClass
    {
        /// <summary>
        /// The on board unit.
        /// </summary>
        OnBoardUnit,

        /// <summary>
        /// The side display.
        /// </summary>
        SideDisplay,

        /// <summary>
        /// The front display.
        /// </summary>
        FrontDisplay,

        /// <summary>
        /// The interior display.
        /// </summary>
        InteriorDisplay,

        /// <summary>
        /// The validator.
        /// </summary>
        Validator,

        /// <summary>
        /// The ticket vending machine.
        /// </summary>
        TicketVendingMachine,

        /// <summary>
        /// The announcement system.
        /// </summary>
        AnnouncementSystem,

        /// <summary>
        /// The MMI.
        /// </summary>
        MMI,

        /// <summary>
        /// The video system.
        /// </summary>
        VideoSystem,

        /// <summary>
        /// The APC.
        /// </summary>
        APC,

        /// <summary>
        /// The mobile interface.
        /// </summary>
        MobileInterface,

        /// <summary>
        /// The other.
        /// </summary>
        Other,

        /// <summary>
        /// The test device.
        /// </summary>
        TestDevice,
    }
}
