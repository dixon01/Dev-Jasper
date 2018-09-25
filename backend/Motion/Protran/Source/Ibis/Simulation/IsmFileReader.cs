// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsmFileReader.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsmFileReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Simulation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// File reader for old .ism files created and used by Infomedia.
    /// </summary>
    public class IsmFileReader : IbisFileReader
    {
        private static readonly Regex EscapeRegex = new Regex("<(CR|LF|PY|[0-9A-F]{2})>");

        private readonly List<string> lines = new List<string>();

        private int currentLineIndex;
        private TimeSpan defaultWaitTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsmFileReader"/> class.
        /// </summary>
        /// <param name="config">
        /// The simulation config.
        /// </param>
        public IsmFileReader(SimulationConfig config)
            : base(config)
        {
            this.defaultWaitTime = config.IntervalBetweenTelegrams == null
                                   || config.IntervalBetweenTelegrams < TimeSpan.Zero
                                       ? TimeSpan.FromSeconds(1)
                                       : config.IntervalBetweenTelegrams.Value;
        }

        /// <summary>
        /// Opens this reader and reads the underlying file.
        /// </summary>
        public override void Open()
        {
            base.Open();
            this.lines.Clear();
            this.currentLineIndex = -1;

            string line;
            while ((line = this.ReadLine()) != null)
            {
                this.lines.Add(line);
            }
        }

        /// <summary>
        /// Reads the next telegram from the file.
        /// </summary>
        /// <returns>
        /// true if a new telegram was found.
        /// </returns>
        public override bool ReadNext()
        {
            while (true)
            {
                this.currentLineIndex++;

                if (this.currentLineIndex >= this.lines.Count)
                {
                    return false;
                }

                var line = this.lines[this.currentLineIndex];
                if (line.Length == 0)
                {
                    // try the next line
                    continue;
                }

                switch (line[0])
                {
                    case '*':
                        int waitTime;
                        if (ParserUtil.TryParse(line.Substring(1), out waitTime) && waitTime > 0)
                        {
                            this.defaultWaitTime = TimeSpan.FromMilliseconds(waitTime);
                        }

                        this.CurrentTelegram = null;
                        this.CurrentWaitTime = this.defaultWaitTime;
                        return true;
                    case '~':
                        var waitDuration = this.defaultWaitTime;
                        int wait;
                        if (ParserUtil.TryParse(line.Substring(1), out wait))
                        {
                            waitDuration = TimeSpan.FromMilliseconds(wait);
                        }

                        this.CurrentTelegram = null;
                        this.CurrentWaitTime = waitDuration;
                        return true;
                    case '.':
                        int index;
                        ParserUtil.TryParse(line.Substring(1), out index);
                        this.currentLineIndex = index - 1;
                        continue;
                    default:
                        this.CurrentTelegram = ParseTelegram(line);
                        this.CurrentWaitTime = this.defaultWaitTime;
                        return true;
                }
            }
        }

        private static byte[] ParseTelegram(string line)
        {
            var unescaped = EscapeRegex.Replace(line, ReplaceEscapes);
            return Encoding.ASCII.GetBytes(unescaped + "\r0"); // append <CR> and fake checksum
        }

        private static string ReplaceEscapes(Match match)
        {
            var value = match.Groups[1].Value;
            if (value == "CR" || value == "PY")
            {
                return string.Empty;
            }

            if (value == "LF")
            {
                return "\n";
            }

            var c = (char)int.Parse(value, NumberStyles.HexNumber);
            return c.ToString(CultureInfo.InvariantCulture);
        }
    }
}
