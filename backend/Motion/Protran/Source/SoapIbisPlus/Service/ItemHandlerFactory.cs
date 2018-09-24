// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;

    using NLog;

    /// <summary>
    /// The factory for <see cref="DataItemHandler"/> and <see cref="TimeItemHandler"/>.
    /// </summary>
    internal class ItemHandlerFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly DataItemHandler NullHandler = new DataItemHandler(null, null, null);

        private readonly IDictionary<string, TransformationChain> transformationChains;

        private readonly Dictionary dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemHandlerFactory"/> class.
        /// </summary>
        /// <param name="transformationChains">
        /// The transformation chains.
        /// </param>
        /// <param name="dictionary">
        /// The generic dictionary.
        /// </param>
        public ItemHandlerFactory(
            IDictionary<string, TransformationChain> transformationChains, Dictionary dictionary)
        {
            this.transformationChains = transformationChains;
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Creates a new <see cref="DataItemHandler"/> for the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="DataItemHandler"/>, never null.
        /// </returns>
        public DataItemHandler CreateHandler(DataItemConfig config)
        {
            if (config == null || !config.Enabled || config.UsedFor == null)
            {
                return NullHandler;
            }

            TransformationChain chain;
            if (string.IsNullOrEmpty(config.TransfRef)
                || !this.transformationChains.TryGetValue(config.TransfRef, out chain))
            {
                Logger.Warn("Couldn't find transformation chain '{0}'", config.TransfRef);
                return NullHandler;
            }

            return new DataItemHandler(chain, config.UsedFor, this.dictionary);
        }

        /// <summary>
        /// Creates a new <see cref="TimeItemHandler"/> for the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="TimeItemHandler"/>, never null.
        /// </returns>
        public TimeItemHandler CreateHandler(TimeItemConfig config)
        {
            return new TimeItemHandler(config, this.dictionary);
        }
    }
}