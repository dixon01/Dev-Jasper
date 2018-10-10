// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleListItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleListItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;

    /// <summary>
    /// The simple list item.
    /// </summary>
    public class SimpleListItem : IListItem, IComparable<SimpleListItem>, IVerifiable
    {
        private readonly string label;
        private int lastUsedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleListItem"/> class.
        /// </summary>
        public SimpleListItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleListItem"/> class.
        /// </summary>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="orderNumber">
        /// The order number.
        /// </param>
        public SimpleListItem(string label, int orderNumber)
        {
            this.Children = new List<SimpleListItem>();
            this.label = label;
            this.OrderNumber = orderNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleListItem"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="orderNumber">
        /// The order number.
        /// </param>
        public SimpleListItem(SimpleListItem parent, string label, int orderNumber)
        {
            this.Parent = parent;
            this.Children = new List<SimpleListItem>();
            this.label = label;
            this.OrderNumber = orderNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleListItem"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="children">
        /// The children.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="orderNumber">
        /// The order number.
        /// </param>
        public SimpleListItem(SimpleListItem parent, List<SimpleListItem> children, string label, int orderNumber)
        {
            this.Parent = parent;
            this.Children = children;
            this.label = label;
            this.OrderNumber = orderNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleListItem"/> class.
        /// </summary>
        /// <param name="children">
        /// The children.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="orderNumber">
        /// The order number.
        /// </param>
        public SimpleListItem(List<SimpleListItem> children, string label, int orderNumber)
        {
            this.Children = children;
            this.label = label;
            this.OrderNumber = orderNumber;
        }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        public List<SimpleListItem> Children { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        [XmlIgnore]
        public IListItem Parent { get; set; }

        /// <summary>
        ///   Gets or sets the order number. Used for sorting the list
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        ///   Gets the displayable label of this item.
        /// </summary>
        [XmlIgnore]
        public string Label
        {
            get
            {
                return this.label;
            }
        }

        /// <summary>
        ///   Gets or sets the last selected entry
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        public int LastSelection
        {
            get
            {
                return this.lastUsedIndex;
            }

            set
            {
                this.lastUsedIndex = value;
            }
        }

        /// <summary>
        /// Gets the number of levels of parents.
        /// </summary>
        [XmlIgnore]
        public int ParentCount
        {
            get
            {
                return this.Parent == null ? 0 : 1 + this.Parent.ParentCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this element is a leaf. -> no children
        /// </summary>
        [XmlIgnore]
        public bool IsLeaf
        {
            get
            {
                if (this.Children == null)
                {
                    return true;
                }

                if (this.Children.Count <= 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether  this element is the top element
        /// </summary>
        [XmlIgnore]
        public bool IsRoot
        {
            get
            {
                if (this.Parent == null)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        ///   Gets the caption for this entry. It's normally the name from the parent
        /// </summary>
        [XmlIgnore]
        public string Caption
        {
            get
            {
                if (this.Parent == null)
                {
                    return this.Label;
                }

                return this.Parent.Caption + "->" + this.Label;
            }
        }

        /// <summary>
        ///   Gets the tag value which can be used for storing list specific data.
        /// </summary>
        [XmlIgnore]
        public object Tag
        {
            get
            {
                if (this.Parent == null)
                {
                    return this.lastUsedIndex.ToString(CultureInfo.InvariantCulture);
                }

                return this.Parent.Tag + "+" + this.lastUsedIndex;
            }
        }

        /// <summary>
        /// Gets all child elements
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/> of child <see cref="IListItem"/>s.
        /// </returns>
        public List<IListItem> GetChildren()
        {
            return this.Children.ConvertAll(item => (IListItem)item);
        }

        /// <summary>
        ///   Gets the specific child from an index. The index is normally the "selectedIndex" from the received event.
        /// </summary>
        /// <param name = "childIndex">index start at 0</param>
        /// <returns>The child.</returns>
        public IListItem GetChild(int childIndex)
        {
            if (this.Children == null)
            {
                return null;
            }

            if (this.Children.Count <= childIndex)
            {
                return null;
            }

            this.lastUsedIndex = childIndex;
            return this.Children[childIndex];
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(SimpleListItem other)
        {
            if (this.OrderNumber < other.OrderNumber)
            {
                return -1;
            }

            if (this.OrderNumber == other.OrderNumber)
            {
                return 0;
            }

            return 1;
        }

        void IVerifiable.Verify()
        {
            this.SetParents();
        }

        private void SetParents()
        {
            if (this.Children != null)
            {
                foreach (SimpleListItem n in this.Children)
                {
                    n.Parent = this;
                    n.SetParents();
                }
            }
        }
    }
}