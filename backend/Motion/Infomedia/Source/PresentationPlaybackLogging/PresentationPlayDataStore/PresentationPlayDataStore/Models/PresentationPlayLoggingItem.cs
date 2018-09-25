namespace Luminator.PresentationPlayLogging.DataStore.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Luminator.PresentationPlayLogging.DataStore.Interfaces;

    [Table("PresentationPlayLog")]
    public class PresentationPlayLoggingItem : IPresentationPlayLoggingItem
    {
        public PresentationPlayLoggingItem(
            string fileName,
            string startLatitude,
            string startLongitude,
            bool isPlayInterrupted,
            string vehicleId,
            string route)
        {
            this.FileName = fileName;
            this.StartedLatitude = startLatitude;
            this.StartedLongitude = startLongitude;
            this.IsPlayInterrupted = isPlayInterrupted;
            this.VehicleId = vehicleId;
            this.Route = route;
        }

        public PresentationPlayLoggingItem()
        {
            this.Created = DateTime.Now;
        }

        public DateTime Created { get; set; }

        /// <summary>Gets or sets the intended resource play duration.</summary>
        public long Duration { get; set; }

        [MaxLength(80)]
        public string FileName { get; set; }

        public int Id { get; set; }

        /// <summary>Gets or sets a value indicating whether the resource play was interrupted.</summary>
        public bool IsPlayInterrupted { get; set; }

        public bool IsValid => true;

        public DateTime? Modified { get; set; }

        /// <summary>Gets or sets the passenger count when available.</summary>
        public int PassengerCount { get; set; }

        /// <summary>Gets or sets the actual resource play duration.</summary>
        public long PlayedDuration { get; set; }

        /// <summary>Gets or sets when the play started.</summary>
        public DateTime? PlayStarted { get; set; }

        /// <summary>Gets or sets when the play stopped.</summary>
        public DateTime? PlayStopped { get; set; }

        /// <summary>Gets or sets the content resource id.</summary>
        [MaxLength(40)]
        public string ResourceId { get; set; }

        /// <summary>Gets or sets the optional route info.</summary>
        [MaxLength(80)]
        public string Route { get; set; }

        /// <summary>Gets or sets the GPS latitude when the resources began play.</summary>
        [MaxLength(40)]
        public string StartedLatitude { get; set; }

        /// <summary>Gets or sets the GPS longitude when the resources began play.</summary>
        [MaxLength(40)]
        public string StartedLongitude { get; set; }

        /// <summary>Gets or sets the GPS latitude when the resources ended play.</summary>
        [MaxLength(40)]
        public string StoppedLatitude { get; set; }

        /// <summary>Gets or sets the GPS longitude when the resources ended play.</summary>
        [MaxLength(40)]
        public string StoppedLongitude { get; set; }

        /// <summary>Gets or sets the optional TenantId.</summary>
        public int? TenantId { get; set; }

        /// <summary>Gets or sets the optional Trip.</summary>
        [MaxLength(40)]
        public string Trip { get; set; }

        /// <summary>Gets or sets the optional Unit Id FK.</summary>
        public Guid? UnitId { get; set; }

        /// <summary>Gets or sets the required unit name.</summary>
        [MaxLength(40)]
        public string UnitName { get; set; } = Environment.MachineName;

        /// <summary>Gets or sets the vehicle id.</summary>
        [MaxLength(40)]
        public string VehicleId { get; set; }
    }
}