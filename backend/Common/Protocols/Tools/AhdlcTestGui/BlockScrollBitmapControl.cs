// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockScrollBitmapControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScrollBlockBitmapControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Providers;
    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// The scroll block bitmap control.
    /// </summary>
    public partial class BlockScrollBitmapControl : DataSendControlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockScrollBitmapControl"/> class.
        /// </summary>
        public BlockScrollBitmapControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This method can be overridden by subclasses to update themselves when
        /// the <see cref="DataSendControlBase.Context"/> changed or was updated.
        /// </summary>
        protected override void UpdateFromContext()
        {
            base.UpdateFromContext();
            this.backgroundCreatorControl.BitmapSize = this.Context.SignSize;
            this.backgroundCreatorControl.HasColor = this.Context.HasColor;
        }

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
            if (this.backgroundCreatorControl.Bitmap == null)
            {
                yield break;
            }

            var provider =
                new BlockScrollBitmapFrameProvider(
                    new BitmapPixelSource(this.backgroundCreatorControl.Bitmap),
                    this.Context.SignSize.Width,
                    this.Context.SignSize.Height);

            if (this.scrollBlockEditor1.BlockEnabled)
            {
                this.AddScrollBlock(provider, this.scrollBlockEditor1);
            }

            if (this.scrollBlockEditor2.BlockEnabled)
            {
                this.AddScrollBlock(provider, this.scrollBlockEditor2);
            }

            yield return new StatusRequestFrame();

            yield return provider.SetupCommand;
            foreach (var outputCommand in provider.GetOutputCommands())
            {
                yield return outputCommand;
            }

            yield return new StatusRequestFrame();
        }

        private void AddScrollBlock(BlockScrollBitmapFrameProvider provider, ScrollBlockEditorControl editor)
        {
            provider.AddScrollBlock(
                editor.Viewport,
                editor.ScrollSpeed,
                new MonochromeOffsetPixelSource(
                    editor.Bitmap.Width,
                    this.Context.SignSize.Height,
                    0,
                    editor.Viewport.Top,
                    new BitmapPixelSource(editor.Bitmap)));
        }
    }
}
