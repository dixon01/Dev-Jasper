// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO002Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Configuration for GO002 telegram to handle
    /// the information about the stops connections.
    /// </summary>
    [Serializable]
    public class GO002Config : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GO002Config"/> class with the default values.
        /// </summary>
        public GO002Config()
        {
            this.CheckLength = false;
            this.StopIndexSize = 2;
            this.RowNumberSize = 1;
            this.PictogramSize = 1;
            this.LineNumberSize = 5;
            this.TrackNumberSize = 2;
            this.ScheduleDeviationSize = 4;
            this.FirstStopIndex = 1;
            this.FirstRowIndex = 1;
            this.LastRowIndex = 9;
            this.PictogramFormat = "{0}";
            this.LineNumberFormat = "{0}";
            this.ScheduleDeviation = new ScheduleDeviation();
            this.DeletePassedStops = false;
            this.ShowForNextStopOnly = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether CheckLength has to be checked.
        /// </summary>
        public bool CheckLength { get; set; }

        /// <summary>
        /// Gets or sets StopIndexSize.
        /// </summary>
        public int StopIndexSize { get; set; }

        /// <summary>
        /// Gets or sets RowNumberSize.
        /// </summary>
        public int RowNumberSize { get; set; }

        /// <summary>
        /// Gets or sets PictogramSize.
        /// </summary>
        public int PictogramSize { get; set; }

        /// <summary>
        /// Gets or sets LineNumberSize.
        /// </summary>
        public int LineNumberSize { get; set; }

        /// <summary>
        /// Gets or sets TrackNumberSize.
        /// </summary>
        public int TrackNumberSize { get; set; }

        /// <summary>
        /// Gets or sets ScheduleDeviationSize.
        /// </summary>
        public int ScheduleDeviationSize { get; set; }

        /// <summary>
        /// Gets or sets FirstStopIndex.
        /// </summary>
        public int FirstStopIndex { get; set; }

        /// <summary>
        /// Gets or sets FirstRowIndex.
        /// </summary>
        public int FirstRowIndex { get; set; }

        /// <summary>
        /// Gets or sets LastRowIndex.
        /// </summary>
        public int LastRowIndex { get; set; }

        /// <summary>
        /// Gets or sets PictogramFormat.
        /// </summary>
        public string PictogramFormat { get; set; }

        /// <summary>
        /// Gets or sets LineNumberFormat.
        /// </summary>
        public string LineNumberFormat { get; set; }

        /// <summary>
        /// Gets or sets ScheduleDeviation.
        /// </summary>
        public ScheduleDeviation ScheduleDeviation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to delete connection information for passed stops.
        /// Default value: false
        /// </summary>
        public bool DeletePassedStops { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ShowForNextStopOnly is to be shown or not.
        /// </summary>
        public bool ShowForNextStopOnly { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedFor { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last GO002 telegram about the field "pictogram".
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForPictogram { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last GO002 telegram about the field "line number".
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last GO002 telegram about the field "departure time".
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last GO002 telegram about the field "track number".
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForTrackNumber { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last GO002 telegram about the field "deviation".
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForScheduleDeviation { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Answer.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override Answer Answer { get; set; }
    }
}
