// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Alarm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Alarm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Alarming
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// An alarm that can be stored and sent around (e.g. with Medi).
    /// IMPORTANT:
    /// Do not create an <see cref="Alarm"/> object on your own, but rather use
    /// one of the <c>XxxAlarmFactory</c> classes.
    /// </summary>
    public partial class Alarm
    {
        /// <summary>
        /// Gets or sets the name of the unit that created the error.
        /// </summary>
        /// <seealso cref="Environment.MachineName"/>
        [XmlAttribute]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the alarm category.
        /// </summary>
        /// <remarks>
        /// The alarm category is a rough separation of areas where the alarm comes from.
        /// </remarks>
        [XmlAttribute]
        public AlarmCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [XmlAttribute]
        public AlarmType Type { get; set; }

        /// <summary>
        /// Gets or sets the alarm attribute.
        /// This property should always be cast to one of the defined attribute enumerations.
        /// </summary>
        /// <seealso cref="GetAttribute{T}"/>
        [XmlAttribute]
        public int Attribute { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// The severity of an alarm is predefined by its <see cref="Type"/>
        /// and is set automatically by the alarm factory.
        /// You can change this predefined value at your own risk.
        /// </summary>
        [XmlAttribute]
        public AlarmSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the message string.
        /// </summary>
        /// <remarks>
        /// This can be used to give an explanation why the alarm happened if the
        /// <see cref="Attribute"/> is not clear enough.
        /// Example: if an application restarted, the message should contain the name of the application.
        /// </remarks>
        public string Message { get; set; }

        /// <summary>
        /// Gets the attribute cast to the given enum type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enum; this has to be the right type from the <c>Enums.generated.cs</c> file.
        /// </typeparam>
        /// <returns>
        /// The cast <see cref="Attribute"/> value.
        /// </returns>
        public T GetAttribute<T>()
            where T : struct, IFormattable, IConvertible, IComparable
        {
            return (T)Enum.Parse(typeof(T), this.Attribute.ToString(CultureInfo.InvariantCulture), true);
        }
    }
}
