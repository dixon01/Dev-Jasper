// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiLangListItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiLangListItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The multi lang list item.
    /// </summary>
    public class MultiLangListItem : IListItem, IComparable<MultiLangListItem>, IVerifiable
    {
        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        public string[] Labels { get; set; }

        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        [XmlElement("Command")]
        public CommandConfigItem CommandName { get; set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        [XmlIgnore]
        public string Label
        {
            get
            {
                LanguageManager langs = LanguageManager.Instance;
                int index = Array.IndexOf(langs.SupportedLanguages, langs.CurrentLanguage);
                if (index < 0 || index >= this.Labels.Length || string.IsNullOrEmpty(this.Labels[index]))
                {
                    return this.Labels[0];
                }

                return this.Labels[index];
            }
        }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        public List<MultiLangListItem> Children { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        [XmlAttribute("index")]
        public int OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        [XmlIgnore]
        public IListItem Parent { get; set; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        [XmlIgnore]
        public object Tag
        {
            get
            {
                return this.CommandName.Command;
            }
        }

        /// <summary>
        /// Gets or sets the last selection.
        /// </summary>
        [XmlIgnore]
        public int LastSelection { get; set; }

        /// <summary>
        /// Gets a value indicating whether this element is a leaf. -> no children
        /// </summary>
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
        public string Caption
        {
            get
            {
                if (this.Parent == null)
                {
                    return this.Label;
                }

                return this.Parent.Caption + " > " + this.Label;
            }
        }

        /// <summary>
        /// Gets the number of levels of parents.
        /// </summary>
        /// <value></value>
        public int ParentCount
        {
            get
            {
                return this.Parent == null ? 0 : 1 + this.Parent.ParentCount;
            }
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

            this.LastSelection = childIndex;
            return this.Children[childIndex];
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
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(MultiLangListItem other)
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

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var sb = new StringBuilder();
            string tabs = string.Empty;
            for (int i = 0; i < this.ParentCount - 1; i++)
            {
                tabs += "\t";
            }

            if (this.IsLeaf)
            {
                return tabs + this.Label + "-->" + this.CommandName;
            }

            sb.Append(tabs + this.Caption + "\n");
            foreach (var child in this.Children)
            {
                sb.Append(tabs + child + "\n");
            }

            return sb.ToString();
        }

        void IVerifiable.Verify()
        {
            this.SetParents();
        }

        private void SetParents()
        {
            if (this.Children != null)
            {
                foreach (MultiLangListItem n in this.Children)
                {
                    n.Parent = this;
                    n.SetParents();
                }
            }
        }
    }
}