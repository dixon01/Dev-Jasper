// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectManager type.
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
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Update.UsbUpdateManager.Data;

    /// <summary>
    /// Project manager implementation.
    /// </summary>
    public class ProjectManager : IProjectManager
    {
        private ProjectHandler currentHandler;

        /// <summary>
        /// Event that is risen whenever <see cref="CurrentProject"/> changes.
        /// </summary>
        public event EventHandler CurrentProjectChanged;

        /// <summary>
        /// Event that is risen before the project is saved in <see cref="IProjectManager.Save"/>.
        /// </summary>
        public event CancelEventHandler Saving;

        /// <summary>
        /// Event that is risen after the project was saved in <see cref="IProjectManager.Save"/>.
        /// </summary>
        public event EventHandler Saved;

        /// <summary>
        /// Gets the current project or null if none has been created/loaded yet.
        /// </summary>
        public UpdateProject CurrentProject
        {
            get
            {
                return this.CurrentHandler == null ? null : this.currentHandler.Config;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current project is read-only.
        /// </summary>
        public bool CurrentProjectReadOnly
        {
            get
            {
                return this.CurrentHandler == null || this.CurrentHandler.ReadOnly;
            }
        }

        /// <summary>
        /// Gets the resource service for the current project.
        /// </summary>
        public IResourceService CurrentResourceService
        {
            get
            {
                return this.CurrentHandler == null ? null : this.currentHandler.ResourceService;
            }
        }

        private ProjectHandler CurrentHandler
        {
            get
            {
                return this.currentHandler;
            }

            set
            {
                if (this.currentHandler == value)
                {
                    return;
                }

                this.currentHandler = value;
                this.RaiseCurrentProjectChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Creates a new <see cref="UpdateProject"/> at the given location.
        /// </summary>
        /// <param name="filename">
        /// The filename of the file to create.
        /// </param>
        public void Create(string filename)
        {
            this.CurrentHandler = ProjectHandler.Create(filename);
        }

        /// <summary>
        /// Loads <see cref="UpdateProject"/> from the given location.
        /// </summary>
        /// <param name="filename">
        /// The filename of the file to load.
        /// </param>
        public void Load(string filename)
        {
            this.CurrentHandler = ProjectHandler.Load(filename);
        }

        /// <summary>
        /// Saves the current project.
        /// This method does nothing if <see cref="CurrentProject"/> is null.
        /// </summary>
        /// <returns>
        /// True if the project was saved.
        /// </returns>
        public bool Save()
        {
            if (this.CurrentHandler == null)
            {
                return false;
            }

            var cancel = new CancelEventArgs(false);
            this.RaiseSaving(cancel);
            if (cancel.Cancel)
            {
                return false;
            }

            this.CurrentHandler.Save();
            this.RaiseSaved(EventArgs.Empty);
            return true;
        }

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
        public bool SaveAs(string filename)
        {
            if (this.CurrentHandler == null)
            {
                return false;
            }

            this.CurrentHandler.SaveAs(filename);
            return true;
        }

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
        public UnitGroup CreateUnitGroup(string name)
        {
            var unitGroup = new UnitGroup
                {
                    Name = name,
                    CurrentDirectoryStructure = new DirectoryNode
                        {
                            Directories =
                                {
                                    new DirectoryNode
                                        {
                                            Name = "Config",
                                            Modifiable = false
                                        },
                                    new DirectoryNode
                                        {
                                            Name = "Progs",
                                            Modifiable = false
                                        },
                                    new DirectoryNode
                                        {
                                            Name = "Presentation",
                                            Modifiable = false
                                        }
                                }
                        }
                };
            return unitGroup;
        }

        /// <summary>
        /// Creates an export preview from the current state of the <see cref="IProjectManager.CurrentProject"/>.
        /// </summary>
        /// <returns>
        /// The new <see cref="UpdateExportPreview"/>. You can add pre- and post-
        /// installation commands as well as resources to it.
        /// </returns>
        public UpdateExportPreview CreateExportPreview()
        {
            return this.CurrentHandler.CreateExportPreview();
        }

        /// <summary>
        /// Creates an export of the given <see cref="UpdateExportPreview"/>.
        /// The export contains all resources and commands needed to update
        /// all units in the project to match the current project structure.
        /// Pre- and post-installation commands are added from the preview as well.
        /// </summary>
        /// <param name="preview">
        /// The preview created with <see cref="IProjectManager.CreateExportPreview"/>.
        /// </param>
        /// <param name="name">
        /// The name of the update.
        /// </param>
        /// <param name="validFromDateTime">
        /// The date and time in UTC from which the update becomes valid
        /// </param>
        /// <param name="installAfterBoot">
        /// The installAfterBoot flag indicate to install the Update command after application restart if true
        /// </param>
        /// <returns>
        /// The <see cref="UpdateExport"/>.
        /// </returns>
        public UpdateExport CreateExport(
            UpdateExportPreview preview, string name, DateTime validFromDateTime, bool installAfterBoot)
        {
            var export = this.CurrentHandler.CreateExport(preview, name, validFromDateTime, installAfterBoot);

            this.Save();
            return export;
        }

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
        public void SetExportState(UpdateExport export, UpdateState state)
        {
            var now = TimeProvider.Current.UtcNow;
            foreach (var unitGroup in this.CurrentProject.UnitGroups)
            {
                foreach (var unit in unitGroup.Units)
                {
                    var updates = unit.Updates.FindAll(u => export.Commands.Contains(u.Command));
                    foreach (var update in updates)
                    {
                        update.States.Add(new UpdateStateInfo { State = state, TimeStamp = now });
                    }
                }
            }

            this.Save();
        }

        /// <summary>
        /// Imports a log file received from an <see cref="IUpdateProvider"/> to the current project.
        /// </summary>
        /// <param name="logFile">
        /// The log file.
        /// </param>
        public void ImportLogFile(IReceivedLogFile logFile)
        {
            this.CurrentHandler.ImportLogFile(logFile);
        }

        /// <summary>
        /// Gets all stored log files for a given unit.
        /// </summary>
        /// <param name="unit">
        /// The unit for which to get the log files.
        /// </param>
        /// <returns>
        /// The list of full paths to the log files.
        /// </returns>
        public string[] GetLogFilesFor(Unit unit)
        {
            return this.CurrentHandler.GetLogFilesFor(unit);
        }

        /// <summary>
        /// Raises the <see cref="CurrentProjectChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCurrentProjectChanged(EventArgs e)
        {
            var handler = this.CurrentProjectChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Saving"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseSaving(CancelEventArgs e)
        {
            var handler = this.Saving;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Saved"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseSaved(EventArgs e)
        {
            var handler = this.Saved;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
