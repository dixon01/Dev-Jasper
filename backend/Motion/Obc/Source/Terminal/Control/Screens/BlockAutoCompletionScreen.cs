// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockAutoCompletionScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlockAutoCompletionScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System.Collections.Generic;

    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The block auto completion screen.
    /// </summary>
    internal class BlockAutoCompletionScreen : SimpleListScreen
    {
        private string currentText;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockAutoCompletionScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public BlockAutoCompletionScreen(IList mainField, IContext context)
            : base(mainField, context)
        {
        }

        /// <summary>
        /// Gets the list that will be shown to the user. The user will/may select an item from this list.
        /// </summary>
        protected override List<string> List
        {
            get
            {
                List<string> ldb = DriverBlocks.LoadAllByDayKind(RemoteEventHandler.VehicleConfig.DayKind);

                var ret = new List<string>();
                foreach (string str in ldb)
                {
                    if (str.StartsWith(this.currentText))
                    {
                        ret.Add(str);
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected override string Caption
        {
            get
            {
                return "Prise de service";
            }
        }

        /// <summary>
        /// Sets the current text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public void SetCurrentText(string text)
        {
            this.currentText = text;
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
            var screen =
                this.Context.MainFieldHandler.GetScreen<EnterDriverBlockNumberScreen>(MainFieldKey.BlockNumberInput);
            if (screen != null)
            {
                screen.MainField.Block = this.List[index];
            }

            this.Context.ShowMainField(MainFieldKey.BlockNumberInput);
        }
    }
}