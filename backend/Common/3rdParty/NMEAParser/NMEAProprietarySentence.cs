
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NMEAProprietarySentence.cs" company="carpintero48, codeporject.com">
//    The Code Project Open License.
// </copyright>
// <summary>
//  Gps signal parser.
//  Downloaded from codeproject.com <see cref="http://www.codeproject.com/Articles/279647/NMEA-sentence-parser-builder">
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NMEA
{
    /// <summary>
    /// The nmea proprietary sentence.
    /// </summary>
    public sealed class NMEAProprietarySentence : NMEASentence
    {
        /// <summary>
        /// Gets or sets the sentence id string.
        /// </summary>
        public string SentenceIDString { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        public ManufacturerCodes Manufacturer { get; set; }
    }
}
