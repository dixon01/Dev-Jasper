// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialDestinationSelectionListScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpecialDestinationSelectionListScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System.Collections.Generic;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The special destination selection list screen.
    /// </summary>
    internal class SpecialDestinationSelectionListScreen : SimpleListScreen
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SpecialDestinationSelectionListScreen>();

        private readonly List<string> entryList;

        private readonly SpecialDestinationList specDestinationList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialDestinationSelectionListScreen"/> class.
        /// </summary>
        /// <param name="specDestinationList">
        /// The special destination list.
        /// </param>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public SpecialDestinationSelectionListScreen(
            SpecialDestinationList specDestinationList,
            IList mainField,
            IContext context)
            : base(mainField, context)
        {
            this.specDestinationList = specDestinationList;
            this.entryList = this.specDestinationList.GetAllDestinationText();
        }

        /// <summary>
        /// Gets the list that will be shown to the user. The user will/may select an item from this list.
        /// </summary>
        protected override List<string> List
        {
            get
            {
                return this.entryList;
            }
        }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected override string Caption
        {
            get
            {
                return ml.ml_string(85, "Special destination");
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
            Logger.Debug(ml.ml_string(156, "Special destination selection: ") + index);
            if (index < 0)
            {
                this.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(86, "Error"),
                        ml.ml_string(87, "The selected index is not valid"),
                        MessageBoxInfo.MsgType.Error));
                return;
            }

            int blockNbr = this.specDestinationList.GetBlockNumber(index);
            if (blockNbr >= 0)
            {
                int destinationCode = this.specDestinationList.GetDestinationCode(blockNbr);
                new SpecialDestinationLoad(this.Context).LoadSpecialDestDrive(
                    blockNbr,
                    destinationCode,
                    this.specDestinationList.GetText(blockNbr),
                    ml.ml_string(140, "Load Special Destination"));
            }
            else
            {
                this.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(86, "Error"),
                        ml.ml_string(88, "The corresponding destination code was not found (extraservices.csv)"),
                        MessageBoxInfo.MsgType.Error));
            }
        }
    }
}