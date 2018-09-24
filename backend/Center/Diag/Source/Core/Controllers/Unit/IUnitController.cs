// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUnitController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Unit
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Diag.Core.Controllers.App;
    using Gorba.Center.Diag.Core.ViewModels.FileSystem;
    using Gorba.Center.Diag.Core.ViewModels.Unit;

    /// <summary>
    /// Interface to be implemented by all unit controllers.
    /// </summary>
    public interface IUnitController : IDisposable
    {
        /// <summary>
        /// Gets the view model for the unit controlled by this controller.
        /// </summary>
        UnitViewModelBase ViewModel { get; }

        /// <summary>
        /// Gets the list of application controllers responsible for applications of this unit.
        /// </summary>
        List<RemoteAppController> ApplicationControllers { get; }

        /// <summary>
        /// Connects to the unit.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the unit.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Checks if the given file can be opened (i.e. there is an application assigned to that extension).
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// True if the file can be opened.
        /// </returns>
        bool CanOpenRemoteFile(FileViewModel file);

        /// <summary>
        /// Downloads and then opens a remote file with the default application.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        void OpenRemoteFile(FileViewModel file);

        /// <summary>
        /// Downloads a remote file, asking the user for the place to save.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        void DownloadRemoteFile(FileViewModel file);

        /// <summary>
        /// Cancels a file download, started either through <see cref="OpenRemoteFile"/>
        /// or <see cref="DownloadRemoteFile"/>.
        /// </summary>
        void CancelRemoteFileDownload();
    }
}