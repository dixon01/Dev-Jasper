// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelEntityDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelEntityDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Describes a data view model to be generated.
    /// </summary>
    public class DataViewModelEntityDescriptor : IChildItem<NamespaceEntityDescriptor>
    {
        private readonly Lazy<bool> shouldGenerate;

        private readonly Lazy<bool> isRoot;

        private readonly Lazy<string> entityName;

        private readonly Lazy<string> viewModelName;

        private readonly Lazy<string> dataModelName;

        private readonly Lazy<string> converterName;

        private readonly Lazy<string> baseConverterName;

        private readonly Lazy<DataViewModelEntityDescriptor> baseDataViewModel;

        private readonly Lazy<string> fullQualifiedViewModelName;

        private readonly Lazy<string> fullQualifiedDataModelName;

        private readonly Lazy<bool> isReference;

        private readonly Lazy<bool> requiresLayoutEditorConstructorParameter;

        private readonly Lazy<bool> isAbstract;

        private readonly Lazy<string> fullQualifiedNamespace;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewModelEntityDescriptor"/> class.
        /// </summary>
        public DataViewModelEntityDescriptor()
        {
            this.baseDataViewModel = new Lazy<DataViewModelEntityDescriptor>(this.GetBaseDataViewModel);
            this.fullQualifiedViewModelName = new Lazy<string>(this.GetFullQualifiedViewModelName);
            this.fullQualifiedDataModelName = new Lazy<string>(this.GetFullQualifiedDataModelName);
            this.shouldGenerate = new Lazy<bool>(this.GetShouldGenerate);
            this.isRoot = new Lazy<bool>(this.GetIsRoot);
            this.entityName = new Lazy<string>(this.GetEntityName);
            this.viewModelName = new Lazy<string>(this.GetViewModelName);
            this.dataModelName = new Lazy<string>(this.GetDataModelName);
            this.converterName = new Lazy<string>(this.GetConverterName);
            this.baseConverterName = new Lazy<string>(this.GetBaseConverterName);
            this.fullQualifiedNamespace = new Lazy<string>(this.GetFullQualifiedNameSpace);
            this.isReference = new Lazy<bool>(this.GetIsReference);
            this.isAbstract = new Lazy<bool>(this.GetIsAbstract);
            this.requiresLayoutEditorConstructorParameter =
                new Lazy<bool>(this.GetRequiresLayoutEditorConstructorParameter);
            this.PropertyDescriptors = new ChildItemCollection<DataViewModelEntityDescriptor, PropertyBase>(this);
        }

        /// <summary>
        /// Gets the base data view model.
        /// </summary>
        public DataViewModelEntityDescriptor BaseDataViewModel
        {
            get
            {
                return this.baseDataViewModel.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is reference.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is reference; otherwise, <c>false</c>.
        /// </value>
        public bool IsReference
        {
            get
            {
                return this.isReference.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether it should be generated.
        /// </summary>
        /// <value>
        /// <c>true</c> if it should be generated otherwise, <c>false</c>.
        /// </value>
        public bool ShouldGenerate
        {
            get
            {
                return this.shouldGenerate.Value;
            }
        }

        /// <summary>
        /// Gets the full name of the qualified view model.
        /// </summary>
        /// <value>
        /// The full name of the qualified view model.
        /// </value>
        public string FullQualifiedViewModelName
        {
            get
            {
                return this.fullQualifiedViewModelName.Value;
            }
        }

        /// <summary>
        /// Gets the full name of the qualified data model.
        /// </summary>
        /// <value>
        /// The full name of the qualified data model.
        /// </value>
        public string FullQualifiedDataModelName
        {
            get
            {
                return this.fullQualifiedDataModelName.Value;
            }
        }

        /// <summary>
        /// Gets the full name of the qualified data model namespace.
        /// </summary>
        /// <value>
        /// The full name of the qualified data model namespace.
        /// </value>
        public string FullQualifiedDataModelNamespace
        {
            get
            {
                return this.fullQualifiedNamespace.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether it requires the layout editor constructor parameter.
        /// </summary>
        /// <value>
        /// <c>true</c> if it requires the layout editor constructor parameter; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresLayoutEditorConstructorParameter
        {
            get
            {
                return this.requiresLayoutEditorConstructorParameter.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is root.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is root; otherwise, <c>false</c>.
        /// </value>
        public bool IsRoot
        {
            get
            {
                return this.isRoot.Value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the base entity.
        /// </summary>
        /// <value>
        /// The name of the base entity.
        /// </value>
        [XmlAttribute]
        public string Base { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether data view model should be abstract.
        /// </summary>
        /// <value>
        /// <c>true</c> if the data view model should be abstract; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IsAbstract
        {
            get
            {
                return this.isAbstract.Value;
            }
        }

        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        public string EntityName
        {
            get
            {
                return this.entityName.Value;
            }
        }

        /// <summary>
        /// Gets the name of the view model.
        /// </summary>
        /// <value>
        /// The name of the view model.
        /// </value>
        public string ViewModelName
        {
            get
            {
                return this.viewModelName.Value;
            }
        }

        /// <summary>
        /// Gets the name of the data model.
        /// </summary>
        /// <value>
        /// The name of the data model.
        /// </value>
        public string DataModelName
        {
            get
            {
                return this.dataModelName.Value;
            }
        }

        /// <summary>
        /// Gets the name of the converter.
        /// </summary>
        /// <value>
        /// The name of the converter.
        /// </value>
        public string ConverterName
        {
            get
            {
                return this.converterName.Value;
            }
        }

        /// <summary>
        /// Gets the name of the base converter.
        /// </summary>
        /// <value>
        /// The name of the base converter.
        /// </value>
        public string BaseConverterName
        {
            get
            {
                return this.baseConverterName.Value;
            }
        }

        /// <summary>
        /// Gets or sets the parent data view model.
        /// </summary>
        /// <value>
        /// The parent data view model, if any; <c>null</c> if the data view model should directly inherit from
        /// DataViewModelBase.
        /// </value>
        [XmlIgnore]
        public NamespaceEntityDescriptor ParentObject { get; set; }

        /// <summary>
        /// Gets the property descriptors.
        /// </summary>
        [XmlElement("Property", typeof(Property))]
        [XmlElement("CompositeProperty", typeof(CompositeProperty))]
        [XmlElement("ReferenceProperty", typeof(ReferenceProperty))]
        [XmlElement("ListProperty", typeof(ListProperty))]
        public ChildItemCollection<DataViewModelEntityDescriptor, PropertyBase> PropertyDescriptors
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        NamespaceEntityDescriptor IChildItem<NamespaceEntityDescriptor>.Parent
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

        /// <summary>
        /// Gets the base element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The base element.</returns>
        internal DataViewModelEntityDescriptor GetBaseElement(string name)
        {
            var parts = name.Split('.');
            NamespaceEntityDescriptor ns;
            if (parts.Length > 1)
            {
                var partsName = parts.Length > 2
                                    ? parts.Take(parts.Length - 1).Aggregate((p1, p2) => p1 + '.' + p2)
                                    : parts[0];
                ns =
                    this.ParentObject.ParentObject.NamespaceEntityDescriptors
                        .SingleOrDefault(n => n.Name == partsName);
            }
            else
            {
                ns = this.ParentObject;
            }

            var baseName = parts[parts.Length - 1];
            if (ns == null)
            {
                const string Format = "Can't find namespace for base element '{0}' for element '{1}'";
                var message = string.Format(Format, baseName, this.Name);
                throw new InvalidDataException(message);
            }

            var baseElement =
                ns.DataViewModelEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == baseName);
            if (baseElement == null)
            {
                const string Format = "Can't find base element named '{0}' for element '{1}' in namespace '{2}'";
                var message = string.Format(Format, baseName, this.Name, ns);
                throw new InvalidDataException(message);
            }

            return baseElement;
        }

        /// <summary>
        /// Gets a value indicating whether this entity is a root.
        /// </summary>
        /// <returns><c>true</c> if this entity is a root; otherwise, <c>false</c>.</returns>
        protected virtual bool GetIsRoot()
        {
            return string.IsNullOrEmpty(this.Base) || this.Base == "Base";
        }

        /// <summary>
        /// Gets a value indicating whether this entity is a root.
        /// </summary>
        /// <returns><c>true</c> if this entity is a root; otherwise, <c>false</c>.</returns>
        protected virtual bool GetIsAbstract()
        {
            return this.Name.EndsWith("Base");
        }

        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        /// <returns>The name of the entity.</returns>
        protected virtual string GetEntityName()
        {
            if (this.ParentObject == null)
            {
                return null;
            }

            if (this.ParentObject.Name == "Layout")
            {
                return this.Name == "Font" ? "Font" : GetNameWithSuffix(this.Name, "Element");
            }

            if (this.ParentObject.Name == "Eval" && this.Name.EndsWith("DynamicProperty"))
            {
                return this.Name;
            }

            if (this.ParentObject.Name == "Layout.Cycle" && this.Name == "GenericTrigger")
            {
                return GetNameWithSuffix(this.Name, "Config");
            }

            return GetNameWithSuffix(this.Name, this.ParentObject.Name == "Eval" ? "Eval" : "Config");
        }

        /// <summary>
        /// Gets the name of the view model.
        /// </summary>
        /// <returns>The name of the view model.</returns>
        protected virtual string GetViewModelName()
        {
            return this.GetViewModelName(this.EntityName);
        }

        /// <summary>
        /// Gets the name of the data model.
        /// </summary>
        /// <returns>
        /// The name of the data model.
        /// </returns>
        protected virtual string GetDataModelName()
        {
            return this.ViewModelName.Replace("DataViewModel", "DataModel");
        }

        /// <summary>
        /// Gets the name of the converter.
        /// </summary>
        /// <returns>The name of the converter.</returns>
        protected virtual string GetConverterName()
        {
            if (string.IsNullOrEmpty(this.ViewModelName))
            {
                return null;
            }

            return this.ViewModelName + "Converter";
        }

        /// <summary>
        /// Gets the name of the base converter.
        /// </summary>
        /// <returns>The name of the base converter.</returns>
        protected virtual string GetBaseConverterName()
        {
            if (this.IsRoot)
            {
                return "DataViewModelConverterBase";
            }

            var baseElement = this.GetBaseElement(this.Base);
            return baseElement.ConverterName;
        }

        /// <summary>
        /// Gets the base data view model.
        /// </summary>
        /// <returns>The base data view model.</returns>
        protected virtual DataViewModelEntityDescriptor GetBaseDataViewModel()
        {
            if (string.IsNullOrEmpty(this.Base) || (this.Base == "Base" && this.ParentObject.Name != "Layout"))
            {
                return new DataViewModelEntityDescriptor { Name = "DataViewModelBase" };
            }

            if (this.Base == "Base" && this.ParentObject.Name == "Layout")
            {
                return new DataViewModelEntityDescriptor { Name = "LayoutElementDataViewModelBase" };
            }

            if (this.Base == "DynamicProperty")
            {
                return new DataViewModelEntityDescriptor { Name = "DynamicDataValue<EvalDataViewModelBase>" };
            }

            var baseElement = this.GetBaseElement(this.Base);

            return baseElement;
        }

        /// <summary>
        /// Gets the name of the view model.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The name of the view model.</returns>
        protected virtual string GetViewModelName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "DataViewModelBase";
            }

            if (name.EndsWith("Base"))
            {
                return name.Substring(0, name.Length - 4) + "DataViewModelBase";
            }

            return name + "DataViewModel";
        }

        private static string GetNameWithSuffix(string name, string suffix)
        {
            if (name.EndsWith("Base"))
            {
                return name.Substring(0, name.Length - 4) + suffix + "Base";
            }

            return name + suffix;
        }

        private bool GetRequiresLayoutEditorConstructorParameter()
        {
            var typesNotRequiring = new[]
                {
                    "DataViewModelBase", "EvalDataViewModelBase"
                };
            return !typesNotRequiring.Contains(this.BaseDataViewModel.ViewModelName);
        }

        private bool GetIsReference()
        {
            return this.Name.EndsWith("Ref");
        }

        private string GetFullQualifiedViewModelName()
        {
            if (this.ParentObject == null)
            {
                return "Gorba.Center.Common.Wpf.Framework.DataViewModels.DataViewModelBase";
            }

            return this.ParentObject.ParentObject.Filters.BaseGeneratedNamespace + "." + this.ParentObject.Name + "."
                   + this.ViewModelName;
        }

        private string GetFullQualifiedNameSpace()
        {
            if (this.ParentObject == null)
            {
                return "Gorba.Center.Media.Core.Models";
            }

            return "Gorba.Center.Media.Core.Models." + this.ParentObject.Name;
        }

        private string GetFullQualifiedDataModelName()
        {
            if (this.ParentObject == null)
            {
                return "Gorba.Center.Media.Core.Models.DataModelBase";
            }

            return "Gorba.Center.Media.Core.Models." + this.ParentObject.Name + "."
                   + this.DataModelName;
        }

        private bool GetShouldGenerate()
        {
            return this.Name != "Base";
        }
    }
}