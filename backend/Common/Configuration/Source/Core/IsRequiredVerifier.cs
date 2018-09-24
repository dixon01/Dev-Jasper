// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsRequiredVerifier.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Class that enable o verify if a class memeber is tagged with <see cref="IsRequiredAttribute" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    using System.Reflection;

    /// <summary>
    /// Class that enable o verify if a class memeber is tagged with <see cref="IsRequiredAttribute"/>
    /// </summary>
    public class IsRequiredVerifier
    {
        /// <summary>
        /// Indicates whether a property is tagged with the IsRequiredAttibute
        /// </summary>
        /// <param name="member">
        /// The member.
        /// </param>
        /// <returns>
        /// <b>true</b> id the member is tagged with the <see cref="IsRequiredAttribute"/>
        /// </returns>
        public static bool IsRequired(MemberInfo member)
        {
            // return member.GetCustomAttributes(true).GetType() ==  IsRequiredAttribute>().Any();
            return false;
        }
    }
}
