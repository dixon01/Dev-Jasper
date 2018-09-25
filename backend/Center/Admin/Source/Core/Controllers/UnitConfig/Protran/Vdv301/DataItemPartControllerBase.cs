// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using Gorba.Common.Configuration.Protran.VDV301;

    /// <summary>
    /// Base class for all part controllers that handle a data item configuration.
    /// </summary>
    public abstract class DataItemPartControllerBase : Vdv301PartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemPartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected DataItemPartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to should show the row editor.
        /// </summary>
        public abstract bool ShouldShowRow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to should show the language editor.
        /// </summary>
        public abstract bool ShouldShowLanguage { get; set; }

        /// <summary>
        /// Creates a <see cref="DataItemConfig"/> (or a subclass) for this data item.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="DataItemConfig"/>.
        /// </returns>
        public abstract DataItemConfig CreateConfig();
    }
}