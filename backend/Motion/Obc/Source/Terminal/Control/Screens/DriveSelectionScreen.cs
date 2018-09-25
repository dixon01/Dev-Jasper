// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveSelectionScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveSelectionScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The drive selection screen.
    /// </summary>
    internal class DriveSelectionScreen : Screen<IDriveSelect>
    {
        private readonly DriveInfo driveInfo;

        private int focusIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveSelectionScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DriveSelectionScreen(IDriveSelect mainField, IContext context)
            : base(mainField, context)
        {
            this.driveInfo = context.StatusHandler.DriveInfo;
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            this.Init();
            this.MainField.DriveConfirmed += this.DriveSelectDriveConfirmedEvent;
            this.MainField.ReturnPressed += this.MainFieldReturnPressed;
            base.Show();
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.MainField.DriveConfirmed -= this.DriveSelectDriveConfirmedEvent;
            this.MainField.ReturnPressed -= this.MainFieldReturnPressed;
            base.Hide();
        }

        private void Init()
        {
            string caption = ml.ml_string(15, "Selection");
            var items = new List<string>
                            {
                                // "Switch Driver", is not necessary.
                                ml.ml_string(18, "Block"),
                                ml.ml_string(19, "Special Destination"),
                                ml.ml_string(143, "Log Off Driver")
                            };
            this.MainField.Init(
                caption,
                items,
                this.driveInfo.IsDrivingSchool,
                this.driveInfo.IsAdditionalDrive,
                this.focusIndex);
        }

        private void DriveSelectDriveConfirmedEvent(object sender, DriveSelectedEventArgs e)
        {
            this.focusIndex = e.Index;

            if (e.Index != 2)
            {
                this.driveInfo.IsDrivingSchool = e.DrivingSchool;
                this.driveInfo.IsAdditionalDrive = e.Additional;
            }

            switch (e.Index)
            {
                case 0:
                    this.ShowBlockNumberInput();
                    break;
                case 1:
                    this.ShowSpecialDestinationSelect();
                    break;
                case 2:
                    this.Logout();
                    break;
                default:
                    this.ShowMessageBox(
                        new MessageBoxInfo(
                            "Not impl.", // MLHIDE
                            "This functionality is not yet implemented", // MLHIDE
                            MessageBoxInfo.MsgType.Error));
                    break;
            }
        }

        private void ShowBlockNumberInput()
        {
            if (this.Context.StatusHandler.GpsInfo.IsGpsValid && RemoteEventHandler.VehicleConfig.DayKind > 0)
            {
                // TODO PR / JR
                // modif Judicael ... ne me semble pas tres propre
                // a discuter avec lui.
                var screen =
                    this.Context.MainFieldHandler.GetScreen<EnterDriverBlockNumberScreen>(
                        MainFieldKey.BlockNumberInput);
                if (screen != null)
                {
                    screen.MainField.Block = string.Empty;
                }

                // fin modif
                this.Context.ShowMainField(MainFieldKey.BlockNumberInput);
                return;
            }

            string message = ml.ml_string(
                159,
                "The system does not have GPS signal.\nYou can only select a special destination.");
            this.ShowMessageBox(new MessageBoxInfo(ml.ml_string(158, "No GPS"), message, MessageBoxInfo.MsgType.Error));
        }

        private void ShowSpecialDestinationSelect()
        {
            this.Context.ShowMainField(MainFieldKey.SpecialDestinationSelect);
        }

        private void Logout()
        {
            this.driveInfo.ClearAll();
            this.Context.ShowMainField(MainFieldKey.DriverLogin);
        }

        private void MainFieldReturnPressed(object sender, EventArgs e)
        {
            this.Context.ShowMainField(MainFieldKey.Menu);
        }
    }
}