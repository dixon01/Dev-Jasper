// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VDV453Message.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VDV453Message type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract class VDV453Message is the base class for all VDV453 messages.
    /// </summary>
    public abstract class Vdv453Message : ICloneable
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Clones a member property.
        /// </summary>
        /// <param name="original">
        /// The original to be cloned.
        /// </param>
        /// <typeparam name="T">
        /// The type of the property to be cloned
        /// </typeparam>
        /// <returns>
        /// the cloned member or null if the original was null.
        /// </returns>
        protected static T CloneMember<T>(T original)
            where T : class, ICloneable
        {
            return original == null ? null : (T)original.Clone();
        }

        /// <summary>
        /// Clones a member property that is a list.
        /// </summary>
        /// <param name="original">
        /// The original to be cloned.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items of the property to be cloned
        /// </typeparam>
        /// <returns>
        /// a new list with all items cloned or null if the original list was null.
        /// </returns>
        protected static List<T> CloneListMember<T>(List<T> original)
            where T : class, ICloneable
        {
            return original == null ? null : original.ConvertAll(item => (T)item.Clone());
        }
    }
}