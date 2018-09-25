// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupElement.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    using System.Text;

    /// <summary>
    /// Group of elements.
    /// </summary>
    public partial class GroupElement
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Group[");
            foreach (var element in this.Elements)
            {
                builder.Append(element).Append(", ");
            }

            builder.Length -= 2;
            builder.Append("]");
            return builder.ToString();
        }
    }
}
