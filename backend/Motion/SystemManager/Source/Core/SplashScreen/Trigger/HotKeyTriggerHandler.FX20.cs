// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotKeyTriggerHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HotKeyTriggerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;
    using Gorba.Common.Utility.Win32.Api.Enums;

    /// <summary>
    /// Trigger handler for a hot key press.
    /// </summary>
    public partial class HotKeyTriggerHandler : TriggerHandlerBase, IMessageFilter
    {
        private const int WmHotkey = 0x0312;

        private readonly char hotKey;

        private SplashScreenHandler owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyTriggerHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The hot key.
        /// </param>
        public HotKeyTriggerHandler(HotKeyTriggerConfig config)
        {
            this.hotKey = Convert.ToChar(config.Key);
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
            this.owner.ShutdownCatcher.RegisterHotKey(KeyModifier.Win, this.hotKey);
            this.owner.ShutdownCatcher.AddMessageFilter(this);
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public override void Stop()
        {
            this.owner.ShutdownCatcher.DeregisterHotKey(KeyModifier.Win, this.hotKey);
            this.owner.ShutdownCatcher.RemoveMessageFilter(this);
        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (m.Msg != WmHotkey)
            {
                return false;
            }

            var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
            var modifier = (KeyModifier)((int)m.LParam & 0xFFFF);
            this.ProcessHotKeyPressed(key, modifier);
            return true;
        }

        private void ProcessHotKeyPressed(Keys key, KeyModifier modifier)
        {
            if (modifier != KeyModifier.Win)
            {
                return;
            }

            if (this.hotKey != (char)key)
            {
                return;
            }

            this.RaiseTriggered(EventArgs.Empty);
        }
    }
}
