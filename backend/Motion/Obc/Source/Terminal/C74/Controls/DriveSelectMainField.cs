// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveSelectMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveSelectMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The drive select main field.
    /// </summary>
    public partial class DriveSelectMainField : MainField, IDriveSelect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriveSelectMainField"/> class.
        /// </summary>
        public DriveSelectMainField()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The drive confirmed event.
        /// </summary>
        public event EventHandler<DriveSelectedEventArgs> DriveConfirmed;

        /// <summary>
        /// Initialization of this field.
        /// </summary>
        /// <param name = "caption">
        /// The caption.
        /// </param>
        /// <param name = "items">
        /// The drives to select from.
        /// </param>
        /// <param name = "isDrivingSchoolActive">
        /// if true the driving school button will be selected
        /// </param>
        /// <param name = "isAdditionalActive">
        /// if true the additional drive button will be selected
        /// </param>
        /// <param name="focusIndex">
        /// Index of the focused drive.
        /// </param>
        public void Init(
            string caption,
            List<string> items,
            bool isDrivingSchoolActive,
            bool isAdditionalActive,
            int focusIndex)
        {
            this.Init();

            this.labelCaption.Text = caption;

            if (focusIndex >= items.Count)
            {
                focusIndex = items.Count - 1;
            }

            var buttons = new[] { this.buttonInput1, this.buttonInput2, this.buttonInput3 };
            for (var i = 0; i < buttons.Length; i++)
            {
                if (items.Count > i)
                {
                    buttons[i].Visible = true;
                    buttons[i].Text = items[i];

                    if (focusIndex == i)
                    {
                        buttons[i].IsSelected = true;
                    }
                }
                else
                {
                    buttons[i].Visible = false;
                }
            }

            this.buttonSchool.IsChecked = isDrivingSchoolActive;
            this.buttonAdditional.IsChecked = isAdditionalActive;
        }

        /// <summary>
        /// Raises the <see cref="DriveConfirmed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseDriveConfirmed(DriveSelectedEventArgs e)
        {
            var handler = this.DriveConfirmed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseDriveConfirmed(int index)
        {
            this.RaiseDriveConfirmed(
                new DriveSelectedEventArgs(index, this.buttonSchool.IsChecked, this.buttonAdditional.IsChecked));
        }

        private void ButtonInput1OnPressed(object sender, EventArgs e)
        {
            this.RaiseDriveConfirmed(0);
        }

        private void ButtonInput2OnPressed(object sender, EventArgs e)
        {
            this.RaiseDriveConfirmed(1);
        }

        private void ButtonInput3OnPressed(object sender, EventArgs e)
        {
            this.RaiseDriveConfirmed(2);
        }
    }
}
