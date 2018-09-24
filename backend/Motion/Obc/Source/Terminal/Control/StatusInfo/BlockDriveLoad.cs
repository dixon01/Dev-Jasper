// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockDriveLoad.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlockDriveLoad type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Validator;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The block drive loader.
    /// </summary>
    internal class BlockDriveLoad
    {
        private readonly IContext context;

        private ValidatorHandler validationHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDriveLoad"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public BlockDriveLoad(IContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Loads the given block drive.
        /// </summary>
        /// <param name="blockNumber">
        /// The block number.
        /// </param>
        /// <param name="msgText">
        /// The message text.
        /// </param>
        public void LoadBlockDrive(int blockNumber, string msgText)
        {
            this.validationHandler = new ValidatorHandler(blockNumber, 0, new BusValidator(this.context, blockNumber));
            this.validationHandler.ValidationDone += this.ValidationHandlerValidationDoneEvent;
            this.validationHandler.Start();
            this.context.Screen.ShowProgressBar(new ProgressBarInfo(msgText, this.validationHandler.MaxTimeSec));
        }

        private void ValidationHandlerValidationDoneEvent(object sender, ValidationDoneEventArgs e)
        {
            this.validationHandler.ValidationDone -= this.ValidationHandlerValidationDoneEvent;
            if (this.context != null)
            {
                this.context.Screen.HideProgressBar();
                if (e.ErrorCode == ValidatorHandler.ErrNoError)
                {
                    this.context.Process(InputAlphabet.BlockSet);
                }
                else
                {
                    this.context.StatusHandler.DeleteSavedStatus();
                    this.context.Screen.ShowMessageBox(
                        new MessageBoxInfo(
                            ml.ml_string(7, "Warning"),
                            ml.ml_string(139, "Could not load the saved status. Please login regularly"),
                            MessageBoxInfo.MsgType.Warning));
                }
            }
        }
    }
}