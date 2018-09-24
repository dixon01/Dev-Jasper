// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceState.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeviceState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    /// <summary>
    /// ISI Device state enumeration used for DeviceState data item.
    /// </summary>
    public enum DeviceState
    {
        /// <summary>
        /// 100 meaning OK
        /// </summary>
        Ok = 100,

        /// <summary>
        /// 207 – Maintenance check request
        /// </summary>
        MaintenanceCheckRequest = 207,

        /// <summary>
        /// 207 – Defective
        /// </summary>
        Defective = 300,

        /// <summary>
        /// 301 – No connection
        /// </summary>
        NoConnection = 301,

        /// <summary>
        /// 303 - Initialization error
        /// </summary>
        InitializationError = 303,

        /// <summary>
        /// 304 - Configuration error
        /// </summary>
        ConfigurationError = 304,

        /// <summary>
        /// 305 - System error
        /// </summary>
        SystemError = 305,

        /// <summary>
        /// 317 - Storage space for activity files is almost full
        /// </summary>
        StorageSpaceForActivityFilesAlmostFull = 317,

        /// <summary>
        /// 318 - Storage space for activity files is full
        /// </summary>
        StorageSpaceForActivityFilesFull = 318,

        /// <summary>
        /// 320 - Parameters default
        /// </summary>
        ParametersDefault = 320,
    }
}
