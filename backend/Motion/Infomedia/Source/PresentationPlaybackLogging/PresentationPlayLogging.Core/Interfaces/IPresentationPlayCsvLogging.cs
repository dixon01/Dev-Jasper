// PresentationPlayLogging
// PresentationPlayLogging.Core
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.Core.Interfaces
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PresentationPlayLogging.Config;

    /// <summary>The PresentationPlayCsvLogging interface.</summary>
    /// <typeparam name="T"></typeparam>
    public interface IPresentationPlayCsvLogging<T> : IDisposable
        where T : class, IPresentationInfo
    {
        /// <summary>The feedback message event when a medi message is received.</summary>
        event EventHandler<MessageEventArgs<UnitsFeedBackMessage<ScreenChanges>>> OnFeedbackMessageReceived;

        /// <summary>The on vehicle position changed.</summary>
        event EventHandler<VehiclePositionMessage> OnVehiclePositionChanged;

        /// <summary>The on Ximple message received.</summary>
        event EventHandler<Ximple> OnXimpleMessageReceived;

        /// <summary>Gets the Presentation Logging Configuration.</summary>
        PresentationPlayLoggingConfig Config { get; }

        /// <summary>Gets a value indicating whether medi is initialized.</summary>
        bool IsMediInitialized { get; }

        /// <summary>Gets a value indicating last VehiclePositionMessage.</summary>
        VehiclePositionMessage LastVehiclePositionMessage { get; set; }

        /// <summary>Initialize the Configuration. Null for default</summary>
        /// <param name="config"></param>
        void InitConfig(PresentationPlayLoggingConfig config = null);
    }
}