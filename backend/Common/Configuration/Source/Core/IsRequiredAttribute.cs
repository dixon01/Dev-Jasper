// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsRequiredAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The RequiredAttribute class is a user-defined attribute class.
//   It can be applied to classes declarations only.
//   It takes one unnamed string argument (the author's name).
//   It has one optional named argument Version, which is of type int.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    using System;

    /// <summary>
    /// The RequiredAttribute class is a user-defined attribute class.
    /// It can be applied to classes declarations only.
    /// It takes one unnamed string argument (the author's name).
    /// It has one optional named argument Version, which is of type int.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IsRequiredAttribute : Attribute
    {
        private readonly bool isRequired;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsRequiredAttribute"/> class. 
        /// </summary>
        /// <param name="isRequired">
        /// Indicates whether the property is isRequired or not  
        /// </param>
        public IsRequiredAttribute(bool isRequired)
        {
            this.isRequired = isRequired;
        }

        /// <summary>
        /// Gets a value indicating whether the proerty is required or no.
        /// </summary>
        public bool IsRequired 
        {
            get
            {
                return this.isRequired;
            }
        }

        /// <summary>
        /// Returns a string with Required or Optional according to isRequired field.
        /// </summary>
        /// <returns>
        /// <b>Required</b> if the property is isRequired otherwise "Optional" 
        /// </returns>
        public override string ToString()
        {
            if (this.isRequired)
            {
                return "Required";
            }

            return "Optional";
        }
    }
}
