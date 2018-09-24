namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Used to sort display modes
    /// </summary>
    public class DisplayModeSorter : IComparer
    {
        #region IComparer Members

        /// <summary>
        /// Compare two display modes
        /// </summary>
        public int Compare(object x, object y)
        {
            DisplayMode d1 = (DisplayMode)x;
            DisplayMode d2 = (DisplayMode)y;

            if (d1.Width > d2.Width)
                return +1;
            if (d1.Width < d2.Width)
                return -1;
            if (d1.Height > d2.Height)
                return +1;
            if (d1.Height < d2.Height)
                return -1;
            if (d1.Format > d2.Format)
                return +1;
            if (d1.Format < d2.Format)
                return -1;
            if (d1.RefreshRate > d2.RefreshRate)
                return +1;
            if (d1.RefreshRate < d2.RefreshRate)
                return -1;

            // They must be the same, return 0
            return 0;
        }

        #endregion
    }
}