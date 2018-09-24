
// --------------------------------------------------------------------------------------------------------------------
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
    /// <summary>
    /// The nmea standart sentence.
    /// </summary>
    public sealed class NMEAStandartSentence : NMEASentence
    {
        /// <summary>
        /// Gets or sets the talker id.
        /// </summary>
        public TalkerIdentifiers TalkerID { get; set; }

        /// <summary>
        /// Gets or sets the sentence id.
        /// </summary>
        public SentenceIdentifiers SentenceID { get; set; }        
    }
}
