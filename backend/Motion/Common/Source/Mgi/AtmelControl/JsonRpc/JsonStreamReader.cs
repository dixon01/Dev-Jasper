// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonStreamReader.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonStreamReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    using NLog;

    /// <summary>
    /// Reader for multiple JSON objects in a stream.
    /// </summary>
    internal class JsonStreamReader : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Stack<byte> nesting = new Stack<byte>();

        private readonly Stream input;

        private bool inQuotes;

        private bool escaped;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStreamReader"/> class.
        /// </summary>
        /// <param name="input">
        /// The stream to read from.
        /// </param>
        public JsonStreamReader(Stream input)
        {
            this.input = input;
        }

        /// <summary>
        /// Reads the next JSON object from the underlying stream.
        /// </summary>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object ReadObject()
        {
            var text = this.ReadNextJson();
            Logger.Trace("Read: {0}", text);
            return JsonConvert.DeserializeObject(text);
        }

        /// <summary>
        /// Reads the next JSON object from the underlying stream.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to read.
        /// </typeparam>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public T ReadObject<T>()
        {
            var text = this.ReadNextJson();
            Logger.Trace("Read: {0}", text);
            return JsonConvert.DeserializeObject<T>(text);
        }

        /// <summary>
        /// Closes the underlying stream.
        /// </summary>
        public void Close()
        {
            this.input.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Close();
        }

        private string ReadNextJson()
        {
            var writer = new StringWriter();
            var ready = false;
            while (!ready)
            {
                var b = this.input.ReadByte();
                if (b == -1)
                {
                    throw new EndOfStreamException();
                }

                writer.Write((char)b);
                if (b == '\\')
                {
                    if (this.inQuotes)
                    {
                        this.escaped = !this.escaped;
                    }

                    continue;
                }

                if (b == '"')
                {
                    if (!this.escaped)
                    {
                        this.inQuotes = !this.inQuotes;
                    }

                    this.escaped = false;
                    continue;
                }

                this.escaped = false;
                if (this.inQuotes)
                {
                    continue;
                }

                if (b == '{' || b == '[')
                {
                    this.nesting.Push((byte)b);
                }
                else if (b == '}' || b == ']')
                {
                    var nested = this.nesting.Pop();
                    if (nested != b - 2)
                    {
                        throw new FormatException(
                            string.Format(
                                "Invalid nesting, got '{0}' but expected '{1}'", (char)b, (char)(nested + 2)));
                    }

                    ready = this.nesting.Count == 0;
                }
            }

            return writer.ToString();
        }
    }
}