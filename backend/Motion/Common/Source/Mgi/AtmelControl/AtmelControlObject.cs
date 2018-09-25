// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AtmelControlObject.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AtmelControlObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Base class for objects that can be used with <see cref="AtmelControlClient.RegisterObject{T}"/>.
    /// </summary>
    public abstract class AtmelControlObject
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtmelControlObject"/> class.
        /// </summary>
        internal AtmelControlObject()
        {
            this.name = this.GetType().Name;
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the object name.
        /// The object name has to be the name of this class otherwise an exception is thrown.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public string objectName
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    throw new ArgumentException(
                        string.Format("Wrong objectName, expected '{0}' but got '{1}'", this.name, value), "value");
                }
            }
        }

        // ReSharper restore InconsistentNaming
    }
}