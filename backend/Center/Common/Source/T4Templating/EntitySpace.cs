// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySpace.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntitySpace type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.T4Templating
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Center.Common.T4Templating.Utility;

    /// <summary>
    /// Defines a partitioned set of entities.
    /// </summary>
    [XmlRoot("EntitySpace", Namespace = Xmlns)]
    public class EntitySpace
    {
        /// <summary>
        /// Xml namespace used for xml serialization.
        /// </summary>
        internal const string Xmlns = "http://schemas.gorba.com/Center/EntitySpace";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySpace"/> class.
        /// </summary>
        public EntitySpace()
        {
            this.Partitions = new ChildItemCollection<EntitySpace, Partition>(this);
        }

        /// <summary>
        /// Property types.
        /// </summary>
        public enum PropertyType
        {
            /// <summary>
            /// Primitive property type (for instance: string).
            /// </summary>
            [XmlEnum]
            Primitive = 0,

            /// <summary>
            /// Reference type.
            /// </summary>
            [XmlEnum]
            Reference = 1,

            /// <summary>
            /// Enumeration type.
            /// </summary>
            [XmlEnum]
            Enumeration = 2
        }

        /// <summary>
        /// Gets the list of partitions.
        /// </summary>
        public ChildItemCollection<EntitySpace, Partition> Partitions { get; private set; }

        /// <summary>
        /// Validates an xml stream against the EntitySpace schema definition.
        /// </summary>
        /// <param name="stream">The stream containing the xml to validate.</param>
        public static void Validate(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (
                var xsdStream =
                    typeof(EntitySpace).Assembly.GetManifestResourceStream(
                        "Center.Center.Common.T4Templating.EntitySpace.xsd"))
            {
                Validate(xsdStream, stream);
            }
        }

        /// <summary>
        /// Validates an xml file against the EntitySpace schema definition.
        /// </summary>
        /// <param name="path">The path to the xml file to validate.</param>
        public static void Validate(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            using (var xmlStream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                Validate(xmlStream);
            }
        }

        /// <summary>
        /// Loads an <see cref="EntitySpace"/> from the given stream according to the specified options (if specified).
        /// </summary>
        /// <param name="stream">The stream containing the xml to be deserialized.</param>
        /// <param name="options">The options for loading.</param>
        /// <returns>The deserialized <see cref="EntitySpace"/>.</returns>
        public static EntitySpace Load(Stream stream, EntitySpaceLoadOptions options = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var position = stream.Position;
            using (var xmlReader = XmlReader.Create(stream))
            {
                var xmlAttributeOverrides = new XmlAttributeOverrides();
                if (options != null)
                {
                    AddEntityProperties(options, xmlAttributeOverrides);
                    AddPartitions(options, xmlAttributeOverrides);
                    AddPartitionEntities(options, xmlAttributeOverrides);
                }

                var serializer = new XmlSerializer(typeof(EntitySpace), xmlAttributeOverrides);
                var content = (EntitySpace)serializer.Deserialize(xmlReader);
                stream.Seek(position, SeekOrigin.Begin);
                return content;
            }
        }

        /// <summary>
        /// Loads an <see cref="EntitySpace"/> from the given xml file according to the specified options (if
        /// specified).
        /// </summary>
        /// <param name="path">The path of the xml to be deserialized.</param>
        /// <param name="options">The options for loading.</param>
        /// <returns>The deserialized <see cref="EntitySpace"/>.</returns>
        public static EntitySpace Load(string path, EntitySpaceLoadOptions options = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentOutOfRangeException("path");
            }

            using (var xmlStream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                return EntitySpace.Load(xmlStream, options);
            }
        }

        /// <summary>
        /// Gets the type of the id property.
        /// </summary>
        /// <param name="partitionName">The name of the partition.</param>
        /// <param name="entityName">The name of the entity.</param>
        /// <returns>The type of the id property.</returns>
        public string GetIdPropertyType(string partitionName, string entityName)
        {
            return
                this.Partitions.Single(partition => partition.Name == partitionName)
                    .Entities.Single(entity => entity.Name == entityName)
                    .IdPropertyType;
        }

        private static void Validate(Stream xsdStream, Stream xmlStream)
        {
            var position = xmlStream.Position;
            var schemas = new XmlSchemaSet();
            schemas.Add(Xmlns, XmlReader.Create(xsdStream));

            var document = XDocument.Load(xmlStream);
            document.Validate(schemas, ValidationEventHandler);
            xmlStream.Seek(position, SeekOrigin.Begin);
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }

        private static void AddPartitions(EntitySpaceLoadOptions options, XmlAttributeOverrides xmlAttributeOverrides)
        {
            if (!options.PartitionsArrayItemTypes.Any())
            {
                return;
            }

            var attributes = new XmlAttributes
                                 {
                                     XmlArray =
                                         new XmlArrayAttribute
                                             {
                                                 ElementName = "Partitions",
                                                 Namespace = Xmlns
                                             }
                                 };
            attributes.XmlArrayItems.Add(new XmlArrayItemAttribute("Partition", typeof(Partition)));
            var xmlArrayItemAttributes =
                options.PartitionsArrayItemTypes.Select(
                    arrayItemTypes =>
                    new XmlArrayItemAttribute(arrayItemTypes.Name, arrayItemTypes.ItemType)
                        {
                            Namespace =
                                arrayItemTypes
                                .Namespace
                        });
            foreach (var xmlArrayItemAttribute in xmlArrayItemAttributes)
            {
                attributes.XmlArrayItems.Add(xmlArrayItemAttribute);
            }

            xmlAttributeOverrides.Add(typeof(EntitySpace), "Partitions", attributes);
        }

        private static void AddPartitionEntities(
            EntitySpaceLoadOptions options,
            XmlAttributeOverrides xmlAttributeOverrides)
        {
            if (!options.PartitionEntitiesArrayItemTypes.Any())
            {
                return;
            }

            var attributes = new XmlAttributes
                                 {
                                     XmlArray =
                                         new XmlArrayAttribute
                                             {
                                                 ElementName = "Entities",
                                                 Namespace = Xmlns
                                             }
                                 };
            attributes.XmlArrayItems.Add(new XmlArrayItemAttribute("Entity", typeof(Entity)));
            var xmlArrayItemAttributes =
                options.PartitionEntitiesArrayItemTypes.Select(
                    arrayItemTypes =>
                    new XmlArrayItemAttribute(arrayItemTypes.Name, arrayItemTypes.ItemType)
                        {
                            Namespace =
                                arrayItemTypes
                                .Namespace
                        });
            foreach (var xmlArrayItemAttribute in xmlArrayItemAttributes)
            {
                attributes.XmlArrayItems.Add(xmlArrayItemAttribute);
            }

            xmlAttributeOverrides.Add(typeof(Partition), "Entities", attributes);
        }

        private static void AddEntityProperties(
            EntitySpaceLoadOptions options,
            XmlAttributeOverrides xmlAttributeOverrides)
        {
            if (!options.EntityPropertiesArrayItemTypes.Any())
            {
                return;
            }

            var attributes = new XmlAttributes
                                 {
                                     XmlArray =
                                         new XmlArrayAttribute
                                             {
                                                 ElementName = "Properties",
                                                 Namespace = Xmlns
                                             }
                                 };
            attributes.XmlArrayItems.Add(new XmlArrayItemAttribute("Property", typeof(Property)));
            attributes.XmlArrayItems.Add(new XmlArrayItemAttribute("Collection", typeof(CollectionProperty)));
            var xmlArrayItemAttributes =
                options.EntityPropertiesArrayItemTypes.Select(
                    arrayItemTypes =>
                    new XmlArrayItemAttribute(arrayItemTypes.Name, arrayItemTypes.ItemType)
                        {
                            Namespace =
                                arrayItemTypes
                                .Namespace
                        });
            foreach (var xmlArrayItemAttribute in xmlArrayItemAttributes)
            {
                attributes.XmlArrayItems.Add(xmlArrayItemAttribute);
            }

            Action<Type> add = type => xmlAttributeOverrides.Add(type, "Properties", attributes);
            options.EntityTypes.Union(new[] { typeof(Entity) }).ToList().ForEach(add);
        }

        /// <summary>
        /// Defines a partition (group) in the entity space.
        /// </summary>
        public class Partition : IChildItem<EntitySpace>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Partition"/> class.
            /// </summary>
            public Partition()
            {
                this.Associations = new ChildItemCollection<Partition, Association>(this);
                this.Entities = new ChildItemCollection<Partition, Entity>(this);
                this.Enums = new ChildItemCollection<Partition, Enum>(this);
            }

            /// <summary>
            /// Gets or sets the name of the partition.
            /// </summary>
            [XmlAttribute("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets the parent partition.
            /// </summary>
            [XmlIgnore]
            public EntitySpace EntitySpace { get; internal set; }

            EntitySpace IChildItem<EntitySpace>.Parent
            {
                get
                {
                    return this.EntitySpace;
                }

                set
                {
                    this.EntitySpace = value;
                }
            }

            /// <summary>
            /// Gets the entities in the partition.
            /// </summary>
            public ChildItemCollection<Partition, Entity> Entities { get; private set; }

            /// <summary>
            /// Gets the list of <see cref="Enum"/>s in the partition.
            /// </summary>
            public ChildItemCollection<Partition, Enum> Enums { get; private set; }

            /// <summary>
            /// Gets the list of <see cref="Association"/>s in the partition.
            /// </summary>
            public ChildItemCollection<Partition, Association> Associations { get; private set; }
        }

        /// <summary>
        /// Defines an enumeration.
        /// </summary>
        public class Enum : IChildItem<Partition>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Enum"/> class.
            /// </summary>
            public Enum()
            {
                this.Members = new List<EnumMember>();
            }

            /// <summary>
            /// Gets the parent partition.
            /// </summary>
            [XmlIgnore]
            public Partition Partition { get; internal set; }

            Partition IChildItem<Partition>.Parent
            {
                get
                {
                    return this.Partition;
                }

                set
                {
                    this.Partition = value;
                }
            }

            /// <summary>
            /// Gets or sets the name of the enumeration type.
            /// </summary>
            [XmlAttribute("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the list of members of the enumeration.
            /// </summary>
            [XmlArrayItem("Member", typeof(EnumMember))]
            public List<EnumMember> Members { get; set; }
        }

        /// <summary>
        /// Defines an enumeration member.
        /// </summary>
        public class EnumMember
        {
            /// <summary>
            /// Gets or sets the name of the enumeration member.
            /// </summary>
            [XmlAttribute("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value of the enumeration member.
            /// </summary>
            [XmlAttribute("value")]
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets the description of the enumeration member.
            /// </summary>
            [XmlAttribute("description")]
            public string Description { get; set; }
        }

        /// <summary>
        /// Defines an entity
        /// </summary>
        public class Entity : IChildItem<Partition>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Entity"/> class.
            /// </summary>
            public Entity()
            {
                this.IdPropertyType = "int";
                this.HasChangeTracking = true;
                this.Properties = new ChildItemCollection<Entity, PropertyBase>(this);
            }

            /// <summary>
            /// Gets the parent partition.
            /// </summary>
            [XmlIgnore]
            public Partition Partition { get; internal set; }

            Partition IChildItem<Partition>.Parent
            {
                get
                {
                    return this.Partition;
                }

                set
                {
                    this.Partition = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this entity is abstract.
            /// </summary>
            [XmlAttribute("isAbstract")]
            public bool IsAbstract { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this entity has change tracking enabled.
            /// This is true by default.
            /// </summary>
            [XmlAttribute("hasChangeTracking")]
            public bool HasChangeTracking { get; set; }

            /// <summary>
            /// Gets or sets the base entity, if defined.
            /// </summary>
            /// <value>The base entity, if defined; <c>null</c> otherwise.</value>
            [XmlAttribute("baseEntity")]
            public string BaseEntity { get; set; }

            /// <summary>
            /// Gets or sets the name of the entity.
            /// </summary>
            [XmlAttribute("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the property to be used to represent this entity.
            /// </summary>
            [XmlAttribute("displayProperty")]
            public string DisplayProperty { get; set; }

            /// <summary>
            /// Gets or sets the list of properties for this entity.
            /// </summary>
            [XmlArrayItem("Property", typeof(Property))]
            [XmlArrayItem("Stream", typeof(StreamProperty))]
            [XmlArrayItem("Collection", typeof(CollectionProperty))]
            [XmlArray("Properties", Namespace = Xmlns)]
            public ChildItemCollection<Entity, PropertyBase> Properties { get; set; }

            /// <summary>
            /// Gets or sets the type of the Id property.
            /// </summary>
            /// <value>
            /// The type of the Id property. By default, it's set to <see cref="System.Int32"/> in the constructor.
            /// </value>
            [XmlAttribute("idPropertyType")]
            public string IdPropertyType { get; set; }
        }

        /// <summary>
        /// Defines an association.
        /// </summary>
        public class Association : IChildItem<Partition>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Association"/> class.
            /// </summary>
            public Association()
            {
                this.Ends = new ChildItemCollection<Association, AssociationEnd>(this);
            }

            /// <summary>
            /// Gets the association ends.
            /// </summary>
            [XmlArrayItem("End", typeof(AssociationEnd))]
            public ChildItemCollection<Association, AssociationEnd> Ends { get; private set; }

            /// <summary>
            /// Gets the parent partition.
            /// </summary>
            [XmlIgnore]
            public Partition Partition { get; internal set; }

            Partition IChildItem<Partition>.Parent
            {
                get
                {
                    return this.Partition;
                }

                set
                {
                    this.Partition = value;
                }
            }
        }

        /// <summary>
        /// Defines an association end.
        /// </summary>
        public class AssociationEnd : IChildItem<Association>
        {
            /// <summary>
            /// Gets the parent association.
            /// </summary>
            [XmlIgnore]
            public Association Association { get; internal set; }

            Association IChildItem<Association>.Parent
            {
                get
                {
                    return this.Association;
                }

                set
                {
                    this.Association = value;
                }
            }

            /// <summary>
            /// Gets or sets the type of the association end.
            /// </summary>
            [XmlAttribute("type")]
            public string Type { get; set; }
        }

        /// <summary>
        /// Base definition for properties.
        /// </summary>
        public abstract class PropertyBase : IChildItem<Entity>
        {
            /// <summary>
            /// Gets the parent entity.
            /// </summary>
            [XmlIgnore]
            public Entity Entity { get; internal set; }

            Entity IChildItem<Entity>.Parent
            {
                get
                {
                    return this.Entity;
                }

                set
                {
                    this.Entity = value;
                }
            }

            /// <summary>
            /// Gets or sets the name of the property.
            /// </summary>
            [XmlAttribute("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the value of the property is mandatory for the entity.
            /// </summary>
            [XmlAttribute("isRequired")]
            public bool IsRequired { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the value of the property is unique for the entity set.
            /// </summary>
            [XmlAttribute("isUnique")]
            public bool IsUnique { get; set; }

            /// <summary>
            /// Gets or sets the index name. It is used together with IsUnique and IndexOrder
            /// to create a multi column index.
            /// </summary>
            [XmlAttribute("indexName")]
            public string IndexName { get; set; }

            /// <summary>
            /// Gets or sets the index order. It is used together with IsUnique and IndexName
            /// to create a multi column index.
            /// </summary>
            [XmlAttribute("indexOrder")]
            public int IndexOrder { get; set; }
        }

        /// <summary>
        /// Defines a concrete property with a type.
        /// </summary>
        public class Property : PropertyBase
        {
            /// <summary>
            /// Gets or sets the type of the property.
            /// </summary>
            [XmlAttribute("type")]
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the property type.
            /// </summary>
            [XmlAttribute("propertyType")]
            public PropertyType PropertyType { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the property should be xml serialized.
            /// </summary>
            [XmlAttribute("isXmlSerialized")]
            public bool IsXmlSerialized { get; set; }
        }

        /// <summary>
        /// Defines a property whose content is a stream.
        /// </summary>
        public class StreamProperty : PropertyBase
        {
        }

        /// <summary>
        /// Defines a property as a collection of items.
        /// </summary>
        public class CollectionProperty : PropertyBase
        {
            /// <summary>
            /// Gets or sets the type of the items in the collection.
            /// </summary>
            [XmlAttribute("itemType")]
            public string ItemType { get; set; }

            /// <summary>
            /// Gets or sets the inverse property for a collection.
            /// </summary>
            [XmlAttribute("inverseProperty")]
            public string InverseProperty { get; set; }
        }

        /// <summary>
        /// Defines the options to control the loading process of the <see cref="EntitySpace"/>.
        /// </summary>
        public class EntitySpaceLoadOptions
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EntitySpaceLoadOptions"/> class.
            /// </summary>
            public EntitySpaceLoadOptions()
            {
                this.PartitionTypes = new List<Type>();
                this.EntityTypes = new List<Type>();
                this.EntityPropertiesArrayItemTypes = new List<ArrayItemTypes>();
                this.PartitionEntitiesArrayItemTypes = new List<ArrayItemTypes>();
                this.PartitionsArrayItemTypes = new List<ArrayItemTypes>();
            }

            /// <summary>
            /// Gets or sets the list of types to(de)serialize <see cref="EntitySpace.Partition"/>s.
            /// </summary>
            public ICollection<Type> PartitionTypes { get; set; }

            /// <summary>
            /// Gets or sets the list of types to(de)serialize <see cref="EntitySpace.Entity"/>s.
            /// </summary>
            public ICollection<Type> EntityTypes { get; set; }

            /// <summary>
            /// Gets the list of <see cref="ArrayItemTypes"/> to(de)serialize properties.
            /// </summary>
            public ICollection<ArrayItemTypes> EntityPropertiesArrayItemTypes { get; private set; }

            /// <summary>
            /// Gets the list of <see cref="ArrayItemTypes"/> to(de)serialize entities.
            /// </summary>
            public ICollection<ArrayItemTypes> PartitionEntitiesArrayItemTypes { get; private set; }

            /// <summary>
            /// Gets the list of <see cref="ArrayItemTypes"/> to(de)serialize partitions.
            /// </summary>
            public ICollection<ArrayItemTypes> PartitionsArrayItemTypes { get; private set; }

            /// <summary>
            /// Adds an <see cref="ArrayItemTypes"/> for properties.
            /// </summary>
            /// <param name="name">The name of the array item.</param>
            /// <param name="itemType">The type of the array item.</param>
            /// <param name="namespace">The optional namespace of the array item.</param>
            public void AddEntityPropertiesArrayItemType(string name, Type itemType, string @namespace = null)
            {
                this.EntityPropertiesArrayItemTypes.Add(
                    new ArrayItemTypes { ItemType = itemType, Name = name, Namespace = @namespace });
            }

            /// <summary>
            /// Adds an <see cref="ArrayItemTypes"/> for partitions.
            /// </summary>
            /// <param name="name">The name of the array item.</param>
            /// <param name="itemType">The type of the array item.</param>
            /// <param name="namespace">The optional namespace of the array item.</param>
            public void AddPartitionsArrayItemType(string name, Type itemType, string @namespace = null)
            {
                this.PartitionsArrayItemTypes.Add(
                    new ArrayItemTypes { ItemType = itemType, Name = name, Namespace = @namespace });
            }

            /// <summary>
            /// Adds an <see cref="ArrayItemTypes"/> for entities.
            /// </summary>
            /// <param name="name">The name of the array item.</param>
            /// <param name="itemType">The type of the array item.</param>
            /// <param name="namespace">The optional namespace of the array item.</param>
            public void AddPartitionEntitiesArrayItemType(string name, Type itemType, string @namespace = null)
            {
                this.PartitionEntitiesArrayItemTypes.Add(
                    new ArrayItemTypes { ItemType = itemType, Name = name, Namespace = @namespace });
            }

            /// <summary>
            /// Defines an array item type that will be used to generate the attribute needed for serialization.
            /// </summary>
            public class ArrayItemTypes
            {
                /// <summary>
                /// Gets or sets the name.
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the namespace.
                /// </summary>
                public string Namespace { get; set; }

                /// <summary>
                /// Gets or sets the type of the item.
                /// </summary>
                public Type ItemType { get; set; }
            }
        }
    }
}