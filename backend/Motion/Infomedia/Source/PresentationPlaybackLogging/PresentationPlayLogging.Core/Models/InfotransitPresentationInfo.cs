// InfotransitPresentationInfo
// Infomedia.Entities
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.Core.Models
{
    using System;
    using System.Xml.Serialization;

    using Luminator.PresentationPlayLogging.Core.Interfaces;

    /// <summary>The Infotainment presentation info used for Presentation Play logging to CSV.</summary>
    [Serializable]
    public class InfotransitPresentationInfo : IInfotransitPresentationInfo
    {
        /// <summary>Initializes a new instance of the <see cref="InfotransitPresentationInfo" /> class.</summary>
        public InfotransitPresentationInfo()
        {
            this.Created = DateTime.Now;
            this.ResourceId = string.Empty;
            this.FileName = string.Empty;
            this.StartedLatitude = string.Empty;
            this.StartedLongitude = string.Empty;
            this.StoppedLatitude = string.Empty;
            this.StoppedLongitude = string.Empty;
            this.PassengerCount = 0;
            this.VehicleId = string.Empty;
            this.Route = string.Empty;
            this.PlayStarted = DateTime.Now;
            this.IsPlayInterrupted = false;
            this.Duration = 0;
            this.PlayedDuration = 0;
            this.Trip = string.Empty;
        }

        /// <summary>Gets or sets the record created timestamp.</summary>
        public DateTime Created { get; set; }

        /// <summary>Gets or sets the play duration time in seconds.</summary>
        public long Duration { get; set; }

        /// <summary>Gets or sets the expected play duration if known.</summary>
        public long PlayedDuration { get; set; }

        /// <summary>Gets or sets the resource file name.</summary>
        public string FileName { get; set; }

        /// <summary>Gets or sets a value indicating whether is play interrupted.</summary>
        public bool IsPlayInterrupted { get; set; }

        /// <summary>
        ///     Test if the minimum fields have been defined and the model contains valid values.
        /// </summary>
        [XmlIgnore]
        public virtual bool IsValid => !string.IsNullOrEmpty(this.FileName) && !string.IsNullOrEmpty(this.UnitName);

        /// <summary>Gets or sets the passenger count if available.</summary>
        public int PassengerCount { get; set; }

        /// <summary>Gets or sets when the play started.</summary>
        public DateTime? PlayStarted { get; set; }

        /// <summary>Gets or sets when the play stopped.</summary>
        public DateTime? PlayStopped { get; set; }

        /// <summary>Gets or sets the resource id.</summary>
        public string ResourceId { get; set; }

        /// <summary>Gets or sets the route.</summary>
        public string Route { get; set; }

        /// <summary>Gets or sets the started latitude.</summary>
        public string StartedLatitude { get; set; }

        /// <summary>Gets or sets the started longitude.</summary>
        public string StartedLongitude { get; set; }

        /// <summary>Gets or sets the stopped latitude.</summary>
        public string StoppedLatitude { get; set; }

        /// <summary>Gets or sets the stopped longitude.</summary>
        public string StoppedLongitude { get; set; }

        /// <summary>Gets or sets the trip id or name if known.</summary>
        public string Trip { get; set; }

        /// <summary>Gets or sets the unit name.</summary>
        public string UnitName { get; set; } = Environment.MachineName;

        /// <summary>Gets or sets the vehicle id.</summary>
        public string VehicleId { get; set; }
    }
}