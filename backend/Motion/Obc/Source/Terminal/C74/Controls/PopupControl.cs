// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupControl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PopupControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Base class for all controls that serve as a popup inside a <see cref="MainField"/>.
    /// </summary>
    public partial class PopupControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupControl"/> class.
        /// </summary>
        public PopupControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the popup caption.
        /// </summary>
        public string Caption
        {
            get
            {
                return this.labelCaption.Text;
            }

            set
            {
                this.labelCaption.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color of the title bar (where the caption is shown).
        /// </summary>
        public Color TitleColor
        {
            get
            {
                return this.labelCaption.BackColor;
            }

            set
            {
                this.labelCaption.BackColor = value;
            }
        }
    }
}
