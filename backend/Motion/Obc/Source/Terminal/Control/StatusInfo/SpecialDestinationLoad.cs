// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialDestinationLoad.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpecialDestinationLoad type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using System;
    using System.Globalization;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The special destination loader.
    /// </summary>
    internal class SpecialDestinationLoad
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SpecialDestinationLoad>();

        private readonly IContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialDestinationLoad"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public SpecialDestinationLoad(IContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Load a special destination drive.
        /// </summary>
        /// <param name="blockNbr">
        /// The block number.
        /// </param>
        /// <param name="destinationCode">
        /// The destination code.
        /// </param>
        /// <param name="specialDestText">
        /// The special destination text.
        /// </param>
        /// <param name="msgText">
        /// The message text.
        /// </param>
        /// <returns>
        /// True if the drive was successfully loaded.
        /// </returns>
        public bool LoadSpecialDestDrive(int blockNbr, int destinationCode, string specialDestText, string msgText)
        {
            try
            {
                var di = this.context.StatusHandler.DriveInfo;
                di.SetSpecialDestText(specialDestText);
                MessageDispatcher.Instance.Broadcast(new ExtraService(blockNbr, destinationCode));
                MessageDispatcher.Instance.Broadcast(
                    new evServiceStarted(blockNbr, true, di.IsDrivingSchool, di.IsAdditionalDrive));

                if (this.context.ConfigHandler.GetConfig().ICenterValidation)
                {
                    MessageDispatcher.Instance.Broadcast(
                        new evDuty(
                            di.DriverNumber.ToString(CultureInfo.InvariantCulture),
                            di.DriverPin.ToString(CultureInfo.InvariantCulture),
                            di.RunNumber.ToString(CultureInfo.InvariantCulture),
                            evDuty.Types.DutyOnSpecialService,
                            di.EvDutyOptions));
                }

                var info = new ProgressBarInfo(msgText, 2);
                info.ProgressBarElapsed += (sender, e) => this.ProgressBarElapsed();
                this.context.Screen.ShowProgressBar(info);
                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Coudln't load special destination drive", ex);
                return false;
            }
        }

        private void ProgressBarElapsed()
        {
            this.context.Screen.HideProgressBar();
            this.context.Process(InputAlphabet.SpecialDestSet);
        }
    }
}