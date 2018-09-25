// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MdiChildBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MdiChildBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls
{
    using System.Windows.Forms;

    /// <summary>
    /// Base class for all MDI children in Protran Visualizer.
    /// </summary>
    public partial class MdiChildBase : Form
    {
        private const int CpNocloseButton = 0x200;

        /// <summary>
        /// Initializes a new instance of the <see cref="MdiChildBase"/> class.
        /// </summary>
        public MdiChildBase()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the parameter to hide the close button.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CpNocloseButton;
                return myCp;
            }
        }
    }
}
