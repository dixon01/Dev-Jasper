// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextMetric.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.DxExtensions
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// DirectX text metric.
    /// </summary>
    public sealed class TextMetric
    {
        private TextMetricW metric;

        private TextMetric()
        {
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool GetTextMetricsWFunction(IntPtr font, ref TextMetricW pTextMetrics);

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the ascent.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the descent.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the internal leading.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the external leading.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the ave char width.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the max char width.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the overhang.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the digitized aspect x.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the digitized aspect y.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the first char.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the last char.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the default char.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the break char.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether this font is italic.
        /// </summary>
        public bool Italic
        {
            get
            {
                return this.metric.Italic != 0;
            }

            set
            {
                this.metric.Italic = value ? (byte)1 : (byte)0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this font is underlined.
        /// </summary>
        public bool Underlined
        {
            get
            {
                return this.metric.Underlined != 0;
            }

            set
            {
                this.metric.Underlined = value ? (byte)1 : (byte)0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this font is struck out.
        /// </summary>
        public bool StruckOut
        {
            get
            {
                return this.metric.StruckOut != 0;
            }

            set
            {
                this.metric.StruckOut = value ? (byte)1 : (byte)0;
            }
        }

        /// <summary>
        /// Gets or sets the pitch and family.
        /// </summary>
        public PitchAndFamily PitchAndFamily
        {
            get
            {
                return (PitchAndFamily)this.metric.PitchAndFamily;
            }

            set
            {
                this.metric.PitchAndFamily = (byte)value;
            }
        }

        /// <summary>
        /// Gets or sets the char set.
        /// </summary>
        public CharacterSet CharSet
        {
            get
            {
                return (CharacterSet)this.metric.CharSet;
            }

            set
            {
                this.metric.CharSet = (byte)value;
            }
        }

        /// <summary>
        /// Gets the text metrics for a given DirectX font.
        /// </summary>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <returns>
        /// The <see cref="TextMetric"/> or null if it can't be retrieved.
        /// </returns>
        public static unsafe TextMetric GetTextMetricsFor(Font font)
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

        /// <summary>
        /// Managed struct matching TEXTMETRICW from <code>wingdi.h</code> 
        /// used by <code>d3dx9core.h</code>.
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
