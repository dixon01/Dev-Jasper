// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentApplicationController.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComponentApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;

    using Gorba.Common.SystemManagement.Client;

    /// <summary>
    /// Application controller that handles a component (object implementing
    /// <see cref="IApplication"/>).
    /// </summary>
    public partial class ComponentApplicationController
    {
        private IApplication CreateInstanceInNewAppDomain()
        {
            throw new NotSupportedException("AppDomains are not supported in Compact Framework");
        }
    }
}