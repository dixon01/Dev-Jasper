// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Engine;

    /// <summary>
    /// Factory / repository for type schemata. Use this class to create
    /// schemas, don't try to create them yourself.
    /// </summary>
    public class SchemaFactory
    {
        private static volatile SchemaFactory instance;

        private readonly Dictionary<TypeName, ITypeSchema> schemata = new Dictionary<TypeName, ITypeSchema>();

        private SchemaFactory()
        {
        }

        private enum SchemaType
        {
            Default,
            BuiltIn,
            List,
            Enum
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static SchemaFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof(SchemaFactory))
                    {
                        if (instance == null)
                        {
                            instance = new SchemaFactory();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Get the schema for a certain type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The schema for the given type. The result will be
        /// cached in case the same type is requested later.
        /// </returns>
        public ITypeSchema GetSchema(Type type)
        {
            return this.GetSchema(new TypeName(type), null, true);
        }

        /// <summary>
        /// Get the schema for a given object.
        /// This method also supports <see cref="IUnknown"/>, so it
        /// is safer to use this if you are not sure if the object
        /// you are handling is known or not.
        /// </summary>
        /// <param name="obj">
        /// The object for which you want to get the schema.
        /// </param>
        /// <returns>
        /// The schema for the given object. The result will be
        /// cached in case the same type is requested later.
        /// </returns>
        public ITypeSchema GetSchemaFor(object obj)
        {
            return this.GetSchema(TypeName.GetNameFor(obj), obj, true);
        }

        private ITypeSchema GetMemberSchema(TypeName type)
        {
            return this.GetSchema(type, null, false);
        }

        private ITypeSchema GetSchema(TypeName type, object obj, bool allowBecSchema)
        {
            lock (this.schemata)
            {
                ITypeSchema schema;
                if (this.schemata.TryGetValue(type, out schema))
                {
                    if (schema is BecSchema && !allowBecSchema)
                    {
                        return new DefaultTypeSchema();
                    }

                    return schema;
                }

                switch (this.GetSchemaType(type))
                {
                    case SchemaType.BuiltIn:
                        schema = new BuiltInTypeSchema { TypeName = type };
                        break;
                    case SchemaType.Enum:
                        schema = new EnumTypeSchema
                            {
                                TypeName = type,
                                UnderlyingSchema = new BuiltInTypeSchema
                                    {
                                        TypeName = new TypeName(type.Type.GetField("value__").FieldType)
                                    }
                            };
                        break;
                    case SchemaType.Default:
                        if (!allowBecSchema)
                        {
                            return new DefaultTypeSchema();
                        }

                        if (!type.IsKnown)
                        {
                            var unknown = obj as UnknownBecObject;
                            if (unknown == null)
                            {
                                throw new NotSupportedException("Can't determine structure of unknown type");
                            }

                            return unknown.Schema;
                        }

                        schema = this.CreateBecSchema(type.Type);
                        break;
                    case SchemaType.List:
                        schema = new ListTypeSchema
                            {
                                TypeName = type,
                                ItemSchema = this.GetMemberSchema(new TypeName(this.GetElementType(type.Type)))
                            };
                        break;
                    default:
                        throw new ArgumentException("Could not defer schema from type", "type");
                }

                this.schemata.Add(type, schema);
                return schema;
            }
        }

        private SchemaType GetSchemaType(TypeName typeName)
        {
            if (!typeName.IsKnown)
            {
                return SchemaType.Default;
            }

            var type = typeName.Type;

            if (type.IsPrimitive || type == typeof(string) || type == typeof(byte[])
                || type == typeof(Type) || type == typeof(Guid) || type == typeof(DateTime))
            {
                return SchemaType.BuiltIn;
            }

            if (type.IsEnum)
            {
                return SchemaType.Enum;
            }

            if (typeof(IList).IsAssignableFrom(type))
            {
                return SchemaType.List;
            }

            return SchemaType.Default;
        }

        private BecSchema CreateBecSchema(Type type)
        {
            var schema = new BecSchema(new TypeName(type));

            var members = SchemaMemberInfo.GetMembers(type);
            foreach (var member in members)
            {
                schema.Members.Add(
                    new SchemaMember { Name = member.Name, Schema = this.GetMemberSchema(member.MemberType) });
            }

            return schema;
        }

        private Type GetElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsGenericType)
            {
                for (var t = type; t != null; t = t.BaseType)
                {
                    foreach (var iface in t.GetInterfaces())
                    {
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            return iface.GetGenericArguments()[0];
                        }
                    }
                }
            }

            // we have no clue how to figure out the type;
            // perhaps we could look at the indexer or Add() ?
            // for now let's assume, we have objects
            return typeof(object);
        }
    }
}
