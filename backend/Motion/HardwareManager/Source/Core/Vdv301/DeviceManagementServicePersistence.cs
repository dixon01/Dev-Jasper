// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceManagementServicePersistence.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeviceManagementServicePersistence type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Vdv301
{
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;

    /// <summary>
    /// Persistence class for the <see cref="DeviceManagementService"/>.
    /// Don't use this class outside this namespace, it is only public to support XML serialization.
    /// </summary>
    public class DeviceManagementServicePersistence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceManagementServicePersistence"/> class.
        /// </summary>
        public DeviceManagementServicePersistence()
        {
            this.DeviceId = new IBISIPint
                                {
                                    ErrorCode = ErrorCodeEnumeration.DataNotValid,
                                    ErrorCodeSpecified = true
                                };
        }

        /// <summary>
        /// Gets or sets the device id used by <see cref="IDeviceManagementService.SetDeviceConfiguration"/>.
        /// </summary>
        public IBISIPint DeviceId { get; set; }
    }
}