// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RebootLimitActionConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RebootLimitActionConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;

    /// <summary>
    /// Action performed if a given limit is reached: reboot the system.
    /// </summary>
    [Serializable]
    public class RebootLimitActionConfig : LimitActionConfigBase
    {
    }
}