// PresentationPlayLogging
// PresentationPlayLogging.Core
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.Core.Interfaces
{
    using System;

    public interface IPresentationInfotransitCsvLogging
    {
        /// <summary>Gets a value indicating whether the logging is started.</summary>
        bool IsStarted { get; }

        /// <summary>Start Logging.</summary>
        /// <exception cref="ApplicationException">Failed startup</exception>
        void Start();

        /// <summary>Stop Logging.</summary>
        void Stop();
    }
}