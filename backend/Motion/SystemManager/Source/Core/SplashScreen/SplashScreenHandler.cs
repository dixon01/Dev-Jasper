// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NLog;

namespace Gorba.Motion.SystemManager.Core.SplashScreen
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.SystemManager.SplashScreen;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.SystemManager.Core.SplashScreen.Trigger;
    using NLog;

    /// <summary>
    /// Handler for a single splash screen.
    /// This class checks the triggers and sets the <see cref="isShowing"/> flag accordingly.
    /// </summary>
    public class SplashScreenHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SplashScreenHandler>();
        private const int ShowHysteresis = 200;

        private readonly SplashScreenConfig config;

        private readonly List<TriggerHandlerBase> triggerHandlers = new List<TriggerHandlerBase>();

        /// <summary>
        /// When a splash screen is "showing," it doesn't mean that it is visible to the user.
        /// A "showing" splash screen can be in the background behind other splash screens.
        /// </summary>
        private bool isShowing;

        private long lastHideTicks;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public SplashScreenHandler(SplashScreenConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Event that is fired when the splash screen has been triggered and should be shown.
        /// </summary>
        public event EventHandler Triggered;

        /// <summary>
        /// Event that is fired when the splash screen has been hidden and should not be shown anymore.
        /// </summary>
        public event EventHandler Hidden;

        /// <summary>
        /// Gets the name of this splash screen.
        /// </summary>
        public string Name
        {
            get
            {
                return this.config.Name;
            }
        }

        /// <summary>
        /// Gets the items to be shown on this splash screen.
        /// </summary>
        public List<SplashScreenItemBase> Items
        {
            get
            {
                return this.config.Items;
            }
        }

        /// <summary>
        /// Gets the foreground color.
        /// </summary>
        public Color Foreground
        {
            get
            {
                return this.GetColor(this.config.Foreground);
            }
        }

        /// <summary>
        /// Gets the background color.
        /// </summary>
        public Color Background
        {
            get
            {
                return this.GetColor(this.config.Background);
            }
        }

        /// <summary>
        /// Gets or sets the shutdown catcher.
        /// </summary>
        public ShutdownCatcher ShutdownCatcher { get; set; }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        public void Start()
        {
            foreach (var triggerConfig in this.config.HideOn)
            {
                var handler = TriggerHandlerBase.Create(triggerConfig);
                Logger.Info("SplashScreenHandler.Start() HideOn called for: \'{0}\'.", triggerConfig);
                handler.Triggered += this.HideHandlerOnTriggered;
                this.triggerHandlers.Add(handler);
            }

            foreach (var triggerConfig in this.config.ShowOn)
            {
                var handler = TriggerHandlerBase.Create(triggerConfig);
                Logger.Info("SplashScreenHandler.Start() ShowOn called for: \'{0}\'.", triggerConfig);
                handler.Triggered += this.ShowHandlerOnTriggered;
                this.triggerHandlers.Add(handler);
            }

            foreach (var handler in this.triggerHandlers)
            {
                handler.Start(this);
            }
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public void Stop()
        {
            foreach (var handler in this.triggerHandlers)
            {
                handler.Triggered -= this.ShowHandlerOnTriggered;
                handler.Triggered -= this.HideHandlerOnTriggered;
                handler.Stop();
            }

            this.triggerHandlers.Clear();
        }

        /// <summary>
        /// Raises the <see cref="Triggered"/> event.
        /// </summary>
        protected virtual void RaiseTriggered()
        {
            var handler = this.Triggered;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="Hidden"/> event.
        /// </summary>
        protected virtual void RaiseHidden()
        {
            var handler = this.Hidden;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private Color GetColor(string name)
        {
            Color color;
            return ParserUtil.TryParseColor(name, out color) ? color : Color.Black;
        }

     

        private void ShowHandlerOnTriggered(object sender, EventArgs e)
        {
            var ticks = TimeProvider.Current.TickCount;
            if (this.lastHideTicks + ShowHysteresis >= ticks)
            {
                // only show the splash screen when it was hidden for at least <ShowHysteresis> ms
                return;
            }

            // show can be triggered even if isShowing is already true, this is to bring the screen to the front
            this.isShowing = true;
            this.RaiseTriggered();
        }

        private void HideHandlerOnTriggered(object sender, EventArgs e)
        {
            if (!this.isShowing)
            {
                return;
            }

            this.lastHideTicks = TimeProvider.Current.TickCount;
            this.isShowing = false;
            this.RaiseHidden();
        }
    }
}