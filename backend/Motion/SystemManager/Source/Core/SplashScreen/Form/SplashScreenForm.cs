// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.Utility.SplashScreen;

    using NLog;

    using Math = System.Math;
    using ThreadStart = System.Threading.ThreadStart;

    /// <summary>
    /// The splash screen form.
    /// </summary>
    public sealed partial class SplashScreenForm : SplashScreenFormBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SplashScreenForm>();

        private readonly List<SplashScreenPartBase> parts = new List<SplashScreenPartBase>();

        private readonly Rectangle visibleRectangle;

        private readonly Timer changeScreenTimer;

        private readonly Timer updateScreenTimer;

        private SplashScreenManager manager;

        private SplashScreenHandler currentScreen;

        private int partsHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenForm"/> class.
        /// </summary>
        /// <param name="visibleRectangle">
        /// The visible rectangle where the splash screen information should be shown.
        /// </param>
        public SplashScreenForm(Rectangle visibleRectangle)
        {
            this.visibleRectangle = visibleRectangle;

            this.InitializeComponent();

            this.changeScreenTimer = new Timer();
            this.changeScreenTimer.Tick += this.ChangeScreenTimerOnTick;
            this.changeScreenTimer.Interval = 500;

            this.updateScreenTimer = new Timer();
            this.updateScreenTimer.Tick += this.UpdateScreenTimerOnTick;
            this.updateScreenTimer.Interval = 1000;

            this.PrepareForm();
        }

        /// <summary>
        /// Gets or sets the splash screen manager used to drive this splash screen.
        /// </summary>
        public SplashScreenManager Manager
        {
            get
            {
                return this.manager;
            }

            set
            {
                if (this.manager != null)
                {
                    this.manager.CurrentScreenChanged -= this.ManagerOnCurrentScreenChanged;
                }

                this.manager = value;
                if (this.manager != null)
                {
                    this.manager.CurrentScreenChanged += this.ManagerOnCurrentScreenChanged;
                    this.RestartChangeScreenTimer();
                }
            }
        }

        private int Gap
        {
            get
            {
                var height = this.visibleRectangle.Height > 0 ? this.visibleRectangle.Height : this.Height;
                return height / 30;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data. </param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Escape)
            {
                Logger.Debug("Hiding top splash screen because user pressed ESC");
                if (this.manager != null)
                {
                    this.manager.HideTopSplashScreen();
                }
            }
        }

        partial void PrepareForm();

        private void PaintParts(Graphics graphics)
        {
            var bounds = this.visibleRectangle;
            if (bounds.Width <= 0)
            {
                bounds.Width = this.Width;
            }

            if (bounds.Height <= 0)
            {
                bounds.Height = this.Height;
            }

            if (this.partsHeight < 0)
            {
                var size = this.CalculatePartBounds(graphics, bounds);
                this.partsHeight = size.Height;
            }

            var y = ((bounds.Height - this.partsHeight) / 2) + this.Gap + bounds.Y;
            foreach (var part in this.parts)
            {
                part.Paint(graphics, new Rectangle(bounds.X, y, bounds.Width, part.Size.Height));
                y += part.Size.Height + this.Gap;
            }
        }

        private void ChangeSplashScreen()
        {
            Logger.Debug("ChangeSplashScreen() - start.");
            var mgr = this.manager;
            if (mgr == null)
            {
                Logger.Warn("Can't change splash screen because manager is missing");
                return;
            }

            var screen = mgr.CurrentScreen;
            if (screen == null)
            {
                Logger.Debug("Hiding form");
                this.HideForm();
                return;
            }

            if (this.currentScreen != screen)
            {
                Logger.Debug("Updating form to show information from screen {0}", screen.Name);
                this.BackColor = screen.Background;
                this.ForeColor = screen.Foreground;

                foreach (var part in this.parts)
                {
                    part.ContentChanged -= this.PartOnContentChanged;
                    part.Dispose();
                }

                this.parts.Clear();
                Logger.Debug("Screen \'{0}\' has {1} items. BG color: \'{2}\', FG color: \'{3}\'.", screen.Name,
                    screen.Items.Count, screen.Background, screen.Foreground);
                foreach (var item in screen.Items)
                {
                    try
                    {
                        Logger.Debug("\'{0}\' Splash screen item: \'{1}\'", screen.Name, item);
                        var part = SplashScreenPartBase.Create(item, mgr);
                        part.ContentChanged += this.PartOnContentChanged;
                        part.BackColor = screen.Background;
                        part.ForeColor = screen.Foreground;
                        this.parts.Add(part);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Couldn't create part " + item.GetType().Name);
                    }
                }

                this.partsHeight = -1;
                this.currentScreen = screen;
            }

            Logger.Debug("Showing form");
            this.ShowForm();
            this.ForceFocus();
        }

        private Size CalculatePartBounds(Graphics g, Rectangle bounds)
        {
            var totalHeight = int.MaxValue;
            var maxWidth = int.MaxValue;

            double scaling = 1;
            double widthScaling = 1;
            double heightScaling = 1;

            for (var i = 0; i < 5 && (totalHeight > bounds.Height || maxWidth > bounds.Width); i++)
            {
                totalHeight = (this.parts.Count + 1) * this.Gap;
                maxWidth = 0;
                foreach (var part in this.parts)
                {
                    part.Scale(scaling, g);
                    totalHeight += part.Size.Height;
                    maxWidth = Math.Max(maxWidth, part.Size.Width);
                }

                widthScaling *= 0.99 * bounds.Width / maxWidth;
                heightScaling *= 0.99 * bounds.Height / totalHeight;

                scaling = Math.Min(widthScaling, heightScaling);
            }

            return new Size
                       {
                           Height = totalHeight,
                           Width = maxWidth
                       };
        }

        private void ChangeScreenTimerOnTick(object sender, EventArgs e)
        {
            this.changeScreenTimer.Enabled = false;
            try
            {
                this.ChangeSplashScreen();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't change splash screen");
            }
        }

        private void UpdateScreenTimerOnTick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void ManagerOnCurrentScreenChanged(object sender, EventArgs e)
        {
            this.RestartChangeScreenTimer();
        }

        private void RestartChangeScreenTimer()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ThreadStart(this.RestartChangeScreenTimer));
                return;
            }

            this.changeScreenTimer.Enabled = false;
            this.changeScreenTimer.Enabled = true;
        }

        private void PartOnContentChanged(object sender, EventArgs eventArgs)
        {
            if (!this.updateScreenTimer.Enabled)
            {
                // we are not showing the screen, so no reason to invalidate it
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new ThreadStart(this.Invalidate));
                return;
            }

            this.Invalidate();
        }
    }
}
