// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PauseItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PauseItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Playback item that pauses for a given amount of time.
    /// </summary>
    internal class PauseItem : AudioItemBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ITimer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PauseItem"/> class.
        /// </summary>
        /// <param name="duration">
        /// The duration.
        /// </param>
        public PauseItem(TimeSpan duration)
        {
            this.timer = TimerFactory.Current.CreateTimer("Pause");
            this.timer.AutoReset = false;
            this.timer.Interval = duration;
            this.timer.Elapsed += this.TimerOnElapsed;
        }

        /// <summary>
        /// Start playing this item.
        /// </summary>
        public override void Start()
        {
            Logger.Info("Waiting {0}", this.timer.Interval);
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Immediately stop playing this item.
        /// </summary>
        public override void Stop()
        {
            this.timer.Enabled = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.timer.Dispose();
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.RaiseCompleted(eventArgs);
        }
    }
}