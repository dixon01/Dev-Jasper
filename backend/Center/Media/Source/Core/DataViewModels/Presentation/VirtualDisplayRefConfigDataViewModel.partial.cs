// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualDisplayRefConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The virtual display reference config data view model.
    /// </summary>
    public partial class VirtualDisplayRefConfigDataViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private VirtualDisplayConfigDataViewModel FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();

            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            foreach (var virtualDisplay in applicationState.CurrentProject.InfomediaConfig.VirtualDisplays)
            {
                if (virtualDisplay.Name.Value == this.ReferenceName)
                {
                    return virtualDisplay;
                }
            }

            Logger.Trace("VirtualDisplay reference with name {0} not found in VirtualDisplays.", this.ReferenceName);
            return null;
        }
    }
}
