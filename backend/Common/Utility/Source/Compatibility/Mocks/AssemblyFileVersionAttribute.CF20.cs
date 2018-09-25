// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyFileVersionAttribute.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Assembly information valid for the whole solution
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace System.Reflection
{
    using System;

    /// <summary>
    /// Assembly file version attribute which is not available in Compact Framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public partial class AssemblyFileVersionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFileVersionAttribute"/> class.
        /// </summary>
        /// <param name="version">
        /// The file version.
        /// </param>
        public AssemblyFileVersionAttribute(string version)
        {
            this.Version = version;
        }

        /// <summary>
        /// Gets the file version.
        /// </summary>
        public string Version { get; private set; }
    }
}

// ReSharper restore CheckNamespace