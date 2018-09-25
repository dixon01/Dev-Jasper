// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotKeyTriggerHandler.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HotKeyTriggerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;

    /// <summary>
    /// Trigger handler for a hotkey press.
    /// </summary>
    public partial class HotKeyTriggerHandler : TriggerHandlerBase
    {
        private readonly HotKeyTriggerConfig config;

        private SplashScreenHandler owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyTriggerHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The hot key.
        /// </param>
        public HotKeyTriggerHandler(HotKeyTriggerConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        /// <param name="splashScreenHandler">
        /// The splash Screen Handler.
        /// </param>
        public override void Start(SplashScreenHandler splashScreenHandler)
        {
            this.owner = splashScreenHandler;
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public override void Stop()
        {
        }
    }
}
