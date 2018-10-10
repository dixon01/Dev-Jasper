// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfotransitePresentationInfoCsvClassMap.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core
{
    using CsvHelper.Configuration;

    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PresentationPlayLogging.Core.Models;

    /// <summary>The infotransit presentation info class map for CSV field optional mapping control.</summary>
    public sealed class InfotransitPresentationInfoCsvClassMap : ClassMap<InfotransitPresentationInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="InfotransitPresentationInfoCsvClassMap"/> class.</summary>
        public InfotransitPresentationInfoCsvClassMap()
        {
            this.Map(m => m.Created);
            this.Map(m => m.VehicleId).Default(string.Empty);
            this.Map(m => m.UnitName).Default(string.Empty);
            this.Map(m => m.Route).Default(string.Empty);
            this.Map(m => m.StartedLatitude).Default(string.Empty);
            this.Map(m => m.StartedLongitude).Default(string.Empty);
            this.Map(m => m.StoppedLatitude).Default(string.Empty);
            this.Map(m => m.StoppedLongitude).Default(string.Empty);
            this.Map(m => m.PlayedDuration).Default("0");
            this.Map(m => m.Duration).Default("0");
            this.Map(m => m.PlayStarted);
            this.Map(m => m.PlayStopped);
            this.Map(m => m.IsPlayInterrupted);
            this.Map(m => m.IsValid).Ignore();
        }
    }
}