// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System;
    using System.Xml.Schema;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// A wrapper around a model property of type <see cref="Gorba.Center.Common.ServiceModel.XmlData"/>.
    /// </summary>
    public class XmlDataViewModel : ViewModelBase
    {
        private readonly Func<XmlData> getter;
        private readonly Action<XmlData> setter;

        private XmlData? xmlData;

        private XmlSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataViewModel"/> class.
        /// </summary>
        /// <param name="getter">
        /// The property getter method.
        /// </param>
        /// <param name="setter">
        /// The property setter method.
        /// </param>
        public XmlDataViewModel(Func<XmlData> getter, Action<XmlData> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        /// <summary>
        /// Gets or sets the XML structure as a string.
        /// </summary>
        public string Xml
        {
            get
            {
                return this.getter().Xml;
            }

            set
            {
                if (value == this.Xml)
                {
                    return;
                }

                this.setter(new XmlData(value, this.Type));
                this.xmlData = null;
                this.RaisePropertyChanged(() => this.Xml);
            }
        }

        /// <summary>
        /// Gets or sets the qualified type name of the serialized data (type name + assembly name).
        /// </summary>
        public string Type
        {
            get
            {
                return this.getter().Type;
            }

            set
            {
                if (value == this.Type)
                {
                    return;
                }

                this.setter(new XmlData(this.Xml, value));
                this.xmlData = null;
                this.RaisePropertyChanged(() => this.Type);
            }
        }

        /// <summary>
        /// Gets or sets the actual XML data.
        /// </summary>
        public XmlData XmlData
        {
            get
            {
                if (!this.xmlData.HasValue)
                {
                    this.xmlData = this.getter();
                }

                return this.xmlData.Value;
            }

            set
            {
                if (this.XmlData == value)
                {
                    return;
                }

                this.setter(value);
                this.xmlData = value;
                this.RaisePropertyChanged(() => this.Xml);
                this.RaisePropertyChanged(() => this.Type);
            }
        }

        /// <summary>
        /// Gets or sets the XML schema that can be used to validate this item.
        /// If this is null, no schema validation should take place.
        /// </summary>
        public XmlSchema Schema
        {
            get
            {
                return this.schema;
            }

            set
            {
                this.SetProperty(ref this.schema, value, () => this.Schema);
            }
        }
    }
}
