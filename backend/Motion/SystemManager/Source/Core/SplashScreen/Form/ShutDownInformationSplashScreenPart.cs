// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutDownInformationSplashScreenPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Text;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Update.Usb;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Splash screen part that shows information that system is shutting down
    /// and about USB stick if it is connected.
    /// </summary>
    public partial class ShutDownInformationSplashScreenPart : TextSplashScreenPartBase
    {
        private readonly ShutDownMessageSplashScreenItem config;

        private readonly UsbStickDetector usbStickDetector;

        private long startTickCount;

        private bool hasUsbStick;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShutDownInformationSplashScreenPart"/> class.
        /// </summary>
        /// <param name="config">
        ///     The config.
        /// </param>
        public ShutDownInformationSplashScreenPart(ShutDownMessageSplashScreenItem config)
        {
            this.config = config;

            this.usbStickDetector = new UsbStickDetector();
            this.usbStickDetector.Inserted += (s, e) =>
                {
                    this.hasUsbStick = true;
                    this.RaiseContentChanged(e);
                };
            this.usbStickDetector.Removed += (s, e) =>
                {
                    this.hasUsbStick = false;
                    this.RaiseContentChanged(e);
                };
            this.hasUsbStick = this.CheckUsbStickInserted();
            this.usbStickDetector.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();

            this.usbStickDetector.Stop();
        }

        /// <summary>
        /// The get display string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetDisplayString()
        {
            var ticks = TimeProvider.Current.TickCount;
            if (this.startTickCount == 0)
            {
                this.startTickCount = ticks;
            }

            // 500 is to make sure we round to the next higher value
            var counter = this.config.ShutdownTime.TotalMilliseconds - ticks + this.startTickCount + 500;
            if (counter < 0)
            {
                counter = 0;
            }

            var sb = new StringBuilder();
            sb.Append(this.config.ShutDownMessage);
            sb.Append((int)(counter / 1000));
            this.AppendLine(sb, string.Empty);
            this.AppendLine(sb, string.Empty);
            if (this.hasUsbStick)
            {
                sb.Append("Please remove the USB stick before system shuts down");
            }

            this.AppendLine(sb, string.Empty);
            return sb.ToString();
        }
    }
}
