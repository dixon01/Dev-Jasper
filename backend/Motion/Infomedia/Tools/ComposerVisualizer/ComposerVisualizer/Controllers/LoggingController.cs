// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoggingController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Controllers
{
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels;

    /// <summary>
    /// The logging controller.
    /// </summary>
    public class LoggingController : IController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingController"/> class.
        /// </summary>
        public LoggingController()
        {
            this.Logging = new LoggingViewModel();
        }

        /// <summary>
        /// Gets or sets the logging viewModel.
        /// </summary>
        public LoggingViewModel Logging { get; set; }

        /// <summary>
        /// Starts the controller
        /// </summary>
        public void Run()
        {
            this.Logging.Messages.Add("New message");
        }

        /// <summary>
        /// Stops the controller
        /// </summary>
        public void Stop()
        {
        }
    }
}
