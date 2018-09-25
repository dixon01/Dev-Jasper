// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVdv301ServiceImpl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IVdv301ServiceImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP
{
    using Gorba.Common.Protocols.Vdv301.Services;

    /// <summary>
    /// Interface to be implemented by all actual service implementations.
    /// This interface is used to determine if we have a proxy or an actual service.
    /// </summary>
    public interface IVdv301ServiceImpl : IVdv301Service
    {
    }
}