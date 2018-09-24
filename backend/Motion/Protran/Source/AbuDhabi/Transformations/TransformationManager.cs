// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Transformations
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Manager for data item transformations.
    /// </summary>
    public class TransformationManager
    {
        private readonly Dictionary<string, TransformationChain> chains = new Dictionary<string, TransformationChain>();

        /// <summary>
        /// Configures this class with the given list of <see cref="Chain"/> configurations.
        /// </summary>
        /// <param name="configs">
        /// The config.
        /// </param>
        public void Configure(ICollection<Chain> configs)
        {
            foreach (var config in configs)
            {
                this.chains.Add(config.Id, this.CreateChain(config, configs));
            }
        }

        /// <summary>
        /// Get the <see cref="TransformationChain"/> for a given ID or
        /// null if no chain can be found.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// the chain matching the ID or null if no chain can be found.
        /// </returns>
        public TransformationChain GetChain(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            TransformationChain chain;
            this.chains.TryGetValue(id, out chain);
            return chain;
        }

        /// <summary>
        /// Creates a chain from the given list.
        /// </summary>
        /// <param name="chain">
        /// The chain configuration.
        /// </param>
        /// <param name="allChains">
        /// All chain configurations known to this application.
        /// </param>
        /// <returns>
        /// a new chain.
        /// </returns>
        protected virtual TransformationChain CreateChain(Chain chain, ICollection<Chain> allChains)
        {
            return new TransformationChain(chain.ResolveReferences(allChains));
        }
    }
}