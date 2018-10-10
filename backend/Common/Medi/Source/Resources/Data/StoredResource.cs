// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredResource.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StoredResource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// This class is only for internal use, it's only public to support XML serialization.
    /// Data object containing information about a stored resource.
    /// </summary>
    public class StoredResource : IStoredResource
    {
        private ResourceState state;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredResource"/> class.
        /// </summary>
        public StoredResource()
        {
            this.References = new List<string>();
            this.Transfers = new List<ResourceTransfer>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredResource"/> class.
        /// </summary>
        /// <param name="id">
        /// The unique resource id.
        /// </param>
        public StoredResource(ResourceId id)
            : this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Event that is fired whenever the <see cref="State"/> changes.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Gets or sets the unique resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets the state of this resource.
        /// </summary>
        public ResourceState State
        {
            get
            {
                return this.state;
            }

            set
            {
                if (this.state == value)
                {
                    return;
                }

                this.state = value;
                this.RaiseStateChanged();
            }
        }

        /// <summary>
        /// Gets or sets the original source address where the
        /// resource comes from.
        /// </summary>
        public MediAddress Source { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this resource is only temporary
        /// and will be deleted once sent to the requested destination.
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Gets or sets the size of the resource.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the list of currently ongoing resource transfers.
        /// </summary>
        public List<ResourceTransfer> Transfers { get; set; }

        /// <summary>
        /// Gets or sets the list of references.
        /// </summary>
        public List<string> References { get; set; }

        /// <summary>
        /// Gets or sets the store reference.
        /// This can be null if the resource is not yet available
        /// or the resource is "checked out".
        /// </summary>
        public string StoreReference { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets the set index.
        /// </summary>
        public int SetIndex { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append('{');
            builder.Append(this.Id);
            builder.Append(": State=").Append(this.State);
            builder.Append(";Size=").Append(this.Size);
            builder.Append(";Source=").Append(this.Source);
            builder.Append(";Temp=").Append(this.IsTemporary);
            builder.Append(";Store=").Append(this.StoreReference);
            builder.Append(";SetIndex=").Append(this.SetIndex);
            builder.Append('}');
            return builder.ToString();
        }

        /// <summary>
        /// Raises the <see cref="StateChanged"/> event.
        /// </summary>
        protected virtual void RaiseStateChanged()
        {
            var handler = this.StateChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}