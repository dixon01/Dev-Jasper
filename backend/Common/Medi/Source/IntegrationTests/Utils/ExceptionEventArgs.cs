// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Event arguments that contain an exception object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Utils
{
    using System;

    /// <summary>
    /// Event arguments that contain an exception object.
    /// </summary>
    [Serializable]
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }
    }
}