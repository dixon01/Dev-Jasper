// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiRecorder.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiRecorder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using Gorba.Common.Protocols.Isi;
    using Gorba.Common.Protocols.Isi.Messages;

    /// <summary>
    /// Recorder for ISI messages.
    /// </summary>
    public class IsiRecorder : IIsiSerializerHook, IDisposable
    {
        private const string LineFormat = "{0} {1:yyyy/MM/dd HH:mm:ss.fff} {2}";
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsiRecorder"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The name of the log file to which to write.
        /// </param>
        public IsiRecorder(string fileName)
        {
            this.writer = new StreamWriter(fileName, false, Encoding.UTF8);
        }

        /// <summary>
        /// Message has been serialized.
        /// </summary>
        /// <param name="message">
        /// The message the was serialized.
        /// </param>
        /// <param name="output">
        /// The serialized message.
        /// </param>
        public void MessageSerialized(IsiMessageBase message, string output)
        {
            this.writer.WriteLine(
                string.Format(CultureInfo.InvariantCulture, LineFormat, ">", DateTime.Now, output));
        }

        /// <summary>
        /// Message has been deserialized.
        /// </summary>
        /// <param name="input">
        /// The serialized message input.
        /// </param>
        /// <param name="message">
        /// The deserialized message.
        /// </param>
        public void MessageDeserialized(string input, IsiMessageBase message)
        {
            this.writer.WriteLine(
                string.Format(CultureInfo.InvariantCulture, LineFormat, "<", DateTime.Now, input));

            // log the deserialized message
            ////this.writer.WriteLine("#{0}", message);
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public void Flush()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Closes the underlying stream.
        /// </summary>
        public void Close()
        {
            this.writer.Close();
        }

        /// <summary>
        /// Writes a new comment line to the log file.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        public void WriteComment(string comment)
        {
            this.writer.WriteLine(
                string.Format(CultureInfo.InvariantCulture, LineFormat, "#", DateTime.Now, comment));
        }
    }
}
