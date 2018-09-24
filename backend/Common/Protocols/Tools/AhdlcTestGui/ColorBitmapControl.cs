// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorBitmapControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColorBitmapControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Providers;
    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// Control for sending color bitmaps.
    /// </summary>
    public partial class ColorBitmapControl : DataSendControlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBitmapControl"/> class.
        /// </summary>
        public ColorBitmapControl()
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
            this.bitmapCreatorControl.BitmapSize = this.Context.SignSize;
            this.bitmapCreatorControl.HasColor = this.Context.HasColor;
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
            if (this.bitmapCreatorControl.Bitmap == null)
            {
                yield break;
            }

            yield return new StatusRequestFrame();

            var provider = new ColorBitmapFrameProvider(0, new BitmapPixelSource(this.bitmapCreatorControl.Bitmap));
            yield return provider.SetupCommand;
            foreach (var outputCommand in provider.GetOutputCommands())
            {
                yield return outputCommand;
            }

            yield return new StatusRequestFrame();
        }
    }
}
