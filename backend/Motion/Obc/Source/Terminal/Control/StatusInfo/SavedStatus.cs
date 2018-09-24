// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavedStatus.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SavedStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using System;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    ///   This class is used to save a drive status
    ///   It will be used when TerminalControl crashes/restarts during a duty block or special destination drive
    ///   Not all data from a drive will be stored. Just the data which the user enters will be stored.
    ///   These data are:
    ///   Language
    ///   Block / Special destination drive
    ///   Block / Special destination number
    ///   Additional and School drive
    /// </summary>
    public class SavedStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavedStatus"/> class.
        /// </summary>
        public SavedStatus()
        {
            this.SavedTimeUtc = TimeProvider.Current.UtcNow;
            this.Language = LanguageManager.Instance.CurrentLanguage.Name;
            this.SpecDestName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the saved time in UTC.
        /// </summary>
        public DateTime SavedTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the driver number.
        /// </summary>
        public int DriverNumber { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the spec destination code.
        /// </summary>
        public int SpecDestCode { get; set; }

        /// <summary>
        /// Gets or sets the spec destination service number.
        /// </summary>
        public int SpecDestServiceNbr { get; set; }

        /// <summary>
        /// Gets or sets the spec destination name.
        /// </summary>
        public string SpecDestName { get; set; }

        /// <summary>
        /// Gets or sets the block number.
        /// </summary>
        public int BlockNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current drive is driving school.
        /// </summary>
        public bool IsDrivingSchool { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current drive is an additional drive.
        /// </summary>
        public bool IsAdditionalDrive { get; set; }

        /// <summary>
        /// Gets a value indicating whether this information is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if ((this.BlockNumber == 0 && this.SpecDestServiceNbr == 0) || (this.DriverNumber == 0))
                {
                    return false;
                }

                return this.SavedTimeUtc.AddMinutes(30) >= TimeProvider.Current.UtcNow;
            }
        }
    }
}