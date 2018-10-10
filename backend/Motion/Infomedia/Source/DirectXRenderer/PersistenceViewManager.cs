// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceViewManager.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The persistence view manager. This class stores a dictionary of
//   objects based on element IDs for persistence rendering of elements
//   when a section changes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer
{
    using System.Collections.Generic;

    /// <summary>
    /// The persistence view manager. This class stores a dictionary of
    /// objects based on element IDs for persistence rendering of elements
    /// when a section changes.
    /// </summary>
    public class PersistenceViewManager
    {
        private readonly IDictionary<int, object> storedObjects = new Dictionary<int, object>();

        /// <summary>
        /// This method returns null or a user stored object based on a unique ID.
        /// </summary>
        /// <param name="elementId">
        /// The element ID.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetObject(int elementId)
        {
            if (this.storedObjects.ContainsKey(elementId))
            {
                return this.storedObjects[elementId];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This methods updates a stored abject by it's associated ID.
        /// </summary>
        /// <param name="elementId">
        /// The element ID.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void UpdateObject(int elementId, object value)
        {
            if (this.storedObjects.ContainsKey(elementId))
            {
                this.storedObjects[elementId] = value;
            }
            else
            {
                this.storedObjects.Add(elementId, value);
            }
        }
    }
}
