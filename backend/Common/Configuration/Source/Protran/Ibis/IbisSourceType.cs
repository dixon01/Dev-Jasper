// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisSourceType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisSourceType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The IBIS source to be used.
    /// </summary>
    public enum IbisSourceType
    {
        /// <summary>
        /// No IBIS source is selected.
        /// </summary>
        None,

        /// <summary>
        /// The IBIS data is taken from a previous recording.
        /// </summary>
        Simulation,

        /// <summary>
        /// The IBIS data is read from a COM port.
        /// </summary>
        SerialPort,

        /// <summary>
        /// The IBIS data is read from a UDP/IP port.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "XML serialization requires caps")]
        UDPServer,

        /// <summary>
        /// The IBIS data is read from JSON interface.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "XML serialization requires caps")]
        JSON
    }
}