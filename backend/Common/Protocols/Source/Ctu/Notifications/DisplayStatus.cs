// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayStatus.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Notifications
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Object tasked to represent the notification
    /// CTU datagram regarding the "DisplayStatus",
    /// uniquely identified by the tag number 3.
    /// </summary>
    public class DisplayStatus : Triplet
    {
        private List<Item> statuses;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayStatus"/> class.
        /// </summary>
        /// <param name="statuses">An enumeration over all status objects.</param>
        public DisplayStatus(IEnumerable<Item> statuses)
            : base(TagName.DisplayStatus)
        {
            this.statuses = new List<Item>(statuses);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayStatus"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal DisplayStatus(int length, BinaryReader reader)
            : base(TagName.DisplayStatus)
        {
            this.statuses = new List<Item>(length / 2);
            while (length > 1)
            {
                this.statuses.Add(new Item(reader));
                length -= 2;
            }
        }

        /// <summary>
        /// Gets or sets the list of status items.
        /// </summary>
        public Item[] Items
        {
            get
            {
                return this.statuses.ToArray();
            }

            set
            {
                this.statuses = new List<Item>(value);
            }
        }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return this.statuses.Count * 2;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in this.statuses)
            {
                sb.AppendFormat("{0}: {1}; ", item.Id, item.Status);
            }

            if (sb.Length > 0)
            {
                sb.Length -= 2;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            foreach (var item in this.statuses)
            {
                writer.Write((sbyte)item.Id);
                writer.Write((sbyte)item.Status);
            }
        }

        /// <summary>
        /// The status about a single display.
        /// </summary>
        public class Item
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Item"/> class.
            /// </summary>
            /// <param name="id">
            /// The display id.
            /// </param>
            /// <param name="status">
            /// The display status.
            /// </param>
            public Item(int id, DisplayStatusCode status)
            {
                this.Id = id;
                this.Status = status;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Item"/> class.
            /// </summary>
            /// <param name="reader">
            /// The reader.
            /// </param>
            internal Item(BinaryReader reader)
            {
                this.Id = reader.ReadSByte();
                this.Status = (DisplayStatusCode)reader.ReadSByte();
            }

            /// <summary>
            /// Gets the display id.
            /// </summary>
            public int Id { get; private set; }

            /// <summary>
            /// Gets the status code of the display.
            /// </summary>
            public DisplayStatusCode Status { get; private set; }
        }
    }
}
