// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectControllerContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// Grants access to methods of the project controller which are used by the internal state machine
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.ProjectStates
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// The reload project result.
    /// </summary>
    public enum ReloadProjectResult
    {
        /// <summary>
        /// The fail.
        /// </summary>
        Fail,

        /// <summary>
        /// The from local.
        /// </summary>
        FromLocal,

        /// <summary>
        /// The from server.
        /// </summary>
        FromServer
    }

    /// <summary>
    /// The ProjectControllerContext interface.
    /// </summary>
    public interface IProjectControllerContext : IProjectController
    {
        /// <summary>
        /// Gets the current state.
        /// </summary>
        IState CurrentState { get; }

        /// <summary>
        /// The reset project.
        /// </summary>
        /// <param name="mediaProjectDataViewModel">
        /// The media project data view model.
        /// </param>
        /// <param name="resetCurrentProjectObject">
        /// The reset current project object.
        /// </param>
        void ResetProject(MediaProjectDataViewModel mediaProjectDataViewModel, bool resetCurrentProjectObject = true);

        /// <summary>
        /// The update recent projects.
        /// </summary>
        /// <param name="isCheckedIn">
        /// The is checked in.
        /// </param>
        /// <param name="saveThumbnail">
        /// The save thumbnail.
        /// </param>
        void UpdateRecentProjects(bool isCheckedIn = true, bool saveThumbnail = true);

        /// <summary>
        /// The notify wrapper to access IShellController extension method.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        void NotifyWrapper(Notification notification);

        /// <summary>
        /// The execute open project. All checks should be done.
        /// </summary>
        /// <param name="mediaConfiguration">
        /// The media Configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> ExecuteOpenProject(MediaConfigurationDataViewModel mediaConfiguration);

        /// <summary>
        /// The create document async.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> CreateDocumentAsync(MediaProjectDataModel model, TenantReadableModel tenant = null);

        /// <summary>
        /// The check in project async.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> CheckInProjectAsync(CreateDocumentVersionParameters parameters);

        /// <summary>
        /// The execute save project local.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ExecuteSaveProjectLocal();

        /// <summary>
        /// The reload project. If project is not found locally the project is loaded from the server.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        Task<ReloadProjectResult> ReloadProjectAsync();
    }
}
