// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolSectionConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Section
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The PoolSectionConfigDataViewModel.
    /// </summary>
    public partial class PoolSectionConfigDataViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The get media shell.
        /// </summary>
        /// <returns>
        /// The <see cref="IMediaShell"/>.
        /// </returns>
        public bool CheckPoolExists()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();

            if (applicationState.CurrentProject.InfomediaConfig.Pools.Contains(this.Pool))
            {
                return true;
            }

            return false;
        }

        private PoolConfigDataViewModel FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            foreach (var pool in applicationState.CurrentProject.InfomediaConfig.Pools)
            {
                if (pool.Name.Value == this.PoolName)
                {
                    return pool;
                }
            }

            Logger.Trace("Pool reference with name {0} not found in Pools.", this.PoolName);
            return null;
        }
    }
}
