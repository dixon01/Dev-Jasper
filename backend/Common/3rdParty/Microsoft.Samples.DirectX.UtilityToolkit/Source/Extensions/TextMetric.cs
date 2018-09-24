namespace Microsoft.Samples.DirectX.UtilityToolkit.Extensions
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.DirectX.Direct3D;

    public sealed class TextMetric
    {
        private TextMetricW metric;

        private TextMetric()
        {
        }

        public int Height
        {
            get
            {
                return this.metric.Height;
            }
            set
            {
                this.metric.Height = value;
            }
        }

        public int Ascent
        {
            get
            {
                return this.metric.Ascent;
            }
            set
            {
                this.metric.Ascent = value;
            }
        }

        public int Descent
        {
            get
            {
                return this.metric.Descent;
            }
            set
            {
                this.metric.Descent = value;
            }
        }

        public int InternalLeading
        {
            get
            {
                return this.metric.InternalLeading;
            }
            set
            {
                this.metric.InternalLeading = value;
            }
        }

        public int ExternalLeading
        {
            get
            {
                return this.metric.ExternalLeading;
            }
            set
            {
                this.metric.ExternalLeading = value;
            }
        }

        public int AveCharWidth
        {
            get
            {
                return this.metric.AveCharWidth;
            }
            set
            {
                this.metric.AveCharWidth = value;
            }
        }

        public int MaxCharWidth
        {
            get
            {
                return this.metric.MaxCharWidth;
            }
            set
            {
                this.metric.MaxCharWidth = value;
            }
        }

        public int Weight
        {
            get
            {
                return this.metric.Weight;
            }
            set
            {
                this.metric.Weight = value;
            }
        }

        public int Overhang
        {
            get
            {
                return this.metric.Overhang;
            }
            set
            {
                this.metric.Overhang = value;
            }
        }

        public int DigitizedAspectX
        {
            get
            {
                return this.metric.DigitizedAspectX;
            }
            set
            {
                this.metric.DigitizedAspectX = value;
            }
        }

        public int DigitizedAspectY
        {
            get
            {
                return this.metric.DigitizedAspectY;
            }
            set
            {
                this.metric.DigitizedAspectY = value;
            }
        }

        public char FirstChar
        {
            get
            {
                return this.metric.FirstChar;
            }
            set
            {
                this.metric.FirstChar = value;
            }
        }

        public char LastChar
        {
            get
            {
                return this.metric.LastChar;
            }
            set
            {
                this.metric.LastChar = value;
            }
        }

        public char DefaultChar
        {
            get
            {
                return this.metric.DefaultChar;
            }
            set
            {
                this.metric.DefaultChar = value;
            }
        }

        public char BreakChar
        {
            get
            {
                return this.metric.BreakChar;
            }
            set
            {
                this.metric.BreakChar = value;
            }
        }

        public byte Italic
        {
            get
            {
                return this.metric.Italic;
            }
            set
            {
                this.metric.Italic = value;
            }
        }

        public byte Underlined
        {
            get
            {
                return this.metric.Underlined;
            }
            set
            {
                this.metric.Underlined = value;
            }
        }

        public byte StruckOut
        {
            get
            {
                return this.metric.StruckOut;
            }
            set
            {
                this.metric.StruckOut = value;
            }
        }

        public byte PitchAndFamily
        {
            get
            {
                return this.metric.PitchAndFamily;
            }
            set
            {
                this.metric.PitchAndFamily = value;
            }
        }

        public byte CharSet
        {
            get
            {
                return this.metric.CharSet;
            }
            set
            {
                this.metric.CharSet = value;
            }
        }

        public unsafe static TextMetric GetTextMetricsFor(Font font)
        {
            var textMetric = new TextMetric();

            var ptr = font.UnmanagedComPointer;
            var func = (GetTextMetricsWFunction)Marshal.GetDelegateForFunctionPointer(
                new IntPtr(*(int*)(*(int*)ptr + 28)), typeof(GetTextMetricsWFunction));
            if (!func(new IntPtr(ptr), ref textMetric.metric))
            {
                return null;
            }

            return textMetric;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool GetTextMetricsWFunction(IntPtr font, ref TextMetricW pTextMetrics);

        /// <summary>
        /// Managed struct matching TEXTMETRICW from wingdi.h used by d3dx9core.h
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TextMetricW
        {
            public int Height;

            public int Ascent;

            public int Descent;

            public int InternalLeading;

            public int ExternalLeading;

            public int AveCharWidth;

            public int MaxCharWidth;

            public int Weight;

            public int Overhang;

            public int DigitizedAspectX;

            public int DigitizedAspectY;

            public char FirstChar;

            public char LastChar;

            public char DefaultChar;

            public char BreakChar;

            public byte Italic;

            public byte Underlined;

            public byte StruckOut;

            public byte PitchAndFamily;

            public byte CharSet;
        }
    }
}
