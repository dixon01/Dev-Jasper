// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// You can inherit from this class if you have driving screen (Block, Special Destination and so on)
    /// This class only overwrites the ESC and Enter Button. When user press one of these buttons,
    /// the menu will appear.
    /// </summary>
    /// <typeparam name="TMainField">
    /// The type of main field.
    /// </typeparam>
    internal abstract class DriveScreen<TMainField> : Screen<TMainField> where TMainField : IMainField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriveScreen{TMainField}"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected DriveScreen(TMainField mainField, IContext context)
            : base(mainField, context)
        {
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            base.Show();
            this.MainField.ReturnPressed += this.MainFieldReturnPressed;
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.MainField.ReturnPressed -= this.MainFieldReturnPressed;
            base.Hide();
        }

        private void MainFieldReturnPressed(object sender, EventArgs e)
        {
            this.Context.ShowMainField(MainFieldKey.Menu);
        }
    }
}