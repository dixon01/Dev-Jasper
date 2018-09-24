// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Motion.Update.UsbUpdateManager.Data;

    /// <summary>
    /// Project manager that allows to create and load <see cref="UpdateProject"/>.
    /// </summary>
    public interface IProjectManager
    {
        /// <summary>
        /// Event that is risen whenever <see cref="CurrentProject"/> changes.
        /// </summary>
        event EventHandler CurrentProjectChanged;

        /// <summary>
        /// Event that is risen before the project is saved in <see cref="Save"/>.
        /// </summary>
        event CancelEventHandler Saving;

        /// <summary>
        /// Event that is risen after the project was saved in <see cref="Save"/>.
        /// </summary>
        event EventHandler Saved;

        /// <summary>
        /// Gets the current project or null if none has been created/loaded yet.
        /// </summary>
        UpdateProject CurrentProject { get; }

        /// <summary>
        /// Gets a value indicating whether the current project is read-only.
        /// </summary>
        bool CurrentProjectReadOnly { get; }

        /// <summary>
        /// Gets the resource service for the current project.
        /// </summary>
        IResourceService CurrentResourceService { get; }

        /// <summary>
        /// Creates a new <see cref="UpdateProject"/> at the given location.
        /// </summary>
        /// <param name="filename">
        /// The filename of the file to create.
        /// </param>
        void Create(string filename);

        /// <summary>
        /// Loads <see cref="UpdateProject"/> from the given location.
        /// </summary>
        /// <param name="filename">
        /// The filename of the file to load.
        /// </param>
        void Load(string filename);

        /// <summary>
        /// Saves the current project.
        /// This method does nothing if <see cref="CurrentProject"/> is null.
        /// </summary>
        /// <returns>
        /// True if the project was saved.
        /// </returns>
        bool Save();

        /// <summary>
        /// Saves a stripped-down copy of the current project to the given file.
        /// This will only retain all unit groups and units with their
        /// current directory structures without updates, feedback and log files.
        /// </summary>
        /// <param name="filename">
        /// The filename of the project file to create.
        /// </param>
        /// <returns>
        /// True if the copy was saved successfully.
        /// </returns>
        bool SaveAs(string filename);

        /// <summary>
        /// Creates a new <see cref="UnitGroup"/> with the given name and
        /// all necessary default values.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="UnitGroup"/>.
        /// </returns>
        UnitGroup CreateUnitGroup(string name);

        /// <summary>
        /// Creates an export preview from the current state of the <see cref="CurrentProject"/>.
        /// </summary>
        /// <returns>
        /// The new <see cref="UpdateExportPreview"/>. You can add pre- and post-
        /// installation commands as well as resources to it.
        /// </returns>
        UpdateExportPreview CreateExportPreview();

        /// <summary>
        /// Creates an export of the given <see cref="UpdateExportPreview"/>.
        /// The export contains all resources and commands needed to update
        /// all units in the project to match the current project structure.
        /// Pre- and post-installation commands are added from the preview as well.
        /// </summary>
        /// <param name="preview">
        /// The preview created with <see cref="CreateExportPreview"/>.
        /// </param>
        /// <param name="name">
        /// The name of the update.
        /// </param>
        /// <param name="validFromDateTime">
        /// The date and time from which the update becomes valid
        /// </param>
        /// <param name="installAfterBoot">
        /// The installAfterBoot flag indicate to install the Update command after application restart if true
        /// </param>
        /// <returns>
        /// The <see cref="UpdateExport"/>.
        /// </returns>
        UpdateExport CreateExport(
            UpdateExportPreview preview, string name, DateTime validFromDateTime, bool installAfterBoot);

        /// <summary>
        /// Sets the state of all updates in the current project for an export.
        /// This method will also save the current project.
        /// </summary>
        /// <param name="export">
        /// The export.
        /// </param>
        /// <param name="state">
        /// The state to set.
        /// </param>
        void SetExportState(UpdateExport export, UpdateState state);

        /// <summary>
        /// Imports a log file received from an <see cref="IUpdateProvider"/> to the current project.
        /// </summary>
        /// <param name="logFile">
        /// The log file.
        /// </param>
        void ImportLogFile(IReceivedLogFile logFile);

        /// <summary>
        /// Gets all stored log files for a given unit.
        /// </summary>
        /// <param name="unit">
        /// The unit for which to get the log files.
        /// </param>
        /// <returns>
        /// The list of full paths to the log files.
        /// </returns>
        string[] GetLogFilesFor(Unit unit);
    }
}