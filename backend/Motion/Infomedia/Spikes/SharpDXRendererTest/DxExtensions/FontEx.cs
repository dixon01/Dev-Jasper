// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontEx.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontEx type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.DxExtensions
{
    using System;

    using SharpDX.Direct3D9;

    using Font = SharpDX.Direct3D9.Font;

    /// <summary>
    /// Font extension providing some additional information about a font.
    /// </summary>
    public class FontEx
    {
        private int? spaceWidth;
        private TextMetric metric;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontEx"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="description">
        /// The font description.
        /// </param>
        public FontEx(Device device, FontDescription description)
        {
            this.Font = new Font(device, description);
            /*device.DeviceLost += this.DeviceOnDeviceLost;
            device.DeviceReset += this.DeviceOnDeviceReset;*/
            device.Disposing += this.DeviceOnDisposing;

            /*this.focusWindow = device.CreationParameters.FocusWindow;
            if (this.focusWindow != null)
            {
                this.focusWindow.Disposed += this.FocusWindowOnDisposed;
            }*/
        }

        /// <summary>
        /// Event that is fired when this object is being disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Gets the underlying DirectX font.
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// Gets the width of a single space.
        /// </summary>
        public int SpaceWidth
        {
            get
            {
                if (!this.spaceWidth.HasValue)
                {
                    var withSpace = this.Font.MeasureText(null, "| |", FontDrawFlags.NoClip);
                    var withoutSpace = this.Font.MeasureText(null, "||", FontDrawFlags.NoClip);
                    this.spaceWidth = withSpace.Width - withoutSpace.Width;
                }

                return this.spaceWidth.Value;
            }
        }

        /// <summary>
        /// Gets the text metrics.
        /// </summary>
        public TextMetric Metrics
        {
            get
            {
                return this.metric ?? (this.metric = TextMetric.GetTextMetricsFor(this.Font));
            }
        }

        private void Dispose()
        {
            var handler = this.Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            if (this.Font.IsDisposed)
            {
                return;
            }

            /*if (this.focusWindow != null)
            {
                this.focusWindow.Disposed -= this.FocusWindowOnDisposed;
            }*/

            /*this.Font.Device.DeviceLost -= this.DeviceOnDeviceLost;
            this.Font.Device.DeviceReset -= this.DeviceOnDeviceReset;*/
            this.Font.Device.Disposing -= this.DeviceOnDisposing;
            this.Font.Dispose();
        }

        private void FocusWindowOnDisposed(object sender, EventArgs eventArgs)
        {
            this.Dispose();
        }

        private void DeviceOnDisposing(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void DeviceOnDeviceLost(object sender, EventArgs e)
        {
            if (this.Font.IsDisposed)
            {
                return;
            }

            this.Font.OnLostDevice();
        }

        private void DeviceOnDeviceReset(object sender, EventArgs e)
        {
            if (this.Font.IsDisposed)
            {
                return;
            }

            this.Font.OnResetDevice();
        }
    }
}