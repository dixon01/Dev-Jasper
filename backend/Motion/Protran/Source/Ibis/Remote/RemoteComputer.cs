// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteComputer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Remote
{
    using System;
    using System.Threading;
    using System.Xml.Serialization;

    /// <summary>
    /// Object tasked to represent the remote board computer
    /// attacched to a specific channel.
    /// </summary>
    public class RemoteComputer : IDisposable
    {
        /// <summary>
        /// Tells in which status is currently this remote computer.
        /// </summary>
        private RemoteComputerStatus status;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteComputer"/> class.
        /// </summary>
        public RemoteComputer()
        {
            this.Status = RemoteComputerStatus.Unknown;
        }

        /// <summary>
        /// Handler tasked to fire the "OnStatusChanged" event.
        /// </summary>
        public virtual event EventHandler StatusChanged;

        /// <summary>
        /// Gets or sets a value indicating whether
        /// this remote computer has sent data or not.
        /// </summary>
        public virtual bool HasSentData { get; set; }

        /// <summary>
        /// Gets or sets the current status of this remote computer.
        /// </summary>
        [XmlIgnore]
        public RemoteComputerStatus Status
        {
            get
            {
                return this.status;
            }

            protected set
            {
                if (this.status == value)
                {
                    return;
                }

                this.status = value;
                this.OnStatusChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Releases all the resources allocated by this object.
        /// </summary>
        public virtual void Dispose()
        {
            // nothing to dispose, here in the base class.
            // maybe on the specific class. it depends.
        }

        /// <summary>
        /// Notifies all the registered handlers 
        /// about the current state of this remote computer.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnStatusChanged(EventArgs e)
        {
            var handler = this.StatusChanged;
            if (handler != null)
            {
                // don't do this on our thread since we might have been invoked by the 
                // IBIS reader thread
                ThreadPool.QueueUserWorkItem(o => handler(this, e));
            }
        }
    }
}
