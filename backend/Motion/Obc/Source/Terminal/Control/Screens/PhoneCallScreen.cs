// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneCallScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhoneCallScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.Communication;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The phone call screen.
    /// </summary>
    internal class PhoneCallScreen : SimpleListScreen
    {
        private static readonly Logger Logger = LogHelper.GetLogger<PhoneCallScreen>();

        private readonly PhoneBook phoneBook;

        private string number = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneCallScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public PhoneCallScreen(IList mainField, IContext context)
            : base(mainField, context)
        {
            this.phoneBook = new PhoneBook();
        }

        /// <summary>
        /// Gets the list that will be shown to the user. The user will/may select an item from this list.
        /// </summary>
        protected override List<string> List
        {
            get
            {
                return this.phoneBook.GetAllNames();
            }
        }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected override string Caption
        {
            get
            {
                return ml.ml_string(83, "Phonebook"); // MLHIDE
            }
        }

        /// <summary>
        ///   This method will be called when the user has selected an entry.
        ///   Implement your action here. The index is the selected item from the GetList() method
        /// </summary>
        /// <param name = "index">
        /// The selected index.
        /// </param>
        protected override void ItemSelected(int index)
        {
            if (this.Context.WanManager.IsBuildingCall() || this.Context.WanManager.IsSpeechConnected())
            {
                var message = ml.ml_string(
                    76,
                    "There is already a call in progress.\nEnd current call first before call a new number");
                this.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(77, "Information"),
                        message,
                        MessageBoxInfo.MsgType.Info)); // MLHIDE
                return;
            }

            this.number = this.phoneBook.GetNumber(index);
            if (this.number.Length == 0)
            {
                this.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(7, "Warning"),
                        ml.ml_string(78, "No number found. Not able to made a call"),
                        MessageBoxInfo.MsgType.Warning)); // MLHIDE
            }
            else
            {
                var info = new ProgressBarInfo(ml.ml_string(79, "Calling ") + this.number, 5);
                info.ProgressBarElapsed += (sender, e) => this.Context.ShowRootScreen();
                this.ShowProgressBar(info);
                var t = new Thread(this.StartCall);
                t.Name = "Start call"; // MLHIDE
                t.IsBackground = true;
                t.Start();
            }
        }

        private void StartCall()
        {
            try
            {
                lock (this)
                {
                    if (this.Context.WanManager.Dial(this.number) == false)
                    {
                        this.Context.ShowRootScreen();
                        this.Context.Screen.ShowMessageBox(
                            new MessageBoxInfo(
                                ml.ml_string(81, "Call failed"),
                                ml.ml_string(82, "Could not call the number ") + this.number,
                                MessageBoxInfo.MsgType.Info)); // MLHIDE
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't dial " + this.number, ex);
            }
        }
    }
}