// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaMemberInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaMemberInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Schema member (property or field) information.
    /// </summary>
    internal class SchemaMemberInfo : IComparable<SchemaMemberInfo>
    {
        private const char AttributePrefix = '@';

        private SchemaMemberInfo(string name, MemberInfo member, TypeName memberType)
        {
            this.Name = name;
            this.Member = member;
            this.MemberType = memberType;
        }

        /// <summary>
        /// Gets the name of the member.
        /// This name can differ from the <see cref="Member"/>'s name
        /// if a <see cref="XmlElementAttribute"/> or <see cref="XmlAttributeAttribute"/>
        /// was set on the member.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the member reflection information.
        /// This is either a <see cref="PropertyInfo"/> or a <see cref="FieldInfo"/>.
        /// </summary>
        public MemberInfo Member { get; private set; }

        /// <summary>
        /// Gets the type name of the member.
        /// </summary>
        public TypeName MemberType { get; private set; }

        /// <summary>
        /// Gets a list of all member information for a given type.
        /// Members with an <see cref="XmlIgnoreAttribute"/> will
        /// be ignored by this method.
        /// </summary>
        /// <param name="type">
        /// The type for which you want to get its members.
        /// </param>
        /// <returns>
        /// A list of all fields and properties.
        /// </returns>
        public static List<SchemaMemberInfo> GetMembers(Type type)
        {
            var members = new List<SchemaMemberInfo>();
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.CanRead && property.CanWrite && property.GetGetMethod().GetParameters().Length == 0)
                {
                    AddMember(members, property, property.PropertyType);
                }
            }

            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                AddMember(members, field, field.FieldType);
            }

            members.Sort();
            return members;
        }

        int IComparable<SchemaMemberInfo>.CompareTo(SchemaMemberInfo other)
        {
            return string.Compare(this.Name, other.Name, StringComparison.InvariantCulture);
        }

        private static void AddMember(List<SchemaMemberInfo> members, MemberInfo member, Type type)
        {
            var ignores = AttributeUtil.GetAttributes<XmlIgnoreAttribute>(member);
            if (ignores.Length > 0)
            {
                return;
            }

            var name = GetMemberName(member);
            if (members.Find(i => i.Name.Equals(name)) != null)
            {
                throw new DuplicateNameException(
                    string.Format("A member with the name '{0}' already exists in {1}", name, type.FullName));
            }

            members.Add(new SchemaMemberInfo(name, member, new TypeName(type)));
        }

        private static string GetMemberName(MemberInfo member)
        {
            var elementNames = AttributeUtil.GetAttributes<XmlElementAttribute>(member);
            if (elementNames.Length == 1)
            {
                var name = elementNames[0].ElementName;
                return string.IsNullOrEmpty(name) ? member.Name : name;
            }

            var attrNames = AttributeUtil.GetAttributes<XmlAttributeAttribute>(member);
            if (attrNames.Length == 1)
            {
                var name = attrNames[0].AttributeName;
                return AttributePrefix + (string.IsNullOrEmpty(name) ? member.Name : name);
            }

            return member.Name;
        }
    }
}
