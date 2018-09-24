// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeviceConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The device config.
    /// </summary>
    [Serializable]
    public class DeviceConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceConfig"/> class.
        /// </summary>
        public DeviceConfig()
        {
            this.SwapEffect = SwapEffects.Discard;
            this.MultiThreaded = true;
            this.MultiSample = MultiSampleTypes.None;
            this.MultiSampleQuality = 0;
            this.PresentationInterval = PresentIntervals.One;
            this.PresentFlag = PresentFlags.None;
        }

        /// <summary>
        /// Gets or sets the swap effect.
        /// Default value is <see cref="SwapEffects.Discard"/>.
        /// </summary>
        [DefaultValue(SwapEffects.Discard)]
        public SwapEffects SwapEffect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DirectX should
        /// support multi threading.
        /// Default value is true.
        /// </summary>
        [DefaultValue(true)]
        public bool MultiThreaded { get; set; }

        /// <summary>
        /// Gets or sets the multi sample type.
        /// Default value is <see cref="MultiSampleTypes.None"/>.
        /// </summary>
        [DefaultValue(MultiSampleTypes.None)]
        public MultiSampleTypes MultiSample { get; set; }

        /// <summary>
        /// Gets or sets the multi sample quality.
        /// Default value is 0.
        /// </summary>
        [DefaultValue(0)]
        public int MultiSampleQuality { get; set; }

        /// <summary>
        /// Gets or sets the presentation interval.
        /// Set this to Immediate if you want more refreshing than the
        /// screen refresh rate (doesn't make sense except for performance measurements).
        /// Default value is <see cref="PresentIntervals.One"/> meaning one frame
        /// is rendered per refresh of the screen.
        /// </summary>
        [DefaultValue(PresentIntervals.One)]
        public PresentIntervals PresentationInterval { get; set; }

        /// <summary>
        /// Gets or sets the presentation flags.
        /// Default value is <see cref="PresentFlags.None"/>.
        /// </summary>
        [DefaultValue(PresentFlags.None)]
        public PresentFlags PresentFlag { get; set; }
    }
}