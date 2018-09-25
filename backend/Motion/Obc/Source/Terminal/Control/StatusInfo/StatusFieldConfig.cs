// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusFieldConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusFieldConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using System;
    using System.Xml.Serialization;
    using Gorba.Motion.Obc.CommonEmb;

    /// <summary>
    /// The status field config.
    /// </summary>
    public class StatusFieldConfig
    {
        private const int NumberOfFields = 4;

        private readonly StatusFieldType[] fields = new StatusFieldType[NumberOfFields];

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusFieldConfig"/> class.
        /// </summary>
        public StatusFieldConfig()
        {
            for (int i = 0; i < this.fields.Length; i++)
            {
                this.fields[i] = StatusFieldType.None;
            }
        }

        /// <summary>
        /// The status field types.
        /// </summary>
        public enum StatusFieldType
        {
            /// <summary>
            /// No information is shown.
            /// </summary>
            None,

            /// <summary>
            /// The block number.
            /// </summary>
            BlockNumber,

            /// <summary>
            /// The route number.
            /// </summary>
            RouteNumber,

            /// <summary>
            /// The run number.
            /// </summary>
            RunNumber,

            /// <summary>
            /// The route path number.
            /// </summary>
            RoutePathNumber,

            /// <summary>
            /// The zone number.
            /// </summary>
            ZoneNumber,

            /// <summary>
            /// The destination number.
            /// </summary>
            DestinationNumber,

            /// <summary>
            /// The driver number.
            /// </summary>
            DriverNumber,

            /// <summary>
            /// The GPS status.
            /// </summary>
            GpsStatus
        }

        /// <summary>
        /// Gets or sets the list of fields.
        /// </summary>
        public StatusFieldType[] Fields
        {
            get
            {
                return this.fields;
            }

            set
            {
                Array.Clear(this.fields, 0, this.fields.Length);
                Array.Copy(value, this.fields, Math.Min(value.Length, this.fields.Length));
            }
        }

        /// <summary>
        /// Gets or sets the explanation in the config
        /// </summary>
        [XmlAttribute("Description")]
        public string Description
        {
            get
            {
                return "Define a maximum of four fields. Possible values: "
                       + EnumUtil.GetAllEnumValues<StatusFieldType>();
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        /// <summary>
        /// Gets the short name for the given status field type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The short name.
        /// </returns>
        internal string GetShortName(StatusFieldType type)
        {
            switch (type)
            {
                case StatusFieldType.BlockNumber:
                    return ml.ml_string(116, "B");
                case StatusFieldType.RouteNumber:
                    return ml.ml_string(117, "L");
                case StatusFieldType.RunNumber:
                    return ml.ml_string(118, "C");
                case StatusFieldType.RoutePathNumber:
                    return ml.ml_string(119, "R");
                case StatusFieldType.ZoneNumber:
                    return ml.ml_string(120, "Z");
                case StatusFieldType.DestinationNumber:
                    return ml.ml_string(121, "D");
                case StatusFieldType.DriverNumber:
                    return ml.ml_string(122, "Dr");
                case StatusFieldType.GpsStatus:
                    return ml.ml_string(123, "G");
                case StatusFieldType.None:
                    return string.Empty;
                default:
                    return "Unknown type: " + type; // MLHIDE
            }
        }
    }
}