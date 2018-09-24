
//--------------------------------------------------------------------------------------------------------------------
// <copyright file="NMEAStandartSentence.cs" company="carpintero48, codeporject.com">
//    The Code Project Open License.
// </copyright>
// <summary>
//  Gps signal parser.
//  Downloaded from codeproject.com <see cref="http://www.codeproject.com/Articles/279647/NMEA-sentence-parser-builder">
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NMEA
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The nmea sentence.
    /// </summary>
    public abstract class NMEASentence
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public object[] parameters;
    }
}
