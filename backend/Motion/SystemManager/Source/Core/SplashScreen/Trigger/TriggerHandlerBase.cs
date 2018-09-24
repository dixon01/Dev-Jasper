// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerHandlerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TriggerHandlerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using System;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;

    /// <summary>
    /// Base class for splash screen trigger handlers.
    /// Trigger handlers listen to a single trigger event and raise the
    /// <see cref="Triggered"/> event when the trigger happens.
    /// </summary>
    public abstract class TriggerHandlerBase
    {
        /// <summary>
        /// Event tat is risen when this trigger was triggered.
        /// </summary>
        public event EventHandler Triggered;

        /// <summary>
        /// Creates a <see cref="TriggerHandlerBase"/> implementation for the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="TriggerHandlerBase"/> implementation.
        /// </returns>
        public static TriggerHandlerBase Create(SplashScreenTriggerConfigBase config)
        {
            var appState = config as ApplicationStateChangeTriggerConfig;
            if (appState != null)
            {
                return new ApplicationStateChangeTriggerHandler(appState);
            }

            var boot = config as SystemBootTriggerConfig;
            if (boot != null)
            {
                return new SystemBootTriggerHandler(boot);
            }

            var shutdown = config as SystemShutdownTriggerConfig;
            if (shutdown != null)
            {
                return new SystemShutdownTriggerHandler(shutdown);
            }

            var input = config as InputTriggerConfig;
            if (input != null)
            {
                return new InputTriggerHandler(input);
            }

            var timeout = config as TimeoutTriggerConfig;
            if (timeout != null)
            {
                return new TimeoutTriggerHandler(timeout);
            }

            var hotKey = config as HotKeyTriggerConfig;
            if (hotKey != null)
            {
                return new HotKeyTriggerHandler(hotKey);
            }

            throw new NotSupportedException("Unsupported trigger: " + config.GetType().Name);
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        /// <param name="owner">
        /// The handler which owns this trigger handler.
        /// </param>
        public abstract void Start(SplashScreenHandler owner);

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Raises the <see cref="Triggered"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseTriggered(EventArgs e)
        {
            var handler = this.Triggered;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}