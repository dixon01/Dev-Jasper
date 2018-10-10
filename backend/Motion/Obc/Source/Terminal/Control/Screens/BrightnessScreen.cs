// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrightnessScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BrightnessScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The brightness screen.
    /// </summary>
    internal class BrightnessScreen : SimpleListScreen
    {
        private static readonly Logger Logger = LogHelper.GetLogger<BrightnessScreen>();

        private readonly PortListener brightness;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrightnessScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public BrightnessScreen(IList mainField, DFA.IContext context)
            : base(mainField, context)
        {
            this.brightness = new PortListener(
                new MediAddress(MessageDispatcher.Instance.LocalAddress.Unit, "*"),
                "Brightness");
            this.brightness.Start(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets the list that will be shown to the user. The user will/may select an item from this list.
        /// </summary>
        protected override List<string> List
        {
            get
            {
                return new List<string>
                {
                    ml.ml_string(9, "Bright"),
                    ml.ml_string(10, "Normal"),
                    ml.ml_string(11, "Dark")
                };
            }
        }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected override string Caption
        {
            get
            {
                return ml.ml_string(12, "Screen Brightness");
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
            if (this.brightness.Port == null)
            {
                Logger.Warn("Coulnd't find Brightness port, not changing value");
            }
            else
            {
                this.brightness.Value = this.brightness.Port.CreateValue(index * 7);
            }

            this.Context.ShowRootScreen();
        }
    }
}
