// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediUpdateClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediUpdateClientConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Clients
{
    using System;

    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the MediUpdateClient.
    /// </summary>
    [Serializable]
    [Implementation("Gorba.Common.Update.Medi.MediUpdateClient, Gorba.Common.Update.Medi")]
    public class MediUpdateClientConfig : UpdateClientConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediUpdateClientConfig"/> class.
        /// </summary>
        public MediUpdateClientConfig()
        {
            this.Name = "MediClient";
        }
    }
}
