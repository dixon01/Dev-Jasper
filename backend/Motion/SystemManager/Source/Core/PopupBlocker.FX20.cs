// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupBlocker.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PopupBlocker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The popup blocker.
    /// </summary>
    public partial class PopupBlocker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly PreventPopupsConfig config;

        private readonly SystemManagerController controller;

        private readonly ITimer timer;

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupBlocker"/> class.
        /// </summary>
        /// <param name="config">
        /// The popup blocker config.
        /// </param>
        /// <param name="controller">
        /// The controller which created this object.
        /// </param>
        public PopupBlocker(PreventPopupsConfig config, SystemManagerController controller)
        {
            this.config = config;
            this.controller = controller;

            this.timer = TimerFactory.Current.CreateTimer(this.GetType().Name);
            this.timer.Elapsed += this.TimerOnElapsed;
            this.timer.AutoReset = true;
            this.timer.Interval = this.config.CheckInterval > TimeSpan.Zero
                                      ? this.config.CheckInterval
                                      : TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Starts the popup blocker.
        /// </summary>
        public void Start()
        {
            if (!this.config.Enabled || this.running)
            {
                return;
            }

            this.running = true;

            this.controller.ShutdownCatcher.AddMessageFilter(new MessageFilter());

            if (this.config.Popups.Count > 0)
            {
                this.timer.Enabled = true;
            }
        }

        /// <summary>
        /// Stops the popup blocker.
        /// </summary>
        public void Stop()
        {
            if (!this.config.Enabled || !this.running)
            {
                return;
            }

            this.running = false;

            this.controller.ShutdownCatcher.RemoveMessageFilter(new MessageFilter());
            this.timer.Enabled = false;
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            foreach (var popup in this.config.Popups)
            {
                var className = string.IsNullOrEmpty(popup.ClassName) ? null : popup.ClassName;
                var windowName = string.IsNullOrEmpty(popup.WindowName) ? null : popup.WindowName;

                var handle = NativeMethods.FindWindow(className, windowName);
                if (handle == IntPtr.Zero)
                {
                    continue;
                }

                Logger.Debug("Found window '{0}'/'{1}', closing it", className, windowName);
                NativeMethods.SendMessage(handle, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private static class NativeMethods
        {
            // ReSharper disable InconsistentNaming
            public const uint WM_CLOSE = 0x10;

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern uint RegisterWindowMessage(string lpString);

            // ReSharper restore InconsistentNaming
        }

        private class MessageFilter : IMessageFilter
        {
            private const string QueryCancelAutoPlay = "QueryCancelAutoPlay";

            private uint queryCancelAutoPlayId;

            public bool PreFilterMessage(ref Message m)
            {
                if (this.queryCancelAutoPlayId == 0)
                {
                    this.queryCancelAutoPlayId = NativeMethods.RegisterWindowMessage(QueryCancelAutoPlay);
                }

                if (m.Msg == this.queryCancelAutoPlayId)
                {
                    // prevent auto-play dialog from popping up
                    m.Result = (IntPtr)1;
                    Logger.Debug("Prevented auto-play popup");
                    return true;
                }

                return false;
            }
        }
    }
}