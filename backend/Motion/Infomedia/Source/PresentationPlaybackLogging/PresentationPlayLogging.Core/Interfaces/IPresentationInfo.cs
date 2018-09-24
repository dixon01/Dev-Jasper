namespace Luminator.PresentationPlayLogging.Core.Interfaces
{
    using System;

    public interface IPresentationInfo
    {
        /// <summary>Gets or sets the created.</summary>
        DateTime Created { get; set; }

        /// <summary>Gets or sets the file name.</summary>
        string FileName { get; set; }

        /// <summary>Test if the minimum fields have been defined and the model contains valid values.</summary>
        bool IsValid { get; }

        /// <summary>Gets or sets the resource id.</summary>
        string ResourceId { get; set; }

        /// <summary>Gets or sets the route.</summary>
        string Route { get; set; }

        /// <summary>Gets or sets the started latitude.</summary>
        string StartedLatitude { get; set; }

        /// <summary>Gets or sets the started longitude.</summary>
        string StartedLongitude { get; set; }

        /// <summary>Gets or sets the stopped latitude.</summary>
        string StoppedLatitude { get; set; }

        /// <summary>Gets or sets the stopped longitude.</summary>
        string StoppedLongitude { get; set; }

        /// <summary>Gets or sets the trip.</summary>
        string Trip { get; set; }

        /// <summary>Gets or sets the unit name.</summary>
        string UnitName { get; set; }

        /// <summary>Gets or sets the vehicle id.</summary>
        string VehicleId { get; set; }
    }
}