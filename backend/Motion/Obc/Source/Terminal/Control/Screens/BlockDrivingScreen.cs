// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockDrivingScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlockDrivingScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The block driving screen.
    /// </summary>
    internal class BlockDrivingScreen : DriveScreen<IBlockDrive>
    {
        private static readonly Logger Logger = LogHelper.GetLogger<BlockDrivingScreen>();

        private readonly ITimer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDrivingScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public BlockDrivingScreen(IBlockDrive mainField, IContext context)
            : base(mainField, context)
        {
            this.timer = TimerFactory.Current.CreateTimer("BlockDriving");
            this.timer.AutoReset = true;
            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Elapsed += this.TimerOnElapsed;
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            base.Show();
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.timer.Enabled = false;
            base.Hide();
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            try
            {
                var di = this.Context.StatusHandler.DriveInfo;
                this.Context.MessageHandler.SetDestinationText(di.DestinationText);
                this.MainField.SetStations(
                    di.BlockDrive.GetNextStop(0),
                    di.BlockDrive.GetNextStop(1),
                    di.BlockDrive.GetNextStop(2));
                this.MainField.SetDelaySec(di.BlockDrive.Delay, di.IsAtBusStop);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't update block driving screen", ex);
            }
        }
    }
}