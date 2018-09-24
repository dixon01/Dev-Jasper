// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Isi.Messages;

    /// <summary>
    /// Serializer that handles <see cref="IsiMessageBase"/> object.
    /// It serializes and deserializes those messages according to
    /// the ISI standard (no XML declaration, no whitespace, UTF-8 encoded)
    /// </summary>
    public class IsiSerializer
    {
        private readonly XmlSerializerNamespaces emptyNamespaces;

        private readonly byte[] readBuffer = new byte[8092];
        private int readBufferPos;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsiSerializer"/> class.
        /// </summary>
        public IsiSerializer()
        {
            this.emptyNamespaces = new XmlSerializerNamespaces();
            this.emptyNamespaces.Add(string.Empty, string.Empty);
        }

        /// <summary>
        /// Gets or sets the stream from which the <see cref="Deserialize"/>
        /// method will read.
        /// </summary>
        public Stream Input { get; set; }

        /// <summary>
        /// Gets or sets the stream to which the <see cref="Serialize"/>
        /// method will write.
        /// </summary>
        public Stream Output { get; set; }

        /// <summary>
        /// Gets or sets the serialization hook that will be notified
        /// when ISI messages are serialized or deserialized.
        /// </summary>
        public IIsiSerializerHook Hook { get; set; }

        /// <summary>
        /// Serializes an ISI message to <see cref="Output"/> and leaves
        /// the stream open.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Serialize(IsiMessageBase message)
        {
            var serializer = new XmlSerializer(message.GetType());
            var output = this.Output;
            HookedStream hooked = null;
            if (this.Hook != null)
            {
                output = hooked = new HookedStream(output);
            }

            using (var writer = IsiXmlWriterFactory.Create(output))
            {
                serializer.Serialize(writer, message, this.emptyNamespaces);
            }

            if (hooked != null)
            {
                var bytes = hooked.ToArray();
                this.Hook.MessageSerialized(message, Encoding.UTF8.GetString(bytes, 0, bytes.Length));
            }
        }

        /// <summary>
        /// Deserializes a single ISI message from <see cref="Input"/> and returns it.
        /// This method might block until a whole message is available.
        /// </summary>
        /// <returns>
        /// the deserialized object.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// if the end of the stream is reached and no message was deserialized.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// if the internal buffer was filled but no message could be read from it.
        /// </exception>
        public IsiMessageBase Deserialize()
        {
            while (true)
            {
                var message = this.ReadFromBuffer();
                if (message != null)
                {
                    return message;
                }

                if (this.readBuffer.Length == this.readBufferPos)
                {
                    throw new InvalidDataException("Could not find a message in the buffer: " + this.readBufferPos);
                }

                var read = this.Input.Read(
                    this.readBuffer, this.readBufferPos, this.readBuffer.Length - this.readBufferPos);
                if (read <= 0)
                {
                    throw new EndOfStreamException();
                }

                this.readBufferPos += read;
            }
        }

        /// <summary>
        /// Read a message from the internal buffer.
        /// </summary>
        /// <returns>
        /// The deserialized message if one is available, otherwise null.
        /// </returns>
        private IsiMessageBase ReadFromBuffer()
        {
            string typeName;
            var stream = this.GetFromBuffer(out typeName);
            if (stream == null)
            {
                return null;
            }

            typeName = typeof(IsiMessageBase).Namespace + "." + typeName;
            var type = Type.GetType(typeName, true, true);
            Debug.Assert(type != null, "Type can't be null");

            var serializer = new XmlSerializer(type);
            IsiMessageBase message = null;
            try
            {
                message = (IsiMessageBase)serializer.Deserialize(stream);
            }
            finally
            {
                if (this.Hook != null)
                {
                    this.Hook.MessageDeserialized(
                        Encoding.UTF8.GetString(this.readBuffer, 0, (int)stream.Length), message);
                }

                // now we can remove the unused bytes
                if (this.readBufferPos > 0)
                {
                    Array.Copy(this.readBuffer, (int)stream.Length, this.readBuffer, 0, this.readBufferPos);
                }
            }

            return message;
        }

        /// <summary>
        /// Tries to get a whole message from the internal buffer.
        /// </summary>
        /// <param name="typeName">the name of the type of the message (if found)</param>
        /// <returns>
        /// a memory stream for the found message or null if not an entire
        /// message is available.
        /// </returns>
        private MemoryStream GetFromBuffer(out string typeName)
        {
            typeName = null;

            int start = this.IndexInReadBuffer('<', 0);
            if (start < 0)
            {
                return null;
            }

            int startTagEnd = this.IndexInReadBuffer('>', start + 1);
            if (startTagEnd < start)
            {
                return null;
            }

            typeName = Encoding.UTF8.GetString(this.readBuffer, start + 1, startTagEnd - start - 1);

            int tag = startTagEnd;
            while ((tag = this.IndexInReadBuffer('<', tag + 1)) > startTagEnd && tag + 1 < this.readBufferPos)
            {
                tag++;
                if (this.readBuffer[tag] != '/')
                {
                    continue;
                }

                int end = this.IndexInReadBuffer('>', tag);
                if (end < tag)
                {
                    return null;
                }

                int tagLength = end - tag;
                if (startTagEnd - start != tagLength)
                {
                    continue;
                }

                tag++;
                var endName = Encoding.UTF8.GetString(this.readBuffer, tag, end - tag);
                if (typeName.Equals(endName))
                {
                    end++;
                    this.readBufferPos -= end;

                    // the readBuffer will only get updated in the calling method
                    // once the data was read from the buffer
                    var stream = new MemoryStream(this.readBuffer, 0, end);
                    stream.Seek(start, SeekOrigin.Begin);
                    return stream;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first index of a given ASCII character in the internal buffer.
        /// </summary>
        /// <param name="c">the ASCII character.</param>
        /// <param name="startIndex">the index at which to start searching</param>
        /// <returns>the found index or -1 if not found.</returns>
        private int IndexInReadBuffer(char c, int startIndex)
        {
            return Array.IndexOf(this.readBuffer, (byte)c, startIndex, this.readBufferPos - startIndex);
        }
    }
}
