// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioStatusMessage.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Medi.Core.Messages
{
    using System;
    using System.Linq;

    /// <summary>The audio status message.</summary>
    [Serializable]
    public class AudioStatusMessage
    {
        #region Public Properties

        public AudioStatusMessage()
        {
            this.LastUpdated = DateTime.Now;
        }

        /// <summary>Gets or sets the last updated, null if uninitialized</summary>
        public DateTime ?LastUpdated { get; set; }

        /// <summary>Gets a value indicating whether audio active.</summary>
        public bool AudioActive
        {
            get
            {
                return this.ExteriorActive || this.InteriorActive;
            }
        }

        /// <summary>Gets or sets a value indicating whether canned msg active.</summary>
        [Obsolete("ToDo set Someday")]
        public bool CannedMsgActive { get; set; }

        /// <summary>Gets or sets a value indicating whether exterior active.</summary>
        public bool ExteriorActive { get; set; }

        /// <summary>Gets or sets the exterior noise level.</summary>
        public int ExteriorNoiseLevel { get; set; } // "Exterior noise level, from -100.0 to 100.0"

        /// <summary>Gets or sets the exterior volume.</summary>
        public int ExteriorVolume { get; set; } // "Current exterior volume, from 0 to 100" 

        /// <summary>Gets or sets the hardware version.</summary>
        public string HardwareVersion { get; set; }

        /// <summary>Gets or sets a value indicating whether the fields have been initialized.</summary>
        public bool Initialized { get; set; }

        /// <summary>Gets or sets a value indicating whether interior active.</summary>
        public bool InteriorActive { get; set; }

        /// <summary>Gets or sets the interior noise level.</summary>
        public int InteriorNoiseLevel { get; set; } // "Interior noise level, from -100.0 to 100.0"

        /// <summary>Gets or sets the interior volume.</summary>
        public int InteriorVolume { get; set; } // "Current interior volume, from 0 to 100" 

        /// <summary>Gets or sets a value indicating whether line in active.</summary>
        public bool LineInActive { get; set; }

        /// <summary>Gets or sets the lockout duration.</summary>
        public int LockoutDuration { get; set; } // Seconds since lockout triggered"

        /// <summary>Gets or sets a value indicating whether ptt active.</summary>
        public bool PttActive { get; set; }

        /// <summary>Gets or sets a value indicating whether ptt locked.</summary>
        public bool PttLocked { get; set; }

        /// <summary>Gets or sets the serial number.</summary>
        public string SerialNumber { get; set; }

        /// <summary>Gets or sets the software version.</summary>
        public string SoftwareVersion { get; set; }

        /// <summary>Gets or sets a value indicating whether test active.</summary>
        public bool TestActive { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return typeof(AudioStatusMessage).GetProperties().Aggregate(string.Empty, (current, p) => current + string.Format("  {0}={1}\r\n", p.Name, p.GetValue(this)));
        }

        #endregion
    }
}