// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationLoggingController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationLoggingController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Log
{
    using System;

    using Gorba.Center.Diag.Core.ViewModels.App;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Logging;

    using NLog;

    /// <summary>
    /// The logging controller that is only responsible for the logs from a single remote application.
    /// </summary>
    public class ApplicationLoggingController : LoggingControllerBase, IDisposable
    {
        private readonly RemoteAppViewModel appViewModel;

        private readonly IRootMessageDispatcher messageDispatcher;

        private ILogObserver logObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLoggingController"/> class.
        /// </summary>
        /// <param name="appViewModel">
        /// The application view model.
        /// </param>
        /// <param name="messageDispatcher">
        /// The message dispatcher.
        /// </param>
        public ApplicationLoggingController(RemoteAppViewModel appViewModel, IRootMessageDispatcher messageDispatcher)
            : base(appViewModel.Logging)
        {
            this.appViewModel = appViewModel;
            this.messageDispatcher = messageDispatcher;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.logObserver == null)
            {
                return;
            }

            this.logObserver.MessageLogged -= this.LogObserverOnMessageLogged;
            this.logObserver = null;
        }

        /// <summary>
        /// Starts the listening to log events.
        /// </summary>
        protected override void Start()
        {
            if (this.logObserver == null)
            {
                this.logObserver =
                    this.messageDispatcher.LogObserverFactory.CreateRemoteObserver(this.appViewModel.Address);
                this.logObserver.MessageLogged += this.LogObserverOnMessageLogged;
            }

            this.logObserver.MinLevel = this.GetMinimumLevel();
        }

        /// <summary>
        /// Stops the listening to log events.
        /// </summary>
        protected override void Stop()
        {
            if (this.logObserver == null)
            {
                return;
            }

            this.logObserver.MinLevel = LogLevel.Off;
        }

        /// <summary>
        /// Changes the minimum level they are listening to.
        /// </summary>
        /// <param name="newLevel">
        /// The new level.
        /// </param>
        protected override void UpdateMinimumLevel(LogLevel newLevel)
        {
            if (this.logObserver == null)
            {
                return;
            }

            this.logObserver.MinLevel = newLevel;
        }

        private void LogObserverOnMessageLogged(object sender, LogEventArgs e)
        {
            this.AddLogMessage(e, this.appViewModel);
        }
    }
}
