// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcVisualizer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// The main form of the AHDLC visualizer.
    /// </summary>
    public partial class MainForm : Form
    {
        private const int DotSize = 5;

        private readonly Brush amberBrush = new SolidBrush(Color.DarkOrange);
        private readonly Brush blackBrush = new SolidBrush(Color.Black);

        private readonly List<OutputCommandFrame> outputCommands = new List<OutputCommandFrame>();

        private bool collectOutput;

        private int dataBlockCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            var icon = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "ahdlcrenderer.ico");
            if (icon != null)
            {
                this.Icon = new Icon(icon);
            }
        }

        /// <summary>
        /// Handles a frame that was created or received.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        public void HandleFrame(FrameBase frame)
        {
            var setupCommand = frame as SetupCommandFrame;
            if (setupCommand != null)
            {
                this.HandleSetupCommand(setupCommand);
                return;
            }

            var outputCommand = frame as OutputCommandFrame;
            if (outputCommand != null)
            {
                this.HandleOutputCommand(outputCommand);
            }
        }

        private void HandleSetupCommand(SetupCommandFrame setupCommand)
        {
            this.outputCommands.Clear();
            this.dataBlockCount = setupCommand.DataBlockCount;
            this.collectOutput = setupCommand.Mode == DisplayMode.BlockScrollBitmap
                || setupCommand.Mode == DisplayMode.BlockScrollLargeBitmap
                || setupCommand.Mode == DisplayMode.BlockScrollSpeedBitmap
                || setupCommand.Mode == DisplayMode.ScrollingBitmap
                || setupCommand.Mode == DisplayMode.StaticBitmap;
        }

        private void HandleOutputCommand(OutputCommandFrame outputCommand)
        {
            if (!this.collectOutput)
            {
                return;
            }

            this.outputCommands.Add(outputCommand);
            if (this.outputCommands.Count < this.dataBlockCount)
            {
                return;
            }

            this.collectOutput = false;
            if (this.outputCommands.Count == 0)
            {
                return;
            }

            var bitmap = new Bitmap(
                (this.outputCommands.Count * 8 * DotSize) + 1,
                (outputCommand.Data.Length * DotSize) + 1,
                PixelFormat.Format32bppRgb);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Gray);
                var x = 0;
                foreach (var output in this.outputCommands)
                {
                    var y = 0;
                    foreach (var bits in output.Data)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            var isSet = (bits & (1 << (7 - i))) != 0;
                            var brush = isSet ? this.amberBrush : this.blackBrush;
                            g.FillEllipse(brush, (x + i) * DotSize, y * DotSize, DotSize, DotSize);
                        }

                        y++;
                    }

                    x += 8;
                }
            }

            this.SetBitmap(outputCommand.Address, bitmap);
        }

        private void SetBitmap(int address, Bitmap bitmap)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler((s, e) => this.SetBitmap(address, bitmap)));
                return;
            }

            var addressString = address.ToString(CultureInfo.InvariantCulture);
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Name == addressString)
                {
                    var pictureBox = tabPage.Controls[0] as PictureBox;
                    if (pictureBox != null)
                    {
                        pictureBox.Image = bitmap;
                    }

                    return;
                }
            }

            var page = new TabPage();
            page.Text = addressString;
            page.Name = addressString;
            page.Controls.Add(
                new PictureBox { Dock = DockStyle.Fill, Image = bitmap, SizeMode = PictureBoxSizeMode.CenterImage });
            this.tabControl1.TabPages.Add(page);
        }
    }
}
