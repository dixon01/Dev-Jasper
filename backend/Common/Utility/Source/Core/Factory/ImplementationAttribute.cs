// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementationAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImplementationAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Factory
{
    using System;

    /// <summary>
    /// Attribute to tell which type implements a given config.
    /// </summary>
    public class ImplementationAttribute : Attribute
    {
        private readonly string typeName;

        private Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementationAttribute"/> class
        /// using the type name.
        /// </summary>
        /// <param name="typeName">
        /// The implementation type name.
        /// </param>
        public ImplementationAttribute(string typeName)
        {
            this.typeName = typeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementationAttribute"/> class.
        /// </summary>
        /// <param name="type">
        /// The implementation type.
        /// </param>
        public ImplementationAttribute(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the implementation type.
        /// </summary>
        public Type Type
        {
            get
            {
                if (this.type != null)
                {
                    return this.type;
                }

                this.type = Type.GetType(this.typeName);
                if (this.type == null)
                {
                    // if the type was not found, try finding it with our suffix (e.g. ".CF35")
                    var myAsmName = this.GetType().Assembly.GetName().Name;
                    var parts = myAsmName.Split('.');
                    this.type = Type.GetType(this.typeName + "." + parts[parts.Length - 1]);
                }

                return this.type;
            }

            set
            {
                this.type = value;
            }
        }
    }
}
