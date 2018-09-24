// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Watchdog.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Watchdog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32.WD
{
    using System;

    /// <summary>
    /// The JIDA watchdog.
    /// </summary>
    public class Watchdog
    {
        private readonly IntPtr handle;

        private readonly int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="Watchdog"/> class.
        /// </summary>
        /// <param name="handle">
        /// The board handle.
        /// </param>
        /// <param name="index">
        /// The watchdog index (currently only 0 is supported).
        /// </param>
        internal Watchdog(IntPtr handle, int index)
        {
            this.handle = handle;
            this.index = index;
        }

        /// <summary>
        /// Triggers this watchdog.
        /// This method must be called on a continues basis by the application to ensure
        /// that the system will not be restarted.
        /// </summary>
        /// <returns>
        /// A flag indicating if the triggering was successful (true).
        /// </returns>
        public bool Trigger()
        {
            return NativeMethods.JidaWDogTrigger(this.handle, this.index);
        }
    }
}
