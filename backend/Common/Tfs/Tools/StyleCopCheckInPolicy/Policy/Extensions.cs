//--------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="PNC Mortgage">
//     Copyright (c) PNC Mortgage. All rights reserved.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy
{
    using System.Collections.Specialized;

    /// <summary>
    /// Contains extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="value">The <see cref="NameValueCollection"/> that is to be cloned.</param>
        /// <returns>The <see cref="NameValueCollection"/> cloned instance.</returns>
        public static NameValueCollection Clone(this NameValueCollection value)
        {
            return Clone(value, false);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="value">The <see cref="NameValueCollection"/> that is to be cloned.</param>
        /// <param name="deep"><b>true</b> if a deep copy should be performed, otherwise <b>false</b> to perform a shallow copy.</param>
        /// <returns>The <see cref="NameValueCollection"/> cloned instance.</returns>
        public static NameValueCollection Clone(this NameValueCollection value, bool deep)
        {
            NameValueCollection result = new NameValueCollection();

            foreach (string key in value.AllKeys)
            {
                string item = value[key];

                result.Add(key != null && deep ? (string)key.Clone() : key, item != null && deep ? (string)item.Clone() : item);
            }

            return result;
        }
    }
}