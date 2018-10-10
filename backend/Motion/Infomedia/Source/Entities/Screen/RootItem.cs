// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootItem.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RootItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// The root element of the screen hierarchy.
    /// It contains all items to be shown on the screen.
    /// </summary>
    public class RootItem : ItemBase, IManageable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootItem"/> class.
        /// </summary>
        public RootItem()
        {
            this.Items = new List<ScreenItemBase>();
        }

        /// <summary>
        /// Gets or sets a list of all items on this screen.
        /// </summary>
        public List<ScreenItemBase> Items { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the screen.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = (RootItem)base.Clone();
            clone.Items = this.Items.ConvertAll(i => (ScreenItemBase)i.Clone());
            return clone;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Root {");
            foreach (var item in this.Items)
            {
                sb.Append(item);
                sb.Append(',');
            }

            if (this.Items.Count > 0)
            {
                sb.Length--;
            }

            sb.Append('}');
            return sb.ToString();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var item in this.Items)
            {
                yield return
                    parent.Factory.CreateManagementProvider(
                        string.Format("{0} {1}", item.GetType().Name, item.Id.ToString(CultureInfo.InvariantCulture)),
                        parent,
                        item);
            }
        }
    }
}
