// PresentationPlayLogging
// PresentationPlayLogging.Core
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.Core.Interfaces
{
    using System;

    /// <summary>The InfotransitPresentationInfo interface.</summary>
    public interface IInfotransitPresentationInfo : IPresentationInfo
    {
        /// <summary>Gets or sets the expected playback duration if known.</summary>
        long PlayedDuration { get; set; }

        /// <summary>Gets or sets the playback duration if known.</summary>
        long Duration { get; set; }

        /// <summary>Gets or sets a value indicating whether is play interrupted.</summary>
        bool IsPlayInterrupted { get; set; }

        /// <summary>Gets or sets the passenger count if available.</summary>
        int PassengerCount { get; set; }

        /// <summary>Gets or sets when the play started.</summary>
        DateTime? PlayStarted { get; set; }

        /// <summary>Gets or sets when the play stopped.</summary>
        DateTime? PlayStopped { get; set; }
    }
}