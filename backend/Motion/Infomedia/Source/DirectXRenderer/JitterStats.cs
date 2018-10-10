// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JitterStats.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  Jitter statistics.
    /// </summary>
    public class JitterStats
    {
        private const int WindowSize = 300;
        private Moments mainMoments;
        private Moments windowMoments;

        // private List<long> samples = new List<long>(100000);
        // private long[] window = new long[windowSize];
        private Queue<long> window = new Queue<long>(WindowSize);

        private bool discardFirst = true;

        /// <summary>
        /// Gets the number of samples.
        /// </summary>
        public int Count
        {
            get { return this.mainMoments.N; }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.mainMoments + " Moving=" + this.windowMoments;
        }

        /// <summary>
        /// The add sample.
        /// </summary>
        /// <param name="x">
        /// A new sample.
        /// </param>
        public void AddSample(long x)
        {
            if (this.discardFirst)
            {
                this.discardFirst = false;
                return;
            }

            this.mainMoments.N++;
            double delta = x - this.mainMoments.M1;
            this.mainMoments.M1 += delta / this.mainMoments.N;
            this.mainMoments.M2 += delta * (x - this.mainMoments.M1);

            // samples.Add(x);
            if (this.window.Count < WindowSize)
            {
                this.windowMoments = this.mainMoments;
            }

            if (this.window.Count == WindowSize)
            {
                long z = this.window.Dequeue();
                delta = (x - z) - this.windowMoments.M1;
                this.windowMoments.M1 += delta / WindowSize;
                this.windowMoments.M2 += delta * (x - this.windowMoments.M1 - z);
            }

            this.window.Enqueue(x);
            this.windowMoments.N = this.window.Count;
        }

        /*
        public void WriteSamples(string path)
        {
            string[] lines = new string[samples.Count];
            int i = 0;
            foreach (var x in samples)
            {
                lines[i++] = x.ToString();
            }
            File.WriteAllLines(path, lines);
        }*/

        /// <summary>
        /// The moments.
        /// </summary>
        internal struct Moments
        {
            /// <summary>
            /// Used to store the first moment
            /// </summary>
            public double M1;

            /// <summary>
            /// Used for storing the second moment.
            /// </summary>
            public double M2;

            /// <summary>
            /// Number of seen samples.
            /// </summary>
            public int N;

            /// <summary>
            /// Gets the mean.
            /// </summary>
            public double Mean
            {
                get { return this.M1; }
            }

            /// <summary>
            /// Gets the standard deviation.
            /// </summary>
            public double Std
            {
                get
                {
                    if (this.N < 2)
                    {
                        return double.NaN;
                    }
                    else
                    {
                        return Math.Sqrt(this.M2 / (this.N - 1));
                    }
                }
            }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return string.Format(
                    "({0:000000},<{1:0.00},{2:0.00}>)",
                    this.N,
                    this.Mean,
                    this.Std);
            }
        }
    }
}
