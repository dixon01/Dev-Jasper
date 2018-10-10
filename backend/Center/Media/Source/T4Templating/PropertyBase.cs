// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the base class to define properties.
    /// </summary>
    public abstract class PropertyBase : IChildItem<DataViewModelEntityDescriptor>
    {
        private readonly Lazy<bool> isHidden;

        private readonly Lazy<string> userVisibleFieldName;

        private readonly Lazy<string> userVisibleGroupName;

        private readonly Lazy<int> userVisibleOrderIndex;
        private readonly Lazy<int> userVisibleGroupOrderIndex;
        private readonly Lazy<string> fieldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBase"/> class.
        /// </summary>
        protected PropertyBase()
        {
            this.isHidden = new Lazy<bool>(this.GetIsHidden);
            this.userVisibleFieldName = new Lazy<string>(this.GetUserVisibleFieldName);
            this.userVisibleGroupName = new Lazy<string>(this.GetUserVisibleGroupName);
            this.fieldName = new Lazy<string>(this.GetFieldName);
            this.userVisibleOrderIndex = new Lazy<int>(this.GetUserVisibleOrderIndex);
            this.userVisibleGroupOrderIndex = new Lazy<int>(this.GetUserVisibleGroupOrderIndex);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
        /// </value>
        public bool IsHidden
        {
            get
            {
                return this.isHidden.Value;
            }
        }

        /// <summary>
        /// Gets the name of the user visible group.
        /// </summary>
        /// <value>
        /// The name of the user visible group.
        /// </value>
        public string UserVisibleGroupName
        {
            get
            {
                return this.userVisibleGroupName.Value;
            }
        }

        /// <summary>
        /// Gets the name of the user visible field.
        /// </summary>
        /// <value>
        /// The name of the user visible field.
        /// </value>
        public string UserVisibleFieldName
        {
            get
            {
                return this.userVisibleFieldName.Value;
            }
        }

        /// <summary>
        /// Gets the user visible order index.
        /// </summary>
        public int UserVisibleOrderIndex
        {
            get
            {
                return this.userVisibleOrderIndex.Value;
            }
        }

        /// <summary>
        /// Gets the user visible group order index.
        /// </summary>
        public int UserVisibleGroupOrderIndex
        {
            get
            {
                return this.userVisibleGroupOrderIndex.Value;
            }
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName
        {
            get
            {
                return this.fieldName.Value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the XML.
        /// </summary>
        /// <value>
        /// The name of the XML.
        /// </value>
        [XmlAttribute]
        public string XmlName { get; set; }

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        /// <value>
        /// The parent object.
        /// </value>
        [XmlIgnore]
        public DataViewModelEntityDescriptor ParentObject { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        DataViewModelEntityDescriptor IChildItem<DataViewModelEntityDescriptor>.Parent
        {
            get
            {
                return this.ParentObject;
            }

            set
            {
                this.ParentObject = value;
            }
        }

        private string GetUserVisibleGroupName()
        {
            var propertyFilter = this.FindPropertyFilter();
            if (propertyFilter != null && !string.IsNullOrEmpty(propertyFilter.UserVisibleGroupName))
            {
                return propertyFilter.UserVisibleGroupName;
            }

            var filter = this.FindDataViewModelFilter();
            if (filter == null || string.IsNullOrEmpty(filter.DefaultUserVisibleGroupName))
            {
                return "Content";
            }

            return filter.DefaultUserVisibleGroupName;
        }

        private DataViewModelFilter FindDataViewModelFilter()
        {
            if (this.ParentObject == null)
            {
                return null;
            }

            if (this.ParentObject.ParentObject == null)
            {
                return null;
            }

            if (this.ParentObject.ParentObject.ParentObject == null)
            {
                return null;
            }

            var namespaceFilter =
                this.ParentObject.ParentObject.ParentObject.Filters.NamespaceFilters.SingleOrDefault(
                    filter => filter.Name == this.ParentObject.ParentObject.Name);
            if (namespaceFilter == null)
            {
                return null;
            }

            return
                namespaceFilter.DataViewModelFilters.SingleOrDefault(filter => filter.Name == this.ParentObject.Name);
        }

        private PropertyFilter FindPropertyFilter()
        {
            var dataViewModelFilter = this.FindDataViewModelFilter();
            if (dataViewModelFilter == null)
            {
                return null;
            }

            return
                dataViewModelFilter.PropertyFilters.SingleOrDefault(
                    filter => filter.Name == this.Name);
        }

        private bool GetIsHidden()
        {
            if (this.ParentObject == null)
            {
                return false;
            }

            if (this.ParentObject.ParentObject == null)
            {
                return false;
            }

            if (this.ParentObject.ParentObject.ParentObject == null)
            {
                return false;
            }

            var result =
                this.ParentObject.ParentObject.ParentObject.Filters.NamespaceFilters.Any(
                    filter =>
                    filter.Name == this.ParentObject.ParentObject.Name
                    && filter.DataViewModelFilters.Any(
                        modelFilter =>
                        modelFilter.Name == this.ParentObject.Name
                        && modelFilter.PropertyFilters.Any(
                            propertyFilter => propertyFilter.Name == this.Name && propertyFilter.IsHidden)));
            return result;
        }

        private string GetUserVisibleFieldName()
        {
            var propertyFilter = this.FindPropertyFilter();
            if (propertyFilter == null || string.IsNullOrEmpty(propertyFilter.UserVisibleGroupName))
            {
                return null;
            }

            return propertyFilter.UserVisibleFieldName;
        }

        private int GetUserVisibleOrderIndex()
        {
            var propertyFilter = this.FindPropertyFilter();
            if (propertyFilter == null)
            {
                return 0;
            }

            return propertyFilter.UserVisibleOrderIndex;
        }

        private int GetUserVisibleGroupOrderIndex()
        {
            var propertyFilter = this.FindPropertyFilter();
            if (propertyFilter != null && propertyFilter.UserVisibleGroupOrderIndex != 0)
            {
                return propertyFilter.UserVisibleGroupOrderIndex;
            }

            var filter = this.FindDataViewModelFilter();
            if (filter == null)
            {
                return 0;
            }

            return filter.DefaultUserVisibleGroupOrderIndex;
        }

        private string GetFieldName()
        {
            return this.Name.ToLower();
        }
    }
}