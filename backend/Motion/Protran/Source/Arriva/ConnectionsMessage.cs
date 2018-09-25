// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionsMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ArrivaConnection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Motion.Protran.Core.Buffers;

    using NLog;

    /// <summary>
    /// Class containing information about the messages
    /// with connections.
    /// </summary>
    public class ConnectionsMessage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private short numberOfConnections;  // 2 byte
        private short destinationLength;    // 2 byte

        // private short conn1Length;          // 2 byte
        // private byte[] conn1Text;           // ( 2 * conn1Length) byte

        /*
        private short conn2Length;          // 2 byte
        private byte[] conn2Text;           // ( 2 * conn2Length) byte
         *
         *
         *
        private short connNLength;          // 2 byte
        private byte[] connNText;           // ( 2 * connNLength) byte
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionsMessage"/> class.
        /// </summary>
        public ConnectionsMessage()
        {
            this.Connections = new List<ArrivaConnection>();
            this.ClearAllFields();
        }

        /// <summary>
        /// Gets IconId.
        /// </summary>
        public short IconId { get; private set; }

        /// <summary>
        /// Gets Destination Name.
        /// </summary>
        public byte[] DestName { get; private set; }

        /// <summary>
        /// Gets Connections.
        /// </summary>
        public List<ArrivaConnection> Connections { get; private set; }

        /// <summary>
        /// Fills all the properties of this object, parsing the content
        /// of the incoming buffer.
        /// The incoming buffer represents a whole "EvtGorbaConnections".
        /// </summary>
        /// <param name="buffer">The buffer containing the information to fill
        /// all the properties of this object.</param>
        /// <returns>True if the parsing operation has succeeded, else false.</returns>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Legacy code.")]
        public bool Parse(byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                // invalid buffer.
                // I cannot parse nothing.
                Logger.Info("Invalid connections buffer");
                return false;
            }

            if (buffer.Length < 38)
            {
                // invalid buffer's length.
                Logger.Info("Invalid connections buffer length");
                return false;
            }

            // ok, let's start the parse.
            try
            {
                this.IconId = (short)((buffer[32] << 8) | buffer[33]);
                this.numberOfConnections = (short)((buffer[34] << 8) | buffer[35]);
                this.destinationLength = (short)((buffer[36] << 8) | buffer[37]);
                this.DestName = new byte[2 * this.destinationLength];
                Array.Copy(buffer, 38, this.DestName, 0, this.DestName.Length);
                BufferUtils.FromBytesToString(this.DestName);

                int connectionOffset = 38 + this.DestName.Length;
                for (int i = 0; i < this.numberOfConnections; i++)
                {
                    int connInfoLength = (short)((buffer[connectionOffset] << 8) | buffer[connectionOffset + 1]);
                    var connInfo = new byte[2 * connInfoLength];
                    Array.Copy(buffer, connectionOffset + 2, connInfo, 0, connInfo.Length);
                    connectionOffset += 2 + connInfo.Length;

                    string connValue = BufferUtils.FromBytesToString(connInfo);
                    if (string.IsNullOrEmpty(connValue))
                    {
                        // invalid string.
                        // I skip this information
                        continue;
                    }

                    string[] tokens = connValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length <= 0)
                    {
                        // invalid tokens array
                        // I skip this information
                        continue;
                    }

                    var typeString = string.Empty;
                    var time = string.Empty;
                    var destination = string.Empty;
                    var line = string.Empty;
                    var platform = string.Empty;

                    if (tokens.Length >= 1)
                    {
                        typeString = tokens[0];
                    }

                    if (tokens.Length >= 2)
                    {
                        time = tokens[1];
                    }

                    if (tokens.Length >= 3)
                    {
                        destination = tokens[2];
                    }

                    if (tokens.Length >= 4)
                    {
                        line = tokens[3];
                    }

                    if (tokens.Length >= 5)
                    {
                        platform = tokens[4];
                    }

                    int type;
                    if (!int.TryParse(typeString, out type))
                    {
                        continue;
                    }

                    var connection = new ArrivaConnection(type, time, destination, line, platform);
                    this.Connections.Add(connection);
                }
            }
            catch (Exception)
            {
                // an error was occured parsing the buffer.
                // before returning, I clear all.
                this.ClearAllFields();
                return false;
            }

            // ok, it seems everything ok.
            return true;
        }

        private void ClearAllFields()
        {
            this.IconId = 0x0000;               // 2 byte
            this.numberOfConnections = 0x0000;  // 2 byte
            this.destinationLength = 0x0000;    // 2 byte
            // this.conn1Length = 0x0000;          // 2 byte

            // the fields "destinationName" and "conn1Text"
            // have a not fixed size,
            // so, for the moment I set them to null.
            // this.destinationName = null;     // (2 * destinationLength) byte
            // this.conn1Text = null;           // (2 * conn1Length) byte
            this.Connections.Clear();
        }
    }
}
