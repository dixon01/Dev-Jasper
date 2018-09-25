// -----------------------------------------------------------------------
// <copyright file="WindowCommand.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AnnaxRendererTest.Commands
{
    using System;
    using System.Text;

    public class WindowCommand : CommandBase
    {
        private readonly int windowNumber;

        private readonly int x;

        private readonly int y;

        private readonly int width;

        private readonly int height;

        [Flags]
        public enum BaseAttr
        {
            NoAction,
            ScrollUp,
            ScrollDown,
            ScrollRight,
            ScrollLeft,
            FlipUp,
            FlipDown,
            FlipRight,
            FlipLeft,
            BlinkNormalBlank,
            BlinkNormalInvers,
            BlinkInversBlank,

            /// <summary>
            /// Scroll only if bitmap is larger than window
            /// </summary>
            AutoScroll = 16,

            /// <summary>
            /// Blink synchronously to master pulse.
            /// </summary>
            Synchronize = 32,

            /// <summary>
            /// Show the time of day.
            /// </summary>
            ClockFunction = 64,

            NotUsed = 0xFF,
        }

        public enum DurationAttr
        {
            Endless,
            EndlessNewStart,
            Once
        }

        public enum DisplayAttr
        {
            Normal,
            Inverted,
            Blank
        }

        public WindowCommand(int windowNumber, int x, int y, int width, int height)
        {
            this.windowNumber = windowNumber;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.BitmapNumber = windowNumber;
        }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public int BitmapNumber { get; set; }

        public BaseAttr Base { get; set; }

        public DurationAttr Duration { get; set; }

        public DisplayAttr Display { get; set; }

        /// <summary>
        /// Gets or sets the number of pixels for scrolling
        /// </summary>
        public int Counting { get; set; }

        /// <summary>
        /// Gets or sets the scroll or blink speed.
        /// </summary>
        public int Timing { get; set; }

        public override void AppendCommandString(StringBuilder buffer)
        {
            var command = string.Format(
                "window {0} {1} {2} {3} {4} {5} {6} {7} {8:D} {9:D} {10:D} {11} {12}\n",
                this.windowNumber,
                this.x,
                this.y,
                this.width,
                this.height,
                this.OffsetX,
                this.OffsetY,
                this.BitmapNumber,
                this.Base,
                this.Duration,
                this.Display,
                this.Timing,
                this.Counting);
            buffer.Append(command);
        }
    }
}
