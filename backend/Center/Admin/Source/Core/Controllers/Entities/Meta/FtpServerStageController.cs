// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpServerStageController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpServerStageController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Meta
{
    using System.Windows;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Meta;
    using Gorba.Center.Admin.Core.ViewModels.Stages.Meta;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Settings;

    /// <summary>
    /// The stage controller for FTP servers from the <see cref="BackgroundSystemSettings"/>.
    /// </summary>
    public class FtpServerStageController : EntityStageControllerBase
    {
        private readonly FtpServerStageViewModel stage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServerStageController"/> class.
        /// </summary>
        /// <param name="dataController">
        /// The system configuration data controller.
        /// </param>
        public FtpServerStageController(SystemConfigDataController dataController)
            : base(new FtpServerDataController(dataController))
        {
            this.Name = "FtpServer";
            this.PartitionName = "Meta";
            this.StageViewModel = this.stage = new FtpServerStageViewModel(dataController.Factory.CommandRegistry);
        }

        /// <summary>
        /// Checks if this controller supports the given entity instance.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model of the entity instance.
        /// </param>
        /// <returns>
        /// True if this controller supports the given entity.
        /// </returns>
        public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            return dataViewModel is FtpServerReadOnlyDataViewModel;
        }

        /// <summary>
        /// Checks if this controller supports the given entity instance.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model of the entity instance.
        /// </param>
        /// <returns>
        /// True if this controller supports the given entity.
        /// </returns>
        public override bool SupportsEntity(DataViewModelBase dataViewModel)
        {
            return dataViewModel is FtpServerDataViewModel;
        }

        /// <summary>
        /// Loads the entity instances from the given connection controller.
        /// </summary>
        public override void LoadData()
        {
            if (this.stage.FtpServers == null)
            {
                this.stage.FtpServers = ((FtpServerDataController)this.DataController).All;
            }
        }

        /// <summary>
        /// Gets the initial column visibility if the user hasn't chosen the visibility yet.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="Visibility"/>.
        /// The following return values have the given meaning:
        /// <see cref="Visibility.Visible"/>: the column is visible and can be hidden by the user.
        /// <see cref="Visibility.Collapsed"/>: the column is hidden and can be shown by the user.
        /// <see cref="Visibility.Hidden"/>: the column is completely removed and can't be selected by the user.
        /// </returns>
        protected override Visibility GetInitialColumnVisibility(string columnName)
        {
            if (columnName == "Password")
            {
                // we never want to show the password
                return Visibility.Hidden;
            }

            if (columnName == "Id")
            {
                // the ID column is only used internally and might change when removing entries, so let's hide it too
                return Visibility.Hidden;
            }

            return base.GetInitialColumnVisibility(columnName);
        }

        /// <summary>
        /// Updates the permissions of the <see cref="EntityStageControllerBase.StageViewModel"/>
        /// with the permission controller.
        /// </summary>
        protected override void UpdatePermissions()
        {
            this.UpdatePermissions(DataScope.Meta);
        }
    }
}
