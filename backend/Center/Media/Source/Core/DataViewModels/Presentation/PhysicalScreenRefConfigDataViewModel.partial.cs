// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalScreenRefConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The PhysicalScreenRefConfigDataViewModel.
    /// </summary>
    public partial class PhysicalScreenRefConfigDataViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private PhysicalScreenConfigDataViewModel FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            foreach (var physicalScreen in applicationState.CurrentProject.InfomediaConfig.PhysicalScreens)
            {
                if (physicalScreen.Name.Value == this.ReferenceName)
                {
                    return physicalScreen;
                }
            }

            Logger.Trace("PhysicalScreen reference with name {0} not found in PhysicalScreens.", this.ReferenceName);
            return null;
        }
    }
}
