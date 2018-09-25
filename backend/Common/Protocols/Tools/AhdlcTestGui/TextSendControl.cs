// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextSendControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextSendControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Providers;

    /// <summary>
    /// The text send control.
    /// </summary>
    public partial class TextSendControl : Gorba.Common.Protocols.Tools.AhdlcTestGui.DataSendControlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextSendControl"/> class.
        /// </summary>
        public TextSendControl()
        {
            this.InitializeComponent();
            this.DisplayMode = DisplayMode.AutoText;
        }

        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        public DisplayMode DisplayMode { get; set; }

        /// <summary>
        /// This method can be overridden by subclasses to create all necessary frames to send the data to the control.
        /// </summary>
        /// <returns>
        /// The frames to be sent to the sign; can't be null but might be empty.
        /// The frames' <see cref="FrameBase.Address"/> doesn't need to be correct,
        /// it will be updated by the calling method.
        /// </returns>
        protected override IEnumerable<LongFrameBase> CreateFrames()
        {
            var texts = new List<string>();
            if (this.textBox1.TextLength != 0)
            {
                texts.Add(this.textBox1.Text);
            }

            if (this.textBox2.TextLength != 0)
            {
                texts.Add(this.textBox2.Text);
            }

            if (this.textBox3.TextLength != 0)
            {
                texts.Add(this.textBox3.Text);
            }

            if (texts.Count == 0)
            {
                yield break;
            }

            IFrameProvider provider;
            switch (this.DisplayMode)
            {
                case DisplayMode.AutoText:
                    provider = new AutoTextFrameProvider(
                        (int)this.numericUpDownBlockGap.Value,
                        (int)this.numericUpDownPadding.Value,
                        texts.ToArray());
                    break;
                case DisplayMode.ScrollText:
                    provider = new ScrollTextFrameProvider(
                        (int)this.numericUpDownBlockGap.Value,
                        (int)this.numericUpDownPadding.Value,
                        texts.ToArray());
                    break;
                case DisplayMode.StaticText:
                    provider = new StaticTextFrameProvider(
                        (int)this.numericUpDownBlockGap.Value,
                        (int)this.numericUpDownPadding.Value,
                        texts.ToArray());
                    break;
                default:
                    throw new NotSupportedException();
            }

            yield return new StatusRequestFrame();

            yield return provider.SetupCommand;
            foreach (var outputCommand in provider.GetOutputCommands())
            {
                yield return outputCommand;
            }

            yield return new StatusRequestFrame();
        }
    }
}
