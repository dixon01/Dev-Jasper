// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GismoRecorder.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GismoRecorder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Recording
{
    using System.Text;

    /// <summary>
    /// This class gives the format based on the
    /// Gismo standard.
    /// </summary>
    public class GismoRecorder : IbisRecorder
    {
        /// <summary>
        /// The object tasked to concatenate the pieces of string
        /// for a well formatted string.
        /// </summary>
        private readonly StringBuilder formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GismoRecorder"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public GismoRecorder(IIbisConfigContext configContext)
            : base(configContext)
        {
            this.formatter = new StringBuilder();
        }

        /// <summary>
        /// Converts the incoming text to a text formatted
        /// as specified by the Gismo's format.
        /// </summary>
        /// <param name="text">The text to which apply the Gismo's format.</param>
        /// <returns>The string with the Gismo's format.</returns>
        protected override string FormatText(string text)
        {
            // inside the file, I've to write some text.
            // but how should be formatted the text ?
            // this recorder allows two types of format:
            // 1) [Timestamp] text  (where Timestamp ==> yyyy:MM:dd HH:mm:ss.fff)
            // 2) Gismo format
            this.formatter.Remove(0, this.formatter.Length);

            // it's valid the first formatting style.
            this.formatter.Append("Gismo format not yet implemented.");
            return this.formatter.ToString();
        }
    }
}
