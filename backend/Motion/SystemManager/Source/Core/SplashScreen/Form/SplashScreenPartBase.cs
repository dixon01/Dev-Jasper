// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenPartBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenPartBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Drawing;
    using System.Text;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;

    /// <summary>
    /// Base class for visual items shown on a splash screen.
    /// </summary>
    public abstract partial class SplashScreenPartBase : IDisposable
    {
        /// <summary>
        /// Event that is risen when the content of this splash screen item changes.
        /// If this event is fired, you should call <see cref="Paint"/> again.
        /// </summary>
        public event EventHandler ContentChanged;

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public virtual Color BackColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public virtual Color ForeColor { get; set; }

        /// <summary>
        /// Gets the size of this part.
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// Creates a new <see cref="SplashScreenPartBase"/>.
        /// </summary>
        /// <param name="item">
        /// The item configuration.
        /// </param>
        /// <param name="manager">
        /// The splash screen manager.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="SplashScreenPartBase"/>.
        /// </returns>
        public static SplashScreenPartBase Create(SplashScreenItemBase item, SplashScreenManager manager)
        {
            var logo = item as LogoSplashScreenItem;
            if (logo != null)
            {
                return new LogoSplashScreenPart(logo);
            }

            var apps = item as ApplicationsSplashScreenItem;
            if (apps != null)
            {
                return new ApplicationsSplashScreenPart(apps);
            }

            var system = item as SystemSplashScreenItem;
            if (system != null)
            {
                return new SystemSplashScreenPart(system, manager);
            }

            var network = item as NetworkSplashScreenItem;
            if (network != null)
            {
                return new NetworkSplashScreenPart(network);
            }

            var gioom = item as GioomSplashScreenItem;
            if (gioom != null)
            {
                return new GioomSplashScreenPart(gioom);
            }

            var management = item as ManagementSplashScreenItem;
            if (management != null)
            {
                return new ManagementSplashScreenPart(management);
            }

            var shutdownMessage = item as ShutDownMessageSplashScreenItem;
            if (shutdownMessage != null)
            {
                return new ShutDownInformationSplashScreenPart(shutdownMessage);
            }

            throw new NotSupportedException("Splash screen item not supported: " + item.GetType().Name);
        }

        /// <summary>
        /// Scales this control with the given factor.
        /// </summary>
        /// <param name="factor">
        /// The factor (0.0 .. 1.0).
        /// </param>
        /// <param name="graphics">
        /// The graphics to calculate the scaling.
        /// </param>
        public void Scale(double factor, Graphics graphics)
        {
            this.Size = this.DoScale(factor, graphics);
        }

        /// <summary>
        /// Paints this part.
        /// </summary>
        /// <param name="g">
        /// The graphics object.
        /// </param>
        /// <param name="rect">
        /// The rectangle into which we should paint.
        /// </param>
        public abstract void Paint(Graphics g, Rectangle rect);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Implementation of the scaling.
        /// </summary>
        /// <param name="factor">
        /// The factor (0.0 .. 1.0).
        /// </param>
        /// <param name="graphics">
        /// The graphics to calculate the scaling.
        /// </param>
        /// <returns>
        /// The calculated height of this part with the used scaling factor.
        /// </returns>
        protected abstract Size DoScale(double factor, Graphics graphics);

        /// <summary>
        /// Helper method to append a line to an output string builder.
        /// This method is required for CF 2.0 compatibility.
        /// </summary>
        /// <param name="builder">
        /// The builder to which the <paramref name="line"/> will be appended.
        /// </param>
        /// <param name="line">
        /// The line without newline characters.
        /// </param>
        /// <returns>
        /// The <paramref name="builder"/>.
        /// </returns>
        protected StringBuilder AppendLine(StringBuilder builder, string line)
        {
            return builder.Append(line).Append("\r\n");
        }

        /// <summary>
        /// Raises the <see cref="ContentChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseContentChanged(EventArgs e)
        {
            var handler = this.ContentChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}