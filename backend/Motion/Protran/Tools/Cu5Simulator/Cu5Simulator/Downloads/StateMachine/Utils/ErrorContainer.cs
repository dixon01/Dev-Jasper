// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorContainer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils
{
    using Gorba.Common.Protocols.Ctu.Responses;

    /// <summary>
    /// Container of the error to be applyed to
    /// a download process.
    /// </summary>
    public sealed class ErrorContainer
    {
        private static readonly ErrorContainer ErrorContainerInstance = new ErrorContainer();

        private ErrorContainer()
        {
            this.CleanError();
        }

        /// <summary>
        /// Gets the unique instance of the object ErrorContainer.
        /// </summary>
        public static ErrorContainer Instance
        {
            get 
            {
                return ErrorContainerInstance; 
            }
        }

        /// <summary>
        /// Gets or sets FileIndex.
        /// </summary>
        public int FileIndex { get; set; }

        /// <summary>
        /// Gets or sets ErrorCode.
        /// </summary>
        public DownloadStatusCode ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets FileAbsName.
        /// </summary>
        public string FileAbsName { get; set; }

        /// <summary>
        /// Clean any error to be applyed to a targeted download process.
        /// </summary>
        public void CleanError()
        {
            this.FileAbsName = string.Empty;
        }
    }
}
