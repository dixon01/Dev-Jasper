// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    /// <summary>
    /// The way icenter.diag can connect to the Unit.
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        /// The Unit is not available and therefore it is not possible to connect to it.
        /// </summary>
        NotAvailable,

        /// <summary>
        /// The Unit is available locally (either through UDCP or direct IP address entry).
        /// </summary>
        Local,

        /// <summary>
        /// The Unit is connected to the Background System and icenter.diag can connect through that.
        /// </summary>
        BackgroundSystem
    }
}