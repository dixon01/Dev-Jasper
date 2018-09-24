// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualDisplayConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using System.Windows;

    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The virtual display config data view model.
    /// </summary>
    public partial class VirtualDisplayConfigDataViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private double currentZoomLevel;

        private Point currentLayoutPosition;

        /// <summary>
        /// Gets or sets the current zoom level.
        /// </summary>
        public double CurrentZoomLevel
        {
            get
            {
                return this.currentZoomLevel;
            }

            set
            {
                this.SetProperty(ref this.currentZoomLevel, value, () => this.CurrentZoomLevel);
            }
        }

        /// <summary>
        /// Gets or sets the current layout position.
        /// </summary>
        public Point CurrentLayoutPosition
        {
            get
            {
                return this.currentLayoutPosition;
            }

            set
            {
                this.SetProperty(ref this.currentLayoutPosition, value, () => this.CurrentLayoutPosition);
            }
        }

        private CyclePackageConfigDataViewModel FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            foreach (var cyclePackage in applicationState.CurrentProject.InfomediaConfig.CyclePackages)
            {
                if (cyclePackage.Name.Value == this.CyclePackageName)
                {
                    return cyclePackage;
                }
            }

            Logger.Trace("CyclePackage reference with name {0} not found in CyclePackages.", this.CyclePackageName);
            return null;
        }

        partial void Initialize(Models.Presentation.VirtualDisplayConfigDataModel dataModel)
        {
            if (dataModel != null)
            {
                this.CurrentZoomLevel = dataModel.CurrentZoomLevel;
                this.CurrentLayoutPosition = dataModel.CurrentLayoutPosition;
            }
        }

        partial void Initialize(VirtualDisplayConfigDataViewModel dataViewModel)
        {
            this.CurrentZoomLevel = dataViewModel.CurrentZoomLevel;
            this.CurrentLayoutPosition = dataViewModel.CurrentLayoutPosition;
        }

        partial void ConvertNotGeneratedToDataModel(ref Models.Presentation.VirtualDisplayConfigDataModel dataModel)
        {
            dataModel.CurrentZoomLevel = this.CurrentZoomLevel;
            dataModel.CurrentLayoutPosition = this.CurrentLayoutPosition;
        }
    }
}
