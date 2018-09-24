// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomSplashScreenPart.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// Splash screen part that shows the current value of a GIOoM port.
    /// </summary>
    public class GioomSplashScreenPart : TextSplashScreenPartBase
    {
        private readonly GioomSplashScreenItem config;

        private readonly PortListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="GioomSplashScreenPart"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public GioomSplashScreenPart(GioomSplashScreenItem config)
        {
            this.config = config;
            this.listener =
                new PortListener(
                    new MediAddress(
                        string.IsNullOrEmpty(config.Unit) ? MessageDispatcher.Instance.LocalAddress.Unit : config.Unit,
                        string.IsNullOrEmpty(config.Application) ? "*" : config.Application),
                    config.Name);
            this.listener.Start(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.listener.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Calculates the string to be shown on the splash screen.
        /// This string can contain multiple lines.
        /// </summary>
        /// <returns>
        /// The string to be shown on the splash screen.
        /// </returns>
        protected override string GetDisplayString()
        {
            var label = string.IsNullOrEmpty(this.config.Label) ? this.config.Name : this.config.Label;
            var value = this.listener.Value;
            return string.Format(
                "{0}: {1}",
                label,
                value == null ? "n/a" : string.Format(this.config.ValueFormat, value.Name));
        }
    }
}