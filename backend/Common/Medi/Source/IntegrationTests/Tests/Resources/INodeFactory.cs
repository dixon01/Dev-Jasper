// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the INodeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    using System.Collections.Generic;

    /// <summary>
    /// Factory interface for classes that create a virtual Medi network.
    /// </summary>
    internal interface INodeFactory
    {
        /// <summary>
        /// Creates all nodes, the first and last node will be used to send messages to each other.
        /// </summary>
        /// <returns>
        /// The list of nodes.
        /// </returns>
        List<MediNode> CreateNodes();
    }
}
