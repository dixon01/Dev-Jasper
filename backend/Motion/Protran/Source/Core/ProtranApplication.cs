// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtranApplication.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Common.SystemManagement.Host;

    /// <summary>
    /// This class handles the management of Protran
    /// </summary>
    public class ProtranApplication : ApplicationBase
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtranApplication"/> class.
        /// </summary>
        public ProtranApplication()
        {
            this.Protran = new Protran();
            this.Protran.ProtocolHost.ProtocolStarted += (s, e) => this.SetRunning();        
        }

        /// <summary>
        /// Gets the protran main class.
        /// </summary>
        public Protran Protran { get; private set; }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// </summary>
        protected override void DoRun()
        {
            this.Protran.Start();
            this.runWait.WaitOne();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="ApplicationBase.DoRun(string[])"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.Protran.Dispose();
            this.runWait.Set();
        }
    }
}
