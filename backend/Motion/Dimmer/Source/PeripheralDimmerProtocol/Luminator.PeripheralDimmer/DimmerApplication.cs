using System.Threading;

namespace Luminator.PeripheralDimmer
{
    using Gorba.Common.SystemManagement.Host;

    /// <summary>
    /// The dimmer application.
    /// </summary>
    public class DimmerApplication : ApplicationBase
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        #region Default Constructors

        /// <summary>
        /// Default constructor.  Initializes a new instance of the <see cref="DimmerApplication"/> class.
        /// </summary>
        public DimmerApplication()
        {
            Dimmer = new Dimmer();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the dimmer main class.
        /// </summary>
        public Dimmer Dimmer { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// </summary>
        protected override void DoRun()
        {
            this.Dimmer.Start();
            this.runWait.WaitOne();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="DoRun"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.Dimmer.Dispose();
            this.runWait.Set();
        }

        #endregion
    }
}
