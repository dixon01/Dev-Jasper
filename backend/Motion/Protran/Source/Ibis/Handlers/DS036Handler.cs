// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS036Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS036Handler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Handler for the DS036 telegram that resets the given cell after a short while.
    /// </summary>
    public class DS036Handler : TelegramHandler<DS036>
    {
        private readonly ITimer resetTimer;
        private DS036Config config;

        private GenericUsageHandler usage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS036Handler"/> class.
        /// The priority is set to 100.
        /// </summary>
        public DS036Handler()
            : base(100)
        {
            this.resetTimer = TimerFactory.Current.CreateTimer("DS036Reset");
            this.resetTimer.Elapsed += this.ResetTimerOnElapsed;
            this.resetTimer.AutoReset = false;
            this.resetTimer.Interval = TimeSpan.FromMilliseconds(200);
        }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="cfg">
        /// The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public override void Configure(TelegramConfig cfg, IIbisConfigContext configContext)
        {
            base.Configure(cfg, configContext);

            this.config = (DS036Config)cfg;

            this.usage = new GenericUsageHandler(cfg.UsedFor, configContext.Dictionary);
        }

        /// <summary>
        /// Handles the input event and generates Ximple if needed.
        /// This method needs to be implemented by subclasses to
        /// create the Ximple object for the given input event.
        /// </summary>
        /// <param name="telegram">
        /// The input event.
        /// </param>
        protected override void HandleInput(DS036 telegram)
        {
            if (this.config.UsedFor == null)
            {
                return;
            }

            this.resetTimer.Enabled = false;

            var ximple = new Ximple();
            this.usage.AddCell(ximple, telegram.AnnouncementIndex);
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));

            if (!this.config.AutoReset)
            {
                return;
            }

            this.resetTimer.Enabled = true;
        }

        private void ResetTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            var ximple = new Ximple();
            this.usage.AddCell(ximple, string.Empty);
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}
