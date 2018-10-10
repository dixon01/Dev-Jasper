// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HtmlRendererTest.Renderers
{
    using System.IO;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// A renderer for the analog clock.
    /// </summary>
    public class AnalogClockRenderer : RendererBase
    {
        private string hourUrl;

        private string minuteUrl;

        private string secondsUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogClockRenderer"/> class.
        /// </summary>
        /// <param name="analogClock">
        /// The analog clock.
        /// </param>
        public AnalogClockRenderer(AnalogClockItem analogClock)
            : base(analogClock)
        {
        }

        /// <summary>
        /// Prepares this renderer.
        /// </summary>
        public override void Prepare()
        {
            this.LoadImages();
        }

        /// <summary>
        /// Creates the JSON object that will be sent to the browser
        /// </summary>
        /// <returns>
        /// an object that is JSON-serializable that contains all information about
        /// this renderer.
        /// </returns>
        public override JsonDrawItem CreateJsonObject()
        {
            var item = (AnalogClockItem)this.Item.Clone();
            if (this.hourUrl != null)
            {
                item.Hour.Filename = this.hourUrl;
            }

            if (this.minuteUrl != null)
            {
                item.Minute.Filename = this.minuteUrl;
            }

            if (this.secondsUrl != null)
            {
                item.Seconds.Filename = this.secondsUrl;
            }

            return new JsonAnalogClockItem(item);
        }

        private void LoadImages()
        {
            var clock = (AnalogClockItem)this.Item;
            this.hourUrl = this.GetImageUrl(clock.Hour);
            this.minuteUrl = this.GetImageUrl(clock.Minute);
            this.secondsUrl = this.GetImageUrl(clock.Seconds);
        }

        private string GetImageUrl(AnalogClockHandItem hand)
        {
            return hand == null ? null : ImageProvider.Instance.GetPathFor(new FileInfo(hand.Filename));
        }

        private class JsonAnalogClockItem : JsonDrawItem
        {
            public JsonAnalogClockItem(AnalogClockItem item)
                : base(item)
            {
            }

            /// <summary>
            /// Gets the time of day in milliseconds.
            /// This property is only used by JSON and JavaScript.
            /// </summary>
            // ReSharper disable UnusedMember.Local
            public int Time
            {
                get
                {
                    // TODO: is this the correct time zone?
                    return (int)TimeProvider.Current.Now.TimeOfDay.TotalMilliseconds;
                }
            }

            // ReSharper restore UnusedMember.Local
        }
    }
}