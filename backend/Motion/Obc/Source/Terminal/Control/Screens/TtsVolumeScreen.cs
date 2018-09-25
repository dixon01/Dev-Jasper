// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TtsVolumeScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TtsVolumeScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The TTS volume screen.
    /// </summary>
    internal class TtsVolumeScreen : SimpleListScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TtsVolumeScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public TtsVolumeScreen(IList mainField, IContext context)
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
                // When changing this list don't forget to changed the formula in ItemSelected() method
                var items = new List<string>
                {
                    ml.ml_string(89, "Very loud"),
                    ml.ml_string(90, "Loud"),
                    ml.ml_string(91, "Normal"),
                    ml.ml_string(92, "Low"),
                    ml.ml_string(93, "Lower"),
                    ml.ml_string(94, "Mute")
                };
                return items;
            }
        }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected override string Caption
        {
            get
            {
                return ml.ml_string(95, "Announcement volume");
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
            MessageDispatcher.Instance.Broadcast(new evTTSVolume(100 - (index * 20)));
            this.Context.ShowRootScreen();
        }
    }
}