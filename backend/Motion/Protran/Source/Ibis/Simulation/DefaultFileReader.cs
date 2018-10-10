// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultFileReader.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultFileReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Simulation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Motion.Protran.Core.Buffers;

    /// <summary>
    /// File reader for the default Protran format.
    /// </summary>
    public class DefaultFileReader : IbisFileReader
    {
        private string[] nextLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileReader"/> class.
        /// </summary>
        /// <param name="config">
        /// The simulation config.
        /// </param>
        public DefaultFileReader(SimulationConfig config)
            : base(config)
        {
        }

        /// <summary>
        /// Opens this reader and opens the underlying file.
        /// </summary>
        public override void Open()
        {
            base.Open();
            this.nextLine = SplitLine(this.ReadLine());
        }

        /// <summary>
        /// Reads the next telegram from the file.
        /// </summary>
        /// <returns>
        /// true if a new telegram was found.
        /// </returns>
        public override bool ReadNext()
        {
            if (this.nextLine == null)
            {
                return false;
            }

            this.CurrentTelegram = BufferUtils.FromHexStringToByteArray(this.nextLine[2]);

            var line = this.ReadLine();
            if (line == null)
            {
                // no more lines to read, so we don't have to wait for the next telegram
                this.CurrentWaitTime = new TimeSpan(0L);
                this.nextLine = null;
                return true;
            }

            var lineParts = SplitLine(line);
            var currentTime = ExtractTimeStamp(this.nextLine);
            var nextTime = ExtractTimeStamp(lineParts);
            this.CurrentWaitTime = nextTime - currentTime;

            this.nextLine = lineParts;
            return true;
        }

        /// <summary>
        /// Extracts the timestamp (yyyy:MM:dd HH:mm:ss.fff)
        /// from a specific line of the simulation file.
        /// </summary>
        /// <param name="lineParts">The simulation file's line from which
        /// extract the timestamp split into [Date],[Time],[Telegram].</param>
        /// <returns>The timestamp contained into the line.</returns>
        /// <exception cref="ArgumentException">If it's impossible to extract
        /// the timestamp from the string.</exception>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        private static DateTime ExtractTimeStamp(string[] lineParts)
        {
            // the Protran's timestamp format is: yyyy:MM:dd HH:mm:ss.fff
            var date = DateTime.ParseExact(lineParts[0], "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var time = DateTime.ParseExact(lineParts[1], "HH:mm:ss.fff", CultureInfo.InvariantCulture);
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        private static string[] SplitLine(string line)
        {
            string[] splittedLine = line.Split(new[] { ' ' });
            if (splittedLine.Length < 3)
            {
                // invalid line. I don't have the minimum
                // amount of required tokens.
                return null;
            }

            if (splittedLine.Length == 3)
            {
                // for the old versions of IBIS log files produced.
                return splittedLine;
            }

            // the only useful tokens are the first three (for the moment)
            // 1) date
            // 2) time
            // 3) telegram in hexadecimal format.
            var tokens = new string[3];
            Array.Copy(splittedLine, 0, tokens, 0, tokens.Length);
            return tokens;
        }
    }
}
