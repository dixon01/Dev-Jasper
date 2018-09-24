// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XimpleTester
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        private MainMenu mainMenu;

        private int creatorCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var size = Screen.FromControl(this).WorkingArea;
            size.Inflate(size.Width / -20, size.Height / -20);
            this.Bounds = size;
        }

        private void MenuItemExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MenuItemAddCreatorClick(object sender, EventArgs e)
        {
            var creator = new XimpleCreatorForm();
            creator.Text += string.Format(" [{0}]", ++this.creatorCounter);
            creator.MdiParent = this;
            creator.Show();
        }

        private void MainFormMdiChildActivate(object sender, EventArgs e)
        {
            this.menuItemCloseWindow.Enabled = this.ActiveMdiChild != null;
        }

        private void MenuItemCloseWindowClick(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                this.ActiveMdiChild.Close();
            }
        }

        private void MenuItemCascadeClick(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void MenuItemTileHorizontallyClick(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void MenuItemTileVerticallyClick(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void MenuItemArrangeIconsClick(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void MenuItemDistributeClick(object sender, EventArgs e)
        {
            var x = 0;
            var y = 0;
            var maxHeight = 0;
            foreach (var child in this.MdiChildren)
            {
                if (x > 0 && child.Width + x > this.ClientSize.Width)
                {
                    x = 0;
                    y += maxHeight;
                }

                child.Location = new Point(x, y);
                x += child.Width;
                maxHeight = Math.Max(maxHeight, child.Height);
            }
        }
    }
}
