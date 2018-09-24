// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WbProCsvFileReader.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WbProCsvFileReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Simulation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Protran.Core.Buffers;

    /// <summary>
    /// IBIS file reader for Wagenbus Monitor's .pro.csv files.
    /// </summary>
    public class WbProCsvFileReader : IbisFileReader
    {
        private readonly CircularBuffer buffer = new CircularBuffer(4096);

        private FileStream input;

        private int lastTimestamp;

        private byte[] nextTelegram;

        /// <summary>
        /// Initializes a new instance of the <see cref="WbProCsvFileReader"/> class.
        /// </summary>
        /// <param name="config">
        /// The simulation config.
        /// </param>
        public WbProCsvFileReader(SimulationConfig config)
            : base(config)
        {
        }

        /// <summary>
        /// Opens this reader and opens the underlying file.
        /// </summary>
        public override void Open()
        {
            this.input = File.OpenRead(this.Config.SimulationFile);
            this.buffer.Clear();
            this.lastTimestamp = -1;
            this.nextTelegram = null;
        }

        /// <summary>
        /// Closes the underlying file.
        /// </summary>
        public override void Close()
        {
            if (this.input == null)
            {
                return;
            }

            this.input.Close();
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
                string[] line;
                while ((line = this.ReadCsvLine()) == null)
                {
                    if (!this.ReadMore())
                    {
                        if (this.nextTelegram != null)
                        {
                            this.CurrentTelegram = this.nextTelegram;
                            this.CurrentWaitTime = TimeSpan.Zero;
                            this.nextTelegram = null;
                            return true;
                        }

                        return false;
                    }
                }

                if (line.Length < 9 || line[0] != "M")
                {
                    continue;
                }

                int timestamp;
                if (!ParserUtil.TryParse(line[2], out timestamp))
                {
                    continue;
                }

                byte[] telegram;
                try
                {
                    telegram = BufferUtils.FromHexStringToByteArray(line[6].Replace(" ", string.Empty));
                }
                catch
                {
                    continue;
                }

                if (telegram == null)
                {
                    continue;
                }

                if (this.nextTelegram == null)
                {
                    this.nextTelegram = telegram;
                    this.lastTimestamp = timestamp;
                    continue;
                }

                this.CurrentTelegram = this.nextTelegram;
                this.CurrentWaitTime = new TimeSpan(0, 0, 0, 0, timestamp - this.lastTimestamp);

                this.nextTelegram = telegram;
                this.lastTimestamp = timestamp;
                return true;
            }
        }

        private bool ReadMore()
        {
            int read = this.input.Read(this.buffer.Buffer, this.buffer.Tail, this.buffer.RemainingLength);
            this.buffer.UpdateTail(read);
            return read > 0;
        }

        private string[] ReadCsvLine()
        {
            int dist = this.buffer.CurrentLength - 1;
            bool insideQuotes = false;
            var cells = new List<string>(10);
            var sb = new StringBuilder();
            for (int i = 0; i < dist; i++)
            {
                var b = this.buffer[i];
                if (b == '"')
                {
                    if (insideQuotes)
                    {
                        if (this.buffer[i + 1] == '"')
                        {
                            // escaped quote character
                            sb.Append('"');
                            i++;
                        }
                        else
                        {
                            insideQuotes = false;
                        }
                    }
                    else
                    {
                        insideQuotes = true;
                    }
                }
                else if (!insideQuotes && b == ';')
                {
                    cells.Add(sb.ToString());
                    sb.Length = 0;
                }
                else if (!insideQuotes && b == '\r' && this.buffer[i + 1] == '\n')
                {
                    this.buffer.UpdateHead(i + 2);
                    cells.Add(sb.ToString());
                    return cells.ToArray();
                }
                else
                {
                    sb.Append((char)b);
                }
            }

            return null;
        }
    }
}
