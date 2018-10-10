// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericLanguage.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Data
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Language specific container for the generic tables.
    /// </summary>
    public class GenericLanguage
    {
        #region Constants and Fields

        private readonly Dictionary<int, GenericTable> tables = new Dictionary<int, GenericTable>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the index of the language relative to the dictionary containing it.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the generic tables.
        /// </summary>
        public Dictionary<int, GenericTable> Tables
        {
            get
            {
                return this.tables;
            }
        }

        #endregion
    }
}