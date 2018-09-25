// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialDestinationDriveScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpecialDestinationDriveScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The special destination drive screen.
    /// </summary>
    internal class SpecialDestinationDriveScreen : DriveScreen<ISpecialDestinationDrive>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialDestinationDriveScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public SpecialDestinationDriveScreen(ISpecialDestinationDrive mainField, IContext context)
            : base(mainField, context)
        {
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            base.Show();

            var destinationText = this.Context.StatusHandler.DriveInfo.DestinationText;
            this.Context.MessageHandler.SetDestinationText(destinationText);
            this.MainField.Init(destinationText);
        }
    }
}