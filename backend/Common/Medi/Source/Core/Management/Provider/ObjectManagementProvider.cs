// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObjectManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System;
    using System.Reflection;

    /// <summary>
    /// A management provider for a simple object.
    /// It returns all properties of an object as its properties.
    /// </summary>
    public sealed class ObjectManagementProvider : ModifiableManagementObjectProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectManagementProvider"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="obj">
        /// The object to be represented by this provider.
        /// </param>
        /// <param name="properties">
        /// A list of properties to be used.
        /// </param>
        public ObjectManagementProvider(string name, IManagementProvider parent, object obj, params string[] properties)
            : base(name, parent)
        {
            var type = obj.GetType();
            foreach (var property in properties)
            {
                var propInfo = type.GetProperty(property);
                if (propInfo == null || !propInfo.CanRead)
                {
                    continue;
                }

                var propType = typeof(ObjectManagementProperty<>).MakeGenericType(propInfo.PropertyType);
                var ctor = propType.GetConstructor(new[] { typeof(PropertyInfo), typeof(object) });
                if (ctor == null)
                {
                    throw new TypeLoadException("Couldn't find constructor");
                }

                this.AddProperty((ManagementProperty)ctor.Invoke(new[] { propInfo, obj }));
            }
        }

        private class ObjectManagementProperty<T> : ManagementProperty<T>
        {
            private readonly PropertyInfo info;

            private readonly object parent;

            public ObjectManagementProperty(PropertyInfo info, object parent)
                : base(info.Name, (T)info.GetValue(parent, null), info.CanWrite && info.GetSetMethod().IsPublic)
            {
                this.info = info;
                this.parent = parent;
            }

            public override T Value
            {
                get
                {
                    return base.Value;
                }

                set
                {
                    this.info.SetValue(this.parent, value, null);
                    base.Value = value;
                }
            }
        }
    }
}
