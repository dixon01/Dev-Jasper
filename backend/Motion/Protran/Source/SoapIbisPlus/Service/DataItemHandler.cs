// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Core.Utils;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;

    /// <summary>
    /// The handler for <see cref="DataItemConfig"/>.
    /// </summary>
    internal class DataItemHandler
    {
        private readonly TransformationChain chain;

        private readonly GenericUsageHandler usageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemHandler"/> class.
        /// </summary>
        /// <param name="chain">
        /// The transformation chain.
        /// </param>
        /// <param name="usedFor">
        /// The generic usage configuration.
        /// </param>
        /// <param name="dictionary">
        /// The generic dictionary.
        /// </param>
        public DataItemHandler(TransformationChain chain, GenericUsage usedFor, Dictionary dictionary)
        {
            this.chain = chain;
            this.usageHandler = new GenericUsageHandler(usedFor, dictionary);
        }

        /// <summary>
        /// Adds a <see cref="XimpleCell"/> to the given <see cref="Ximple"/> object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple object to which the cell should be added.
        /// </param>
        /// <param name="value">
        /// The time value (Unix timestamp).
        /// </param>
        /// <param name="rowIndex">
        /// The row index (default is 0).
        /// </param>
        /// <returns>
        /// The the newly created <see cref="XimpleCell"/> or null if none was created.
        /// </returns>
        public XimpleCell AddCell(Ximple ximple, int value, int rowIndex = 0)
        {
            return this.AddCell(ximple, value.ToString(CultureInfo.InvariantCulture), rowIndex);
        }

        /// <summary>
        /// Adds a <see cref="XimpleCell"/> to the given <see cref="Ximple"/> object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple object to which the cell should be added.
        /// </param>
        /// <param name="value">
        /// The time value (Unix timestamp).
        /// </param>
        /// <param name="rowIndex">
        /// The row index (default is 0).
        /// </param>
        /// <returns>
        /// The the newly created <see cref="XimpleCell"/> or null if none was created.
        /// </returns>
        public XimpleCell AddCell(Ximple ximple, string value, int rowIndex = 0)
        {
            if (this.chain == null)
            {
                return null;
            }

            var transformed = this.chain.Transform(value);
            return this.usageHandler.AddCell(ximple, transformed, rowIndex);
        }
    }
}