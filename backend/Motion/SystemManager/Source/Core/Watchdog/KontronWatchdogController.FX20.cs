// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KontronWatchdogController.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the KontronWatchdogController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Watchdog
{
    using System;

    using Gorba.Common.Utility.Core;

    using Kontron.Jida32;
    using Kontron.Jida32.WD;

    using NLog;

    /// <summary>
    /// Watchdog implementation using the Kontron JIDA API to trigger the hardware watchdog.
    /// </summary>
    public partial class KontronWatchdogController : HardwareWatchdogControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ITimer timer;

        private readonly JidaApi jida;

        private readonly JidaBoard board;

        private readonly Watchdog watchdog;

        /// <summary>
        /// Initializes a new instance of the <see cref="KontronWatchdogController"/> class.
        /// </summary>
        /// <exception cref="JidaException">
        /// If the watchdog could not be opened.
        /// </exception>
        internal KontronWatchdogController()
        {
            this.timer = TimerFactory.Current.CreateTimer(this.GetType().Name);
            this.timer.AutoReset = true;
            this.timer.Interval = TimeSpan.FromSeconds(10);
            this.timer.Elapsed += this.TimerOnElapsed;

            this.jida = new JidaApi();
            if (!this.jida.Initialize())
            {
                throw new JidaException("Couldn't initialize JIDA API");
            }

            this.board = this.jida.OpenBoard(JidaApi.BoardClassCpu, 0);
            if (this.board == null)
            {
                throw new JidaException("Couldn't find CPU board");
            }

            this.watchdog = this.board.Watchdog;
            if (this.watchdog == null)
            {
                throw new JidaException("Couldn't find watchdog");
            }

            Logger.Info("Watchdog ready");
        }

        /// <summary>
        /// Actual implementation of <see cref="HardwareWatchdogControllerBase.Start"/>.
        /// </summary>
        protected override void DoStart()
        {
            this.timer.Enabled = true;
            Logger.Info("Watchdog started");
        }

        /// <summary>
        /// Actual implementation of <see cref="HardwareWatchdogControllerBase.Stop"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.timer.Enabled = false;
            Logger.Info("Watchdog stopped");
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            try
            {
                if (this.watchdog.Trigger())
                {
                    Logger.Trace("Watchdog sucessfully triggered");
                }
                else
                {
                    Logger.Warn("Couldn't trigger watchdog (returned false)");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't trigger watchdog");
            }
        }
    }
}