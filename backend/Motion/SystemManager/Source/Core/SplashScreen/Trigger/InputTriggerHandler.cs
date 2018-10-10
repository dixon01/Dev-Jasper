// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputTriggerHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputTriggerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using System;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Trigger handler for a GIOoM input change.
    /// </summary>
    public class InputTriggerHandler : TriggerHandlerBase
    {
        private readonly InputTriggerConfig config;

        private readonly PortListener listener;

        private bool gotFirstValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputTriggerHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public InputTriggerHandler(InputTriggerConfig config)
        {
            this.config = config;
            var address = new MediAddress(
                string.IsNullOrEmpty(config.Unit) ? ApplicationHelper.MachineName : config.Unit,
                string.IsNullOrEmpty(config.Application) ? "*" : config.Application);
            this.listener = new PortListener(address, config.Name);
            this.listener.ValueChanged += this.ListenerOnValueChanged;
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        /// <param name="owner">
        /// The handler which owns this trigger handler.
        /// </param>
        public override void Start(SplashScreenHandler owner)
        {
            this.listener.Start(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public override void Stop()
        {
            this.listener.Dispose();
        }

        private void ListenerOnValueChanged(object sender, EventArgs e)
        {
            if (!this.config.Value.HasValue && !this.gotFirstValue)
            {
                // don't trigger if we get the first value and we should trigger on all value changes
                this.gotFirstValue = true;
                return;
            }

            if (!this.config.Value.HasValue || this.config.Value.Value == this.listener.Value.Value)
            {
                this.RaiseTriggered(e);
            }
        }
    }
}