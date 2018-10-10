// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;
    using System.Timers;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.SystemManager.Core.Hal;
    using Gorba.Motion.SystemManager.Core.SplashScreen.Form;
    using Gorba.Motion.SystemManager.Core.SplashScreen.Trigger;

    using NLog;

    /// <summary>
    /// Main class managing splash screens.
    /// </summary>
    public partial class SplashScreenManager
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SplashScreenManager>();

        private readonly SplashScreenConfigList config;

        private readonly List<SplashScreenHandler> screens = new List<SplashScreenHandler>();

        private readonly LinkedList<SplashScreenHandler> showingSplashScreens;

        private bool running;

        private SplashScreenHandler currentScreen;

        private Thread formThread;
        private static System.Timers.Timer splashScreenTimeouTimer;

        //public event EventHandler<SplashScreenMessage> ShowSplashScreenEvent = delegate { }; 


        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenManager"/> class.
        /// </summary>
        /// <param name="config">
        ///     The configuration.
        /// </param>
        /// <param name="hardwareAbstraction">
        ///     The HAL to access to hardware information.
        /// </param>
        /// <param name="shutdownCatcher">The shutdown catcher</param>
        public SplashScreenManager(
            SplashScreenConfigList config, HardwareAbstractionBase hardwareAbstraction, ShutdownCatcher shutdownCatcher)
        {
            this.HardwareAbstraction = hardwareAbstraction;
            this.config = config;
            MessageDispatcher.Instance.Subscribe<SplashScreenMessage>(this.ShowSplashScreenHandler);
            foreach (var splashScreen in this.config.Items)
            {
                if (!splashScreen.Enabled)
                {
                    continue;
                }

                var screen = new SplashScreenHandler(splashScreen);
                screen.ShutdownCatcher = shutdownCatcher;
                screen.Triggered += this.ScreenOnTriggered;
                screen.Hidden += this.ScreenOnHidden;
                this.screens.Add(screen);
            }

            this.showingSplashScreens = new LinkedList<SplashScreenHandler>();
        }

        private void ShowSplashScreenHandler(object sender, MessageEventArgs<SplashScreenMessage> e)
        {
            Logger.Info($" We are in {MethodBase.GetCurrentMethod().Name} {e.Message}");
            if (e.Message.Show )
            {
                Logger.Info($"We are showing the splashscreen for message:  {e.Message} : screens count: {this.screens.Count}");
                foreach (var splashScreenHandler in this.screens)
                {
                    Logger.Info(MethodBase.GetCurrentMethod().Name + $" Screen Name : {splashScreenHandler.Name}");
                    if(splashScreenHandler.Name == "HotKey")
                    {
                        Logger.Info(MethodBase.GetCurrentMethod().Name + $" Triggered to Show the Splash Screen");
                        if (splashScreenTimeouTimer == null)
                        {
                            splashScreenTimeouTimer = new System.Timers.Timer(20000);
                            splashScreenTimeouTimer?.Start();
                            Logger.Info(MethodBase.GetCurrentMethod().Name + $" Splash screen timeout is Enabled: {splashScreenTimeouTimer?.Enabled}");
                            if (splashScreenTimeouTimer != null)
                            {
                                splashScreenTimeouTimer.Elapsed += SplashScreenTimeouTimer_Elapsed;
                            }
                        }
                        else
                        {
                            Logger.Info(MethodBase.GetCurrentMethod().Name + $" Reset Timer");
                            splashScreenTimeouTimer?.Stop();
                            splashScreenTimeouTimer?.Start();
                        }
                        this.ScreenOnTriggered(splashScreenHandler, EventArgs.Empty);
                    }
                }
            }
            else
            {
                Logger.Info($"We are hiding the splashscreen {e.Message} for duration {e.Message.Duration}");
               
            }
        }

        private void SplashScreenTimeouTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Info($" SplashScreenTimeouTimer_Elapsed - screens count: {this.screens.Count}");
            foreach (var splashScreenHandler in this.screens)
            {
                if (splashScreenHandler.Name == "HotKey")
                {
                    Logger.Info(MethodBase.GetCurrentMethod().Name + $" Screen Name : {splashScreenHandler.Name}");
                    this.ScreenOnHidden(splashScreenHandler, EventArgs.Empty);
                    if (splashScreenTimeouTimer != null)
                    {
                        splashScreenTimeouTimer.Elapsed -= this.SplashScreenTimeouTimer_Elapsed;
                        splashScreenTimeouTimer?.Stop();
                        splashScreenTimeouTimer.Dispose();
                        splashScreenTimeouTimer = null;
                    }
                }
            }
        }

        /// <summary>
        /// Event that is fired when the <see cref="CurrentScreen"/> changed.
        /// </summary>
        public event EventHandler CurrentScreenChanged;

        /// <summary>
        /// Gets the current screen to be shown or null if no screen should be shown.
        /// </summary>
        public SplashScreenHandler CurrentScreen
        {
            get
            {
                return this.currentScreen;
            }

            private set
            {
                if (this.currentScreen == value)
                {
                    return;
                }

                if (value == null)
                {
                    Logger.Debug("Hiding splash screen");
                }
                else
                {
                    Logger.Debug("Showing splash screen: {0}", value.Name);
                }

                this.currentScreen = value;
                this.RaiseCurrentScreenChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the hardware abstraction layer (HAL) that can be used to query system information.
        /// </summary>
        public HardwareAbstractionBase HardwareAbstraction { get; private set; }

        /// <summary>
        /// Starts this manager.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Start(ApplicationContext context)
        {
            if (this.running)
            {
                return;
            }

            this.StartSplashScreenForm(context);

            this.running = true;
            foreach (var screen in this.screens)
            {
                screen.Start();
            }
        }

        /// <summary>
        /// Stops this manager.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;

            foreach (var screen in this.screens)
            {
                screen.Stop();
            }

            this.showingSplashScreens.Clear();
        }

        /// <summary>
        /// Hides the topmost splash screen.
        /// </summary>
        public void HideTopSplashScreen()
        {
            lock (this.showingSplashScreens)
            {
                this.showingSplashScreens.RemoveLast();
                this.CurrentScreen = this.showingSplashScreens.Count > 0 ? this.showingSplashScreens.Last.Value : null;
            }
        }

        /// <summary>
        /// Raises the <see cref="CurrentScreenChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCurrentScreenChanged(EventArgs e)
        {
            var handler = this.CurrentScreenChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void StartFormThread()
        {
            Logger.Debug("Starting splash screen in a separate thread");
            this.formThread = new Thread(this.RunFormThread);
            this.formThread.IsBackground = true;
            this.formThread.Start();
        }

        private SplashScreenForm CreateSpashScreenForm()
        {
            var form =
                new SplashScreenForm(
                    new Rectangle(this.config.X, this.config.Y, this.config.Width, this.config.Height));
            form.Manager = this;
            return form;
        }

        private void ScreenOnTriggered(object sender, EventArgs e)
        {
            var screen = sender as SplashScreenHandler;
            if (screen == null)
            {
                return;
            }

            Logger.Debug("Splash screen {0} was triggered", screen.Name);

            lock (this.showingSplashScreens)
            {
                this.showingSplashScreens.Remove(screen);
                this.showingSplashScreens.AddLast(screen);
                this.CurrentScreen = screen;
            }
        }

        private void ScreenOnHidden(object sender, EventArgs e)
        {
            var screen = sender as SplashScreenHandler;
            if (screen == null)
            {
                return;
            }

            Logger.Debug("Splash screen {0} was hidden", screen.Name);

            lock (this.showingSplashScreens)
            {
                this.showingSplashScreens.Remove(screen);
                this.CurrentScreen = this.showingSplashScreens.Count > 0 ? this.showingSplashScreens.Last.Value : null;
            }
        }
    }
}
