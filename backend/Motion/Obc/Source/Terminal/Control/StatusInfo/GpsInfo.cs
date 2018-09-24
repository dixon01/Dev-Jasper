// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GpsInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using System;

    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The GPS info.
    /// </summary>
    internal class GpsInfo : IGpsInfo
    {
        private readonly PortListener gpsCoverage;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsInfo"/> class.
        /// </summary>
        /// <param name="coordinateX">
        /// The coordinate x.
        /// </param>
        /// <param name="coordinateY">
        /// The coordinate y.
        /// </param>
        /// <param name="direction">
        /// The direction.
        /// </param>
        public GpsInfo(string coordinateX, string coordinateY, string direction)
        {
            // TODO: coordinates are not used at all, why do we have them here?
            this.SXCoordinate = string.Empty;
            this.SYCoordinate = string.Empty;
            this.SXCoordinate = coordinateX;
            this.SYCoordinate = coordinateY;
            this.SDirection = direction;

            this.gpsCoverage = new PortListener(MediAddress.Broadcast, "GpsCoverage");
            this.gpsCoverage.ValueChanged += this.GpsCoverageOnValueChanged;
            this.gpsCoverage.Start(TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Gets a value indicating whether GPS is valid.
        /// </summary>
        public bool IsGpsValid { get; private set; }

        /// <summary>
        /// Gets the GPS validity string (used for display).
        /// </summary>
        public string SIsGpsValid
        {
            get
            {
                if (this.IsGpsValid)
                {
                    return ml.ml_string(132, "Yes"); // MLHIDE
                }

                return ml.ml_string(133, "No"); // MLHIDE
            }
        }

        /// <summary>
        /// Gets the X coordinate string (used for display).
        /// </summary>
        public string SXCoordinate { get; private set; }

        /// <summary>
        /// Gets the Y coordinate string (used for display).
        /// </summary>
        public string SYCoordinate { get; private set; }

        /// <summary>
        /// Gets the direction string (used for display).
        /// </summary>
        public string SDirection { get; private set; }

        private void GpsCoverageOnValueChanged(object sender, EventArgs eventArgs)
        {
            this.IsGpsValid = FlagValues.True.Equals(this.gpsCoverage.Value);
        }
    }
}