// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IconBar.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IconBar type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    /// <summary>
    /// The icon bar implementation that actually just uses the GUI project's icon bar.
    /// TODO: implement this class properly without dependency on the GUI project.
    /// </summary>
    public partial class IconBar : Gui.MainFields.IconBar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IconBar"/> class.
        /// </summary>
        public IconBar()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Simulates a press on the menu button.
        /// </summary>
        public void PressMenuButton()
        {
            this.RaiseContextIconClick();
        }
    }
}
