// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockDriveWaitScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlockDriveWaitScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The block drive wait screen.
    /// </summary>
    internal class BlockDriveWaitScreen : DriveScreen<IBlockDriveWait>
    {
        private static readonly Logger Logger = LogHelper.GetLogger<BlockDriveWaitScreen>();

        private readonly ITimer timer;

        private bool isDriverInformed;

        private bool isVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDriveWaitScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public BlockDriveWaitScreen(IBlockDriveWait mainField, IContext context)
            : base(mainField, context)
        {
            this.timer = TimerFactory.Current.CreateTimer("BlockDriveWait");
            this.timer.AutoReset = true;
            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Elapsed += this.TimerOnElapsed;
            ////DateTime dt = DateTime.Now;
            ////dt = dt.AddSeconds(30);
            ////this.Init(ml.ml_string(5, "Start"), ml.ml_string(6, "End"), dt);
        }

        /* public void Init(string destinationText, string startText, DateTime driveTimeStart)
    {
        this.destinationText = destinationText;
        this.startText = startText;
        this.driveTimeStart = driveTimeStart;
    }*/

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            this.isVisible = true;
            this.isDriverInformed = false;
            MessageDispatcher.Instance.Subscribe<evTripLoaded>(this.EvTripLoadedEvent);
            base.Show();
            this.MainField.Init(string.Empty, string.Empty, TimeProvider.Current.Now);
            this.StartTimer();
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.isVisible = false;
            this.StopTimer();
            base.Hide();
            MessageDispatcher.Instance.Unsubscribe<evTripLoaded>(this.EvTripLoadedEvent);
        }

        private void StartTimer()
        {
            this.StopTimer();
            this.timer.Enabled = true;
        }

        private void StopTimer()
        {
            this.timer.Enabled = false;
        }

        private void EvTripLoadedEvent(object sender, MessageEventArgs<evTripLoaded> e)
        {
            this.StartTimer();
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            if (Monitor.TryEnter(this) == false)
            {
                return;
            }

            try
            {
                BlockDrive bd = this.Context.StatusHandler.DriveInfo.BlockDrive;
                string errorDescription; // bd.GetErrorDescription();
                int counter = 0;
                while ((errorDescription = bd.ErrorDescription) != null && (counter++ < 20))
                {
                    if (this.isVisible == false)
                    {
                        return;
                    }

                    // TODO: really???
                    Thread.Sleep(500);
                }

                if (errorDescription == null)
                {
                    if (this.isDriverInformed)
                    {
                        this.HideMessageBox();
                        this.isDriverInformed = false;
                    }

                    this.Context.MessageHandler.SetDestinationText(
                        this.Context.StatusHandler.DriveInfo.DestinationText);
                    ////this.Init(context.GetStatusHandler().GetDriveInfo().GetDestinationText(),
                    ////  bd.GetStartName(), bd.GetDepartureTime());
                    this.MainField.Init(
                        this.Context.StatusHandler.DriveInfo.DestinationText,
                        bd.StartName,
                        bd.DepartureTime);
                }
                else
                {
                    if (this.isVisible && !this.isDriverInformed)
                    {
                        // this is a liddle hack. Because in this timer method in first run and this case here
                        // will appear a messagebox. but, in this time the user may pressed menu button.
                        // So already a new screen is appeared. if this should be the case, do not show any messagebox.
                        this.isDriverInformed = true;

                        // pas de message "course pas encore chargée en mode ERG
                        if (!this.Context.ConfigHandler.GetConfig().ThirdPartyPS)
                        {
                            this.ShowMessageBox(
                                new MessageBoxInfo(
                                    ml.ml_string(7, "Warning"),
                                    ml.ml_string(8, "Block number validated. Drive not yet loaded."),
                                    MessageBoxInfo.MsgType.Warning));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't update block wait screen", ex);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}