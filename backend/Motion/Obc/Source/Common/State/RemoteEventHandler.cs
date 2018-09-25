// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteEventHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteEventHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// The remote event handler.
    /// TODO: This class should be renamed since it is now only a receiver for some data messages.
    /// </summary>
    public static class RemoteEventHandler
    {
        static RemoteEventHandler()
        {
            VehicleConfig = new ehConfig();

            MessageDispatcher.Instance.Subscribe<Trip>((s, e) => CurrentTrip = e.Message);
            MessageDispatcher.Instance.Subscribe<Service>((s, e) => CurrentService = e.Message);
            MessageDispatcher.Instance.Subscribe<ExtraService>((s, e) => CurrentExtraService = e.Message);
            MessageDispatcher.Instance.Subscribe<ehConfig>((s, e) => VehicleConfig = e.Message);
        }

        /// <summary>
        /// Gets or sets the device config.
        /// </summary>
        public static ehConfig VehicleConfig { get; set; }

        /// <summary>
        /// Gets the current trip and bus stops
        /// </summary>
        public static Trip CurrentTrip { get; private set; }

        /// <summary>
        /// Gets the current service
        /// </summary>
        public static Service CurrentService { get; private set; }

        /// <summary>
        /// Gets the current extra service
        /// </summary>
        public static ExtraService CurrentExtraService { get; private set; }

        /// <summary>
        /// Initializes this class.
        /// </summary>
        public static void Initialize()
        {
            // this method actually doesn't do anything; everythign is done in the .cctor
            // but this method still needs to be called so the class is initialized
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            VehicleConfig.ToString();
        }
    }
}