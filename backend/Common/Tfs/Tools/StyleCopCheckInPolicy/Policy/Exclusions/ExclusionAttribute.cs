//--------------------------------------------------------------------------
// <copyright file="ExclusionAttribute.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      Microsoft Public License (Ms-PL) which can be found in the License.rtf 
//      at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions
{
    using System;

    /// <summary>
    /// Specifies a description for a type based on the string resource specified. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal sealed class ExclusionAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExclusionAttribute"/> class.
        /// </summary>
        /// <param name="exclusionType">The type of exclusion.</param>
        /// <param name="nameResourceId">The resource identifier to use for the exclusion name.</param>
        /// <param name="descriptionResourceId">The resource identifier to use for the description.</param>
        /// <param name="editorType">The type of editor for the exclusion.</param>
        public ExclusionAttribute(Type exclusionType, string nameResourceId, string descriptionResourceId, Type editorType)
        {
            this.ExclusionType = exclusionType;
            this.NameResourceId = nameResourceId;
            this.DescriptionResourceId = descriptionResourceId;
            this.EditorType = editorType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the exclusion type.
        /// </summary>
        public Type ExclusionType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name resource identifier.
        /// </summary>
        public string NameResourceId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description resource identifier.
        /// </summary>
        public string DescriptionResourceId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the editor type for the exclusion.
        /// </summary>
        public Type EditorType
        {
            get;
            private set;
        }

        #endregion
    }
}